using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to walk to a random destination within a specified radius.
/// </summary>
public class ChatDecision : IChatDecision
{
    /// <summary>
    /// System prompt for generating responses.
    /// </summary>
    private string Prompt => @$"You are now playing the role of a medieval character.
Your name is {npc.Name}.
Below is your story:
{npc.SystemPrompt}
It is {DayNightCycle.Instance.GetCurrentDay()} days after the murder of {GameManager.Instance.generatedHistory.characters.Find(x => x.dead).name}.
{(npc.CharacterData.murderer ?
"You are the murderer. Try to deflect uneasy questions about the murder. Try to not get caught. Don't EVER tell anyone you are the murderer."
: "Try to help the detective with finding the murderer. Answer given questions as best as you can with given information in your story. Don't EVER fabricate or make up new informations about ANYONE or ANYTHING.")}";

    /// <summary>
    /// Cached list containing System prompt
    /// </summary>
    private List<Message> conversationPrefix;

    /// <summary>
    /// All decision's possible states
    /// </summary>
    private enum DecisionState
    {
        GOING_TO_NPC,
        WAITING_FOR_CHATEE,
        WAITING_FOR_GENERATION,
        WAITING_FOR_RESPONSE,
        CONCLUDING,
        FINISHED_CHATTING,
    }

    /// <summary>
    /// The <see cref="NPC"/> associated with this decision.
    /// </summary>
    private readonly NPC npc;

    /// <summary>
    /// The <see cref="NPC"/> this NPC is talking to.
    /// </summary>
    private readonly IChattable otherNpc;

    /// <summary>
    /// Current state of this decision.
    /// </summary>
    private DecisionState currentState;

    /// <summary>
    /// List of messages between npc and chatee. 
    /// </summary>
    private List<Message> conversation;

    /// <summary>
    /// Gets the pretty name of the decision for LLM inference purposes.
    /// </summary>
    public string PrettyName => "chatting";

    /// <summary>
    /// Gets debug information about the current destination.
    /// </summary>
    /// <returns>A string containing the current destination.</returns>
    public string DebugInfo() => $"chatting with {otherNpc.Name}, {currentState}";

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatDecision"/> class.
    /// </summary>
    /// <param name="npc">The <see cref="NPC"> associated with this decision.</param>
    /// <param name="otherNpc">The <see cref="NPC"> this NPC is talking to</param>
    public ChatDecision(NPC npc, IChattable otherNpc, bool interrupted)
    {
        this.npc = npc;
        this.otherNpc = otherNpc;
        if (interrupted)
        {
            currentState = DecisionState.WAITING_FOR_RESPONSE;
        }
        conversation = new List<Message>();
        conversationPrefix = new List<Message> { new Message() { role = "system", content = Prompt + $"\nYou are currently talking with: {otherNpc.Name}" } };
    }

    /// <summary>
    /// Starts the walking decision by setting the <see cref="NPC"/>'s NavMeshAgent destination.
    /// </summary>
    public void Start()
    {
        npc.agent.SetDestination(otherNpc.LookTarget.position);
        currentState = DecisionState.GOING_TO_NPC;
    }

    /// <summary>
    /// Starts concluding the conversation.
    /// </summary>
    public void FinishChatting()
    {
        currentState = DecisionState.CONCLUDING;
        npc.StartCoroutine(DrawConclusions(conversation));
    }

    /// <summary>
    /// Finishes the decision by resetting the <see cref="NPC"/>'s NavMeshAgent path.
    /// </summary>
    public void Finish()
    {
        if (currentState != DecisionState.FINISHED_CHATTING)
        {
            Debug.LogWarning($"{npc.Name}: Finished in wrong state: {currentState}");
        }
        npc.agent.ResetPath();
    }

    /// <summary>
    /// Updates the decision logic. Checks if the NPC has reached its destination.
    /// </summary>
    /// <returns>False if the walk is finished; otherwise, true.</returns>
    public bool Tick()
    {
        if (currentState == DecisionState.FINISHED_CHATTING)
        {
            return false;
        }
        if (currentState == DecisionState.GOING_TO_NPC && !npc.agent.pathPending && npc.agent.remainingDistance < 0.5f)
        {
            currentState = DecisionState.WAITING_FOR_CHATEE;
        }
        if (currentState == DecisionState.WAITING_FOR_CHATEE && otherNpc.AvailableToChat)
        {
            StartConversation();
        }
        return true;
    }

    /// <summary>
    /// Generates response to message
    /// </summary>
    public void Chat(string message)
    {
        // TODO: Parse incoming message
        Debug.Log($"Received message: {message}");
        conversation.Add(new Message() { role = "user", content = message });

        // TODO: Generate response
        var response = "";
        conversation.Add(new Message() { role = "assistant", content = response });
        if (GameManager.Instance.DisableServerConnection)
        {
            otherNpc.Chat("test response");
        }
        else
        {
            LlmManager.Instance.Chat(npc.ModelID, conversationPrefix.Concat(conversation).ToList(), (dto) => otherNpc.Chat(dto.response), (err) => Debug.LogError(err));
        }
    }

