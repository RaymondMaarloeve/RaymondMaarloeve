using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to pray to pagan gods at an obelisk.
/// </summary>
public class PaganPrayDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PaganPrayDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the pagan shrine or obelisk.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public PaganPrayDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the pagan obelisk.
    /// </summary>
    protected override float StoppingDistance => 1.2f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// </summary>
    protected override bool NpcShouldDisappear => false;

    /// <summary>
    /// Duration of time the NPC spends praying to pagan gods.
    /// </summary>
    protected override float WaitDuration => 5f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "praying to pagan gods";

    /// <summary>
    /// Called when the decision finishes (after the prayer duration).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: add a heretical / forbidden ritual memory,
        // or flag this NPC as suspicious for church-related quests.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// </summary>
    /// <returns>False — the ritual completes only after WaitDuration elapses.</returns>
    protected override bool ShouldFinish() => false;
}
