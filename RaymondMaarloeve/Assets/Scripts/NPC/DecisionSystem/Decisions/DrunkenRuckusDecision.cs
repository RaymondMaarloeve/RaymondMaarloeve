using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to cause a drunken ruckus inside a tavern.
/// </summary>
public class DrunkenRuckusDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrunkenRuckusDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the tavern.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public DrunkenRuckusDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the tavern.
    /// </summary>
    protected override float StoppingDistance => 1.2f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the tavern.
    /// The ruckus happens inside, so the NPC should disappear.
    /// </summary>
    protected override bool NpcShouldDisappear => true;

    /// <summary>
    /// Duration of time the NPC spends causing a drunken disturbance.
    /// </summary>
    protected override float WaitDuration => 6f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "causing a drunken ruckus";

    /// <summary>
    /// Called when the decision finishes (after the ruckus).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: trigger suspicion, guard reaction, or memory.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Returns false so the full disturbance plays out.
    /// </summary>
    /// <returns>False — the decision completes only after WaitDuration elapses.</returns>
    protected override bool ShouldFinish() => false;
}
