using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiniGameManager
{
    public MiniGameManager()
    {
        Instance = this;
    }

    public void StartMiniGame()
    {
        // TODO: Spawn all judges
        var judges = new List<NPC>();

        verdicts = new Dictionary<NPC, bool?>();
        foreach (var judge in judges)
        {
            verdicts.Add(judge, null);
            judge.SetCurrentDecision(new JudgeDecision(judge, false));
        }
        Judge();
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

    [SerializeField] private GameObject[] judgePrefabs;

    public static MiniGameManager Instance;
}
