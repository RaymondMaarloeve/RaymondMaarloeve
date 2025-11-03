using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to tend to a garden or tiny crop field.
/// </summary>
public class TendGardenDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TendGardenDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the garden or crop area.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public TendGardenDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the garden.
    /// </summary>
    protected override float StoppingDistance => 1.0f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// Tending the garden happens outside, so the NPC should remain visible.
    /// </summary>
    protected override bool NpcShouldDisappear => false;

    /// <summary>
    /// Duration of time the NPC spends tending the garden.
    /// </summary>
    protected override float WaitDuration => 8f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "tending the garden";

    /// <summary>
    /// Called when the decision finishes (after waiting near the garden).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: add a memory or trigger an event.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Return false to wait for the full duration.
    /// </summary>
    /// <returns>False — the decision completes only after WaitDuration elapses.</returns>
    protected override bool ShouldFinish() => false;
}