    /// <summary>
    /// Updates the decision logic. Checks if the NPC has reached its destination.
    /// </summary>
    private void StartConversation()
    {
        otherNpc.StartChatting(npc);
        // TODO: Generate message
        currentState = DecisionState.WAITING_FOR_GENERATION;
        otherNpc.Chat("hi");
        currentState = DecisionState.WAITING_FOR_RESPONSE;
    }

    /// <summary>
    /// Coroutine that draws conclusions from conversation.,
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator DrawConclusions(List<Message> conversation)
    {
        if (GameManager.Instance.SkipConslusions)
        {
            currentState = DecisionState.FINISHED_CHATTING;
            yield break;
        }

        currentState = DecisionState.CONCLUDING;
        var env = npc.GetCurrentEnvironment();

        string prompt = $"You will be given a conversation between a medieval character {npc.Name} and {otherNpc.Name}.\n" +
                        $"You will write a summary of given conversation and insert it into 'paragraph'.\n" +
                        $"If the Detective was DIRECTLY asking the character to do something, write an index of selected action (1-{env.Count + 1}) into 'action'\n" +
                        $"If not, select 'none'\n" +
                        $"Use simple reasoning and focus only on what is directly said or implied in the conversation.\n" +
                        $"Do not invent information.\n" +
                        $"Do not repeat the entire conversation.\n" +
                        $"Pick only one action.\n" +
                        $"Actions to choose from: [\n" +
                        $"['none'\n{string.Join('\n', env.ConvertAll(x => $"'{x.decision.PrettyName} {(x.associatedGameObject != null ? $"at {x.associatedGameObject?.name.ToLower().Replace("(clone)", "")}'" : "'")}"))}]\n" +
                        $"Conversation: [\n" +
                        string.Join(',', conversation.ConvertAll(x => $"{(x.role == "user" ? otherNpc.Name : npc.Name)}: {x.content}")) +
                        $"]\n" +
                        $"Your response must be ONLY this EXACT CORRECT JSON object:\n" +
                        $"{{\n\"paragraph\": \"generated paragraph here\",\n\"action:\", <action index (1-{env.Count + 1})>\n}}";

        List<Message> messages = new List<Message>();
        messages.Add(new Message { role = "system", content = prompt });
        messages.Add(new Message { role = "user", content = JsonUtility.ToJson(conversation) });

        bool callbackCalled = false;
        string resp = null;

        Debug.Log($"{npc.Name}: Drawing conclusions...");

        LlmManager.Instance.Chat(npc.ModelID, messages, result =>
        {
            callbackCalled = true;
            resp = result.response;
        }, (error) =>
        {
            Debug.LogError($"{npc.Name}: DrawConclusions error: {error}");
            callbackCalled = true;
        }, 0.95f, 0.5f);

        // Wait for the callback to be called
        while (!callbackCalled)
            yield return null;

        if (resp == null)
        {
            Debug.LogError($"{npc.Name}: DrawConclusions error: resp is null");
            yield break;
        }

        if (!resp.Contains('{') || !resp.Contains('}'))
        {
            Debug.LogError($"{npc.Name}: DrawConclusions error: missing JSON brackets\n{resp}");
            yield break;
        }

        string strippedResp = resp.Substring(resp.IndexOf('{'));
        strippedResp = strippedResp.Substring(0, strippedResp.LastIndexOf('}') + 1);

        DrawConclusionsResponseDTO conclusions;
        try
        {
            conclusions = JsonUtility.FromJson<DrawConclusionsResponseDTO>(strippedResp);
        }
        catch (Exception e)
        {
            Debug.LogError($"{npc.Name}: DrawConclusions error: {e.Message}:\nFull response:{resp}\n\nStripped response:{strippedResp}");
            yield break;
        }

        if (conclusions.action < 1 || conclusions.action > env.Count + 1)
        {
            Debug.LogError($"{npc.Name}: DrawConclusions error: wrong action selected: {conclusions.action}\nFull response:{resp}\n\nStripped response:{strippedResp}");
            yield break;
        }

        Debug.Log($"{npc.Name}: DrawConclusions complete:\nSelected action (1->): {conclusions.action}\nParagraph:{conclusions.paragraph}");

        if (conclusions.action > 1)
        {
            currentState = DecisionState.FINISHED_CHATTING;
            var newDecision = env[conclusions.action - 2].decision;
            npc.SetCurrentDecision(newDecision);
            Debug.Log($"{npc.Name}: Concluded and selected decision: {newDecision.DebugInfo()}");
        }
        npc.ObtainedMemories.Add(new ObtainedMemory()
        {
            recency = 10,
            relevance = 10,
            importance = 10,
            memory = conclusions.paragraph
        });
    }
}
