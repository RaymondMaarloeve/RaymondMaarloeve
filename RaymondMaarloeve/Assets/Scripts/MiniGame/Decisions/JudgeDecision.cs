using System.Collections.Generic;
using UnityEngine;

public class JudgeDecision : IChatDecision
{
    public JudgeDecision(IChattable owner, bool active)
    {
        isActive = active;
        this.owner = owner;
    }

    public void Start()
    {
        currentState = isActive ? DecisionState.WAITING_FOR_PLAYER : DecisionState.PASSIVE;
        GenerateQuestion();
    }

    public void Finish()
    {
        PlayerController.Instance.FinishChatting();
    }

    public bool Tick()
    {
        return true;
    }

    public void FinishChatting()
    {
    }

    public void Chat(string message)
    {
        if (currentState != DecisionState.WAITING_FOR_PLAYER)
        {
            Debug.LogWarning($"Received chat in wrong state: {currentState}");
        }

        currentState = DecisionState.WAITING_FOR_VERDICT_GENERATION;

        List<Message> messages = new List<Message>();
        messages.Add(new Message { role = "system", content = prompt });
        messages.Add(new Message { role = "user", content = message });

        LlmManager.Instance.Chat(owner.Name, messages, result =>
        {
            var resp = result.response;
            Debug.Log($"{owner.Name}: Judge decided: {resp}");
            // TODO: Extract the verdict.

            var verdict = false;
            MiniGameManager.Instance.Judged(this.owner, verdict);
            currentState = DecisionState.FINISHED;
        }, (error) =>
        {
            Debug.LogError($"{owner.Name}: Chat error: {error}");
            // TODO: verdict = false?
            currentState = DecisionState.FINISHED;
        });
    }

    public string PrettyName => "judging";

    public string DebugInfo() => $"Judging the player. Current state: {currentState}";

    private void GenerateQuestion()
    {
        currentState = DecisionState.WAITING_FOR_QUESTION_GENERATION;

        List<Message> messages = new List<Message>();
        messages.Add(new Message { role = "system", content = prompt });
        messages.Add(new Message { role = "user", content = generateQuestionPrompt });

        LlmManager.Instance.Chat(owner.Name, messages, result =>
        {
            generatedQuestion = result.response;
            Debug.Log($"{owner.Name}: Judge generated question: {generatedQuestion}");
            PlayerController.Instance.Chat(generatedQuestion);
            currentState = DecisionState.WAITING_FOR_PLAYER;
        }, (error) =>
        {
            Debug.LogError($"{owner.Name}: Chat error: {error}");
            // TODO: verdict = false?
            currentState = DecisionState.FINISHED;
        });
    }

    // TODO: create a prompt
    private string generateQuestionPrompt => @$"You are a judge. Generate a question concerning the murder of XXX";
    private string prompt => @$"You are a judge. You asked {PlayerController.Instance.Name} a question concerning the murder: {generatedQuestion}.";
    private string generatedQuestion;

    private DecisionState currentState;

    private enum DecisionState
    {
        PASSIVE,
        WAITING_FOR_QUESTION_GENERATION,
        WAITING_FOR_PLAYER,
        WAITING_FOR_VERDICT_GENERATION,
        FINISHED
    }

    private readonly IChattable owner;

    private readonly bool isActive;
}
