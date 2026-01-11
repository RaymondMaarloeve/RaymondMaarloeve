using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to sing drunkenly inside a tavern.
/// </summary>
public class SingingDrunkDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SingingDrunkDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the tavern.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public SingingDrunkDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the tavern.
    /// </summary>
    protected override float StoppingDistance => 1.2f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the tavern.
    /// Singing happens inside, so the NPC should disappear.
    /// </summary>
    protected override bool NpcShouldDisappear => true;

    /// <summary>
    /// Duration of time the NPC spends singing drunkenly.
    /// </summary>
    protected override float WaitDuration => 7f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "singing drunkenly";

    /// <summary>
    /// Called when the decision finishes (after the singing).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: trigger ambience, attention, or a memory.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Returns false so the NPC always completes the action.
    /// </summary>
    /// <returns>False — the decision completes only after WaitDuration elapses.</returns>
    protected override bool ShouldFinish() => false;
}
