using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to gossip with townsfolk near a well.
/// </summary>
public class GossipAtWellDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GossipAtWellDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the well.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public GossipAtWellDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the well.
    /// Slightly larger to suggest standing in a small circle of people.
    /// </summary>
    protected override float StoppingDistance => 2.0f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the well.
    /// Gossiping happens outside, so keep the NPC visible.
    /// </summary>
    protected override bool NpcShouldDisappear => false;

    /// <summary>
    /// Duration of time the NPC spends gossiping by the well.
    /// </summary>
    protected override float WaitDuration => 7f;

    /// <summary>
    /// Human-readable name used for the speech bubble / logs.
    /// </summary>
    public override string PrettyName => "gossiping at the well";

    /// <summary>
    /// Called when the decision finishes (after waiting).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: publish an event / memory entry.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Return false to always wait the full duration.
    /// </summary>
    protected override bool ShouldFinish() => false;
}
