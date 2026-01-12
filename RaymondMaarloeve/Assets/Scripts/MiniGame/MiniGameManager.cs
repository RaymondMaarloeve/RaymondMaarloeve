using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameManager : MonoBehaviour
{
    public string fullCrimeStory = "";

    public MiniGameManager()
    {
        Instance = this;
    }

    public void StartMiniGame()
    {
        // 1. Zrespawnuj sędziów
        SpawnJudges();

        var murderer = GameManager.Instance.npcs.Where(x => x.CharacterData.murderer).First();

        fullCrimeStory = GameManager.Instance.generatedHistory.story + $"The murderer is {murderer.Name}";
        Debug.Log(fullCrimeStory);

        verdicts = new Dictionary<NPC, bool?>();
        currentJudgeIndex = 0;

        // 2. Inicjalizacja sędziów z pełną historią
        for (int i = 0; i < judges.Count; i++)
        {
            var judge = judges[i];
            verdicts.Add(judge, null);

            TrialStage stage = GetStageForIndex(i);

            // Przekazujemy 'fullCrimeStory' zamiast pojedynczych faktów
            judge.SetCurrentDecision(new JudgeDecision(judge, false, stage, fullCrimeStory));
        }

        // 3. Aktywacja pierwszego
        ActivateCurrentJudge();
    }

    private TrialStage GetStageForIndex(int index)
    {
        if (index == 0) return TrialStage.WHO_KILLED;
        if (index == 1) return TrialStage.WHY_KILLED;
        return TrialStage.ANYTHING_ELSE;
    }

    public void Judged(IChattable judge, bool verdict)
    {
        verdicts[(NPC)judge] = verdict;
        Debug.Log($"Judge {judge.Name} verdict registered: {(verdict ? "NOT CORRECT" : "CORRECT")}");

        // Reset sędziego
        var npcJudge = (NPC)judge;
        TrialStage finishedStage = GetStageForIndex(currentJudgeIndex);
        npcJudge.SetCurrentDecision(new JudgeDecision(npcJudge, false, finishedStage, fullCrimeStory));

        // Następny
        currentJudgeIndex++;

        if (currentJudgeIndex >= judges.Count)
        {
            EndingSequence();
        }
        else
        {
            ActivateCurrentJudge();
        }
    }

    private void ActivateCurrentJudge()
    {
        if (currentJudgeIndex < judges.Count)
        {
            var activeJudge = judges[currentJudgeIndex];
            TrialStage stage = GetStageForIndex(currentJudgeIndex);

            Debug.Log($"Activating Judge {currentJudgeIndex} for stage: {stage}");
            activeJudge.SetCurrentDecision(new JudgeDecision(activeJudge, true, stage, fullCrimeStory));
        }
    }

    private void EndingSequence()
    {
        var votedGuilty = verdicts.Values.Where(x => x.HasValue && x.Value).Count();
        var resultMessage = $"Trial Finished. Guilty Votes: {votedGuilty}/{verdicts.Count}";

        if (votedGuilty > 0.5f * verdicts.Count)
        {
            Debug.Log("FINAL RESULT: GUILTY");
        }
        else
        {
            Debug.Log("FINAL RESULT: NOT GUILTY");
        }

        resultMessage += $"\nFull story: {GameManager.Instance.generatedHistory.story}";

        Debug.Log(resultMessage);

        PlayerPrefs.SetString("GameResult", resultMessage);
        PlayerPrefs.Save();
        SceneManager.LoadScene("EndScene");
    }

    private NPC SpawnJudge()
    {
        Vector3 npcPosition = new Vector3(
            MapGenerator.Instance.transform.position.x - MapGenerator.Instance.mapWidth / 2 + Random.Range(0, MapGenerator.Instance.mapWidth),
            0,
            MapGenerator.Instance.transform.position.z - MapGenerator.Instance.mapLength / 2 + Random.Range(0, MapGenerator.Instance.mapLength)
        );

        int npcVariant = Random.Range(0, GameManager.Instance.npcPrefabs.Length);

        GameObject newNpc = Instantiate(GameManager.Instance.npcPrefabs[npcVariant], npcPosition, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(newNpc, SceneManager.GetSceneByName("Game"));

        var npcComponent = newNpc.GetComponent<NPC>();

        IDecisionSystem system = new NullDecisionSystem();
        npcComponent.Setup(system, "npc", new CharacterDTO { name = "Judge" });

        return npcComponent;
    }

    private void SpawnJudges()
    {
        judges = new List<NPC>();

        GameObject wallsRoot = GameObject.Find("WallsRoot");
        Transform gates = null;

        foreach (Transform child in wallsRoot.transform)
        {
            if (child.name == "GATE(Clone)")
            {
                gates = child;
                break;
            }
        }

        if (gates == null)
        {
            Debug.LogError("Nie znaleziono bramy (Gate(Clone))!");
        }

        Transform entrance = (gates != null) ? gates.Find("PlayerSpawner") : null;

        judges.Add(SpawnJudge());
        judges.Add(SpawnJudge());
        judges.Add(SpawnJudge());

        if (entrance != null)
        {
            for (int i = 0; i < judges.Count; i++)
            {
                var judge = judges[i];
                judge.transform.position = entrance.position - new Vector3(-1.5f + (i * 1.5f), 0, 0);
                judge.transform.rotation = entrance.rotation;
            }
        }
    }

    private bool EveryoneJudged => !verdicts.Values.Any(x => x == null);

    private Dictionary<NPC, bool?> verdicts;
    private List<NPC> judges;
    private int currentJudgeIndex = 0;

    public static MiniGameManager Instance;

    [ConsoleCommand("minigame", "Start minigame")]
    public static bool StartMiniGameCommand()
    {
        Instance.StartMiniGame();
        return true;
    }
}
