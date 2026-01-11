using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to bury something in the ground behind a building or in a secluded spot.
/// </summary>
public class BuryingSomethingDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BuryingSomethingDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the burying location (e.g., behind a house).</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public BuryingSomethingDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the burying spot.
    /// </summary>
    protected override float StoppingDistance => 1.0f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// Burying something is an outdoor, visible action, so the NPC stays visible.
    /// </summary>
    protected override bool NpcShouldDisappear => false;

    /// <summary>
    /// Duration of time the NPC spends burying the object.
    /// </summary>
    protected override float WaitDuration => 8f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "burying something";

    /// <summary>
    /// Called when the decision finishes (after the burying duration).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: record a suspicious memory or send an event.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Returns false so the NPC always completes the action.
    /// </summary>
    /// <returns>False — the decision completes only after WaitDuration elapses.</returns>
    protected override bool ShouldFinish() => false;
}
