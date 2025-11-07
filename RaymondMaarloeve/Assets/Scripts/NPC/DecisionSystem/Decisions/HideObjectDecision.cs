using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to hide an object behind a building, in a barn, or in an alley.
/// </summary>
public class HideObjectDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HideObjectDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the hiding spot or nearby building.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public HideObjectDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the hiding spot.
    /// </summary>
    protected override float StoppingDistance => 1.0f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// Hiding an object usually happens outside, so the NPC stays visible.
    /// </summary>
    protected override bool NpcShouldDisappear => false;

    /// <summary>
    /// Duration of time the NPC spends hiding the object.
    /// </summary>
    protected override float WaitDuration => 6f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "hiding something";

    /// <summary>
    /// Called when the decision finishes (after the hiding duration).
    /// </summary>
    protected override void OnFinished()
    {
        // Optional: add a suspicious memory or trigger an event.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Returns false to ensure the full duration elapses.
    /// </summary>
    /// <returns>False — the decision completes only after WaitDuration elapses.</returns>
    protected override bool ShouldFinish() => false;
}
