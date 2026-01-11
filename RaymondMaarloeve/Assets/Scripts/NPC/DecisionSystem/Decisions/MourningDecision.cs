using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to mourn in silence at a church or cemetery.
/// </summary>
public class MourningDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MourningDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the church or mourning place.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public MourningDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the mourning place.
    /// </summary>
    protected override float StoppingDistance => 1.2f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// </summary>
    protected override bool NpcShouldDisappear => true;

    /// <summary>
    /// Duration of time the NPC spends mourning.
    /// </summary>
    protected override float WaitDuration => 10f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "mourning in silence";

    /// <summary>
    /// Called when the decision finishes (after the mourning duration).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: add an emotional memory or trigger a narrative event.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Returns false so the NPC always completes the mourning period.
    /// </summary>
    /// <returns>False � the decision completes only after WaitDuration elapses.</returns>
    protected override bool ShouldFinish() => false;
}
