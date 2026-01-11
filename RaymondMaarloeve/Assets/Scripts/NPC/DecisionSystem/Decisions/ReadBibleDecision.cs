using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to read the Bible (or a holy book) inside the church.
/// </summary>
public class ReadBibleDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadBibleDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the church.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public ReadBibleDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the church.
    /// </summary>
    protected override float StoppingDistance => 1.2f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the church.
    /// Reading the Bible is done inside, so the NPC should temporarily disappear.
    /// </summary>
    protected override bool NpcShouldDisappear => true;

    /// <summary>
    /// Duration of time the NPC spends reading inside the church.
    /// </summary>
    protected override float WaitDuration => 10f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "reading the Bible";

    /// <summary>
    /// Called when the decision finishes (after the reading duration).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: trigger an event or add a memory.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Returns false to ensure the full duration passes.
    /// </summary>
    /// <returns>False — the decision completes only after WaitDuration elapses.</returns>
    protected override bool ShouldFinish() => false;
}
