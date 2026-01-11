using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to sweep the yard or area around a building.
/// </summary>
public class SweepYardDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SweepYardDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the house, tavern, or yard area.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public SweepYardDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the sweeping spot.
    /// </summary>
    protected override float StoppingDistance => 1.0f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// Sweeping the yard is an outdoor action, so the NPC should stay visible.
    /// </summary>
    protected override bool NpcShouldDisappear => false;

    /// <summary>
    /// Duration of time the NPC spends sweeping the yard.
    /// </summary>
    protected override float WaitDuration => 7f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "sweeping the yard";

    /// <summary>
    /// Called when the decision finishes (after the sweeping duration).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: trigger an event or add a memory.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Returns false so the NPC always waits the full duration.
    /// </summary>
    /// <returns>False — the decision completes only after WaitDuration elapses.</returns>
    protected override bool ShouldFinish() => false;
}
