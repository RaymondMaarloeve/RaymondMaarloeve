using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameManager : MonoBehaviour
{
    public MiniGameManager()
    {
        Instance = this;
    }

    public void StartMiniGame()
    {
        SpawnJudges();

        verdicts = new Dictionary<NPC, bool?>();
        foreach (var judge in judges)
        {
            verdicts.Add(judge, null);
            judge.SetCurrentDecision(new JudgeDecision(judge, false));
        }
        Judge();
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
            //return;
        }

        // Znajdź Entrance w _minnor_gates_02(Clone)
        Transform entrance = gates.Find("PlayerSpawner");
        if (entrance == null)
        {
            Debug.LogError("Nie znaleziono PlayerSpawner!");
            //return;
        }

        judges.Add(SpawnJudge());
        judges.Add(SpawnJudge());
        judges.Add(SpawnJudge());

        for (int i = 0; i < judges.Count; i++)
        {
            var judge = judges[i];
            judge.transform.position = entrance.position - new Vector3(-0.5f * i, 0, 0);
            judge.transform.rotation = entrance.rotation;

            Debug.Log($"pos {judge.transform.position}");
        }
    }

    public void Judged(IChattable judge, bool verdict)
    {
        if (EveryoneJudged)
        {
            EndingSequence();
            return;
        }

        ((NPC)judge).SetCurrentDecision(new JudgeDecision(judge, false));
    }

    private void Judge()
    {
        var judges = verdicts.Where(x => x.Value == null).Select(x => x.Key).ToList();
        var selectedJudge = judges[Random.Range(0, judges.Count)];
        selectedJudge.SetCurrentDecision(new JudgeDecision(selectedJudge, true));
    }

    private void EndingSequence()
    {
        var votedGuilty = verdicts.Values.Where(x => x.HasValue && x.Value).ToList().Count;
        if (votedGuilty > 0.5f * verdicts.Count)
        {
            Debug.Log("Guilty");
        }
        else
        {
            Debug.Log("Not guilty");
        }
    }

    private bool EveryoneJudged => !verdicts.Values.Any(x => x == null);

    // null -> did not judge
    // true -> guilty
    // false -> not guilty
    private Dictionary<NPC, bool?> verdicts;

    private List<NPC> judges;

    public static MiniGameManager Instance;

    [ConsoleCommand("minigame", "Start minigame")]
    public static bool StartMiniGameCommand()
    {
        Instance.StartMiniGame();
        return true;
    }
}
