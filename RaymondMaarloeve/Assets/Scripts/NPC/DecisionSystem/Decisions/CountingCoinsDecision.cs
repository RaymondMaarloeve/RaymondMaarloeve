using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to count coins, usually at a table inside a tavern.
/// </summary>
public class CountingCoinsDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CountingCoinsDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the tavern (or place to count coins).</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public CountingCoinsDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the tavern.
    /// </summary>
    protected override float StoppingDistance => 1.2f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// Counting coins is usually done indoors at a table, so the NPC should disappear.
    /// </summary>
    protected override bool NpcShouldDisappear => true;

    /// <summary>
    /// Duration of time the NPC spends counting coins.
    /// </summary>
    protected override float WaitDuration => 7f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "counting coins";

    /// <summary>
    /// Called when the decision finishes (after counting coins).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: create a memory or trigger an event.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Returns false so the NPC always completes the action.
    /// </summary>
    protected override bool ShouldFinish() => false;
}
