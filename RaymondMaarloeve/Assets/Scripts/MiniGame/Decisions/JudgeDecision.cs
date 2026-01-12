using System.Collections.Generic;
using UnityEngine;
using System;

// Etapy procesu sądowego
public enum TrialStage
{
    WHO_KILLED,     // Kto zabił?
    WHY_KILLED,     // Dlaczego zabił?
    ANYTHING_ELSE   // Czy chcesz coś dodać?
}

// Klasa pomocnicza do parsowania odpowiedzi JSON od LLM
[Serializable]
public class VerdictResponse
{
    public bool isCorrect;
    public string reasoning;
}

public class JudgeDecision : IChatDecision
{
    private readonly TrialStage stage;
    private readonly string crimeContext; // Pełna historia wydarzeń

    // Konstruktor przyjmuje teraz pełną historię (kontekst) zamiast pojedynczych zmiennych
    public JudgeDecision(IChattable owner, bool active, TrialStage stage, string crimeContext)
    {
        this.isActive = active;
        this.owner = owner;
        this.stage = stage;
        this.crimeContext = crimeContext;
    }

    public void Start()
    {
        currentState = isActive ? DecisionState.WAITING_FOR_PLAYER : DecisionState.PASSIVE;
        if (isActive)
        {
            AskPrescribedQuestion();
        }
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
            return;
        }

        currentState = DecisionState.WAITING_FOR_VERDICT_GENERATION;

        List<Message> messages = new List<Message>();

        // 1. Instrukcja Systemowa z pełną historią
        string systemPrompt = GetSystemPrompt();
        messages.Add(new Message { role = "system", content = systemPrompt });

        // 2. Kontekst pytania
        messages.Add(new Message { role = "assistant", content = generatedQuestion });

        // 3. Odpowiedź gracza
        messages.Add(new Message { role = "user", content = message });

        LlmManager.Instance.Chat("npc", messages, result =>
        {
            var rawResponse = result.response;
            Debug.Log($"{owner.Name}: Raw LLM Response: {rawResponse}");

            bool verdict = ParseVerdict(rawResponse);

            MiniGameManager.Instance.Judged(this.owner, verdict);
            currentState = DecisionState.FINISHED;
        }, (error) =>
        {
            Debug.LogError($"{owner.Name}: Chat error: {error}");
            // W razie błędu API zakładamy niewinność
            MiniGameManager.Instance.Judged(this.owner, false);
            currentState = DecisionState.FINISHED;
        });
    }

    private void AskPrescribedQuestion()
    {
        // Pytania są stałe dla każdego etapu
        switch (stage)
        {
            case TrialStage.WHO_KILLED:
                generatedQuestion = "Who killed?";
                break;
            case TrialStage.WHY_KILLED:
                generatedQuestion = "Why did they kill?";
                break;
            case TrialStage.ANYTHING_ELSE:
                generatedQuestion = "Do you want to add anything else?";
                break;
        }

        Debug.Log($"{owner.Name} (Judge): {generatedQuestion}");
        PlayerController.Instance.StartChatting(owner);
        PlayerController.Instance.Chat(generatedQuestion);

        currentState = DecisionState.WAITING_FOR_PLAYER;
    }

    private string GetSystemPrompt()
    {
        return $@"You are a High Judge evaluating the testimony of a DETECTIVE (the user).
        
        OFFICIAL CASE FILE (THE ABSOLUTE TRUTH):
        ""{crimeContext}""

        YOUR TASK:
        The Detective is answering your question: ""{generatedQuestion}"".
        Verify if the Detective's version of events matches the OFFICIAL CASE FILE.

        EVALUATION LOGIC:
        1. IF question is 'Who is the murderer?':
           - If Detective names the correct person found in CASE FILE -> Response is VALID (isCorrect: false).
           - If Detective names the wrong person -> Response is INVALID (isCorrect: true).

        2. IF question is 'What was the motive?':
           - If Detective describes the correct motive found in CASE FILE -> Response is VALID (isCorrect: false).
           - If Detective invents a motive or gets it wrong -> Response is INVALID (isCorrect: true).

        3. IF question is 'Anything to add?':
           - If Detective adds true details or says 'No' -> Response is VALID (isCorrect: false).
           - If Detective contradicts the CASE FILE -> Response is INVALID (isCorrect: true).

        IMPORTANT:
        - Return 'isCorrect': true if the Detective is WRONG/FAILED.
        - Return 'isCorrect': false if the Detective is CORRECT/PASSED.

        RESPONSE FORMAT (JSON ONLY):
        {{
            ""isCorrect"": boolean,
            ""reasoning"": ""short check against the facts""
        }}";
    }

    private bool ParseVerdict(string jsonResponse)
    {
        try
        {
            VerdictResponse response = JsonUtility.FromJson<VerdictResponse>(jsonResponse);
            return response.isCorrect;
        }
        catch (Exception)
        {
            // Fallback: Szukanie klamer JSON ręcznie
            try
            {
                int startIndex = jsonResponse.IndexOf('{');
                int endIndex = jsonResponse.LastIndexOf('}');
                if (startIndex >= 0 && endIndex > startIndex)
                {
                    string jsonOnly = jsonResponse.Substring(startIndex, endIndex - startIndex + 1);
                    VerdictResponse response = JsonUtility.FromJson<VerdictResponse>(jsonOnly);
                    return response.isCorrect;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"JSON Parsing failed. Response: {jsonResponse}. Error: {e}");
            }
        }
        // Ostateczny fallback
        return jsonResponse.ToLower().Contains("true") || jsonResponse.ToLower().Contains("correct");
    }

    public string PrettyName => "judging";
    public string DebugInfo() => $"Judging stage: {stage}. State: {currentState}";

    private string generatedQuestion;
    private DecisionState currentState;

    private enum DecisionState
    {
        PASSIVE,
        WAITING_FOR_PLAYER,
        WAITING_FOR_VERDICT_GENERATION,
        FINISHED
    }

    private readonly IChattable owner;
    private readonly bool isActive;
}
