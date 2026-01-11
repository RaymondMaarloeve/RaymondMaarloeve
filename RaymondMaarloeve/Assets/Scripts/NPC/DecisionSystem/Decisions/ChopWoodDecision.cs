using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to chop wood near a woodpile or yard.
/// </summary>
public class ChopWoodDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChopWoodDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the woodpile or chopping area.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public ChopWoodDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the chopping spot.
    /// </summary>
    protected override float StoppingDistance => 1.0f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// Chopping wood is an outdoor action — the NPC should stay visible.
    /// </summary>
    protected override bool NpcShouldDisappear => false;

    /// <summary>
    /// Duration of time the NPC spends chopping wood.
    /// </summary>
    protected override float WaitDuration => 9f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "chopping wood";

    /// <summary>
    /// Called when the decision finishes (after the waiting time).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: publish an event or add a memory.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Returns false to always wait the full duration.
    /// </summary>
    protected override bool ShouldFinish() => false;
}
