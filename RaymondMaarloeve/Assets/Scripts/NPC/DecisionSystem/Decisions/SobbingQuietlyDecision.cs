using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to quietly sob inside their home.
/// </summary>
public class SobbingQuietlyDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SobbingQuietlyDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the house.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public SobbingQuietlyDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the house.
    /// </summary>
    protected override float StoppingDistance => 1.2f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// Sobbing happens indoors, so the NPC should disappear.
    /// </summary>
    protected override bool NpcShouldDisappear => true;

    /// <summary>
    /// Duration of time the NPC spends sobbing quietly inside.
    /// </summary>
    protected override float WaitDuration => 9f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "sobbing quietly";

    /// <summary>
    /// Called when the decision finishes (after the sobbing duration).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: add an emotional or tragic memory.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Returns false to ensure the full duration.
    /// </summary>
    /// <returns>False — the decision completes only after WaitDuration elapses.</returns>
    protected override bool ShouldFinish() => false;
}
