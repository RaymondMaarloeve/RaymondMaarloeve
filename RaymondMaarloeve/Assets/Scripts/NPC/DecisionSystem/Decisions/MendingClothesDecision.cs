using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to mend or repair clothes (sewing, patching fabric).
/// </summary>
public class MendingClothesDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MendingClothesDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the house or indoor place where the NPC mends clothes.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public MendingClothesDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the place.
    /// </summary>
    protected override float StoppingDistance => 1.2f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// Mending clothes is typically done indoors, so the NPC should disappear.
    /// </summary>
    protected override bool NpcShouldDisappear => true;

    /// <summary>
    /// Duration of time the NPC spends mending clothes.
    /// </summary>
    protected override float WaitDuration => 9f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "mending clothes";

    /// <summary>
    /// Called when the decision finishes (after the mending duration).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: add a memory or trigger an event.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Returns false so the NPC always completes the action.
    /// </summary>
    protected override bool ShouldFinish() => false;
}
