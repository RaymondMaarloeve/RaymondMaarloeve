using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to repair the roof or thatch of a house.
/// </summary>
public class RepairRoofDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RepairRoofDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the house or building being repaired.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public RepairRoofDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the repair spot.
    /// </summary>
    protected override float StoppingDistance => 1.5f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// Repairing the roof happens outside, so the NPC should stay visible.
    /// </summary>
    protected override bool NpcShouldDisappear => false;

    /// <summary>
    /// Duration of time the NPC spends repairing the roof.
    /// </summary>
    protected override float WaitDuration => 9f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "repairing the roof";

    /// <summary>
    /// Called when the decision finishes (after the repair duration).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: trigger an event, update a memory, or log the action.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Returns false so the NPC always completes the action.
    /// </summary>
    /// <returns>False — the decision completes only after WaitDuration elapses.</returns>
    protected override bool ShouldFinish() => false;
}
