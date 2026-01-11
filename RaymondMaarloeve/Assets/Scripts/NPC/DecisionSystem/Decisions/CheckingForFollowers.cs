using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to look around nervously, checking if anyone is following them.
/// </summary>
public class CheckingForFollowersDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CheckingForFollowersDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the spot where the NPC stops (e.g. an entrance).</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public CheckingForFollowersDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the checking spot.
    /// </summary>
    protected override float StoppingDistance => 1.0f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// Checking for followers is a visible, nervous action, so the NPC stays visible.
    /// </summary>
    protected override bool NpcShouldDisappear => false;

    /// <summary>
    /// Duration of time the NPC spends checking if someone is following them.
    /// </summary>
    protected override float WaitDuration => 6f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "checking for followers";

    /// <summary>
    /// Called when the decision finishes (after the checking duration).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: mark this as a suspicious memory or trigger an event.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Returns false so the NPC always completes the action.
    /// </summary>
    /// <returns>False — the decision completes only after WaitDuration elapses.</returns>
    protected override bool ShouldFinish() => false;
}
