using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to read public announcements on the town notice board.
/// </summary>
public class ReadNoticeBoardDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadNoticeBoardDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the town notice board.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public ReadNoticeBoardDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the notice board.
    /// </summary>
    protected override float StoppingDistance => 1.2f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// Reading the board happens in public, so keep the NPC visible.
    /// </summary>
    protected override bool NpcShouldDisappear => false;

    /// <summary>
    /// Duration of time the NPC spends reading announcements.
    /// </summary>
    protected override float WaitDuration => 5f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "reading the notice board";

    /// <summary>
    /// Called when the decision finishes (after waiting near the notice board).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: trigger an event or add a memory.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should finish prematurely.
    /// Returns false to rely on the WaitDuration.
    /// </summary>
    /// <returns>False — decision completes only after the duration ends.</returns>
    protected override bool ShouldFinish() => false;
}
