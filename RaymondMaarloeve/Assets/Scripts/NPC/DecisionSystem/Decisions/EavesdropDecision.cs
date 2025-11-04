using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to eavesdrop by a window (outside a building).
/// </summary>
public class EavesdropDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EavesdropDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the target building (or its Entrance if present).</param>
    /// <param name="npc">The NPC making the decision.</param>
    public EavesdropDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// Stopping distance for the NPC when approaching the building.
    /// Keep it small so the NPC stands close to the wall/window.
    /// </summary>
    protected override float StoppingDistance => 0.6f;

    /// <summary>
    /// Whether the NPC should disappear after completing the decision.
    /// Eavesdropping happens outside, so the NPC should remain visible.
    /// </summary>
    protected override bool NpcShouldDisappear => false;

    /// <summary>
    /// Duration the NPC should wait at the building while eavesdropping.
    /// </summary>
    protected override float WaitDuration => 7f;

    /// <summary>
    /// Human-readable name of the decision (used for the speech bubble / logging).
    /// </summary>
    public override string PrettyName => "eavesdropping at the window";

    /// <summary>
    /// Called when the decision is finished.
    /// No state changes by default (you can hook memory/events here if desired).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: publish an event or adjust suspicion, etc.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should finish earlier than the default timer.
    /// Return false to rely solely on WaitDuration.
    /// </summary>
    /// <returns>False, indicating the decision should not finish prematurely.</returns>
    protected override bool ShouldFinish() => false;
}
