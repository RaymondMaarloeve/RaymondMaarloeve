using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to warm their hands near a fire or hearth.
/// </summary>
public class WarmHandsDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WarmHandsDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the fire or hearth.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public WarmHandsDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// How close the NPC should stop near the fire.
    /// A small distance makes them stand naturally near the heat source.
    /// </summary>
    protected override float StoppingDistance => 1.0f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// Warming hands is done outside or in public — keep the NPC visible.
    /// </summary>
    protected override bool NpcShouldDisappear => false;

    /// <summary>
    /// Duration of time the NPC spends warming hands near the fire.
    /// </summary>
    protected override float WaitDuration => 6f;

    /// <summary>
    /// Human-readable description of the decision, used for speech bubbles or logs.
    /// </summary>
    public override string PrettyName => "warming hands by the fire";

    /// <summary>
    /// Called when the decision finishes (after waiting near the fire).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: publish an event, e.g. NPC gained comfort, or record memory.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines if the decision should finish prematurely.
    /// Return false to let it complete naturally after WaitDuration.
    /// </summary>
    /// <returns>False — the decision ends only when WaitDuration elapses.</returns>
    protected override bool ShouldFinish() => false;
}
