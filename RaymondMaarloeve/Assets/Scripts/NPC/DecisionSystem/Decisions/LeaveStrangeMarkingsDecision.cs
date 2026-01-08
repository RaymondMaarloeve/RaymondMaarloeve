using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to leave strange markings at an obelisk.
/// </summary>
public class LeavingStrangeMarkingsDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LeavingStrangeMarkingsDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the obelisk.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public LeavingStrangeMarkingsDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the shrine.
    /// </summary>
    protected override float StoppingDistance => 1.0f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// This happens outdoors at the obelisk, so the NPC should stay visible.
    /// </summary>
    protected override bool NpcShouldDisappear => false;

    /// <summary>
    /// Duration of time the NPC spends leaving markings.
    /// </summary>
    protected override float WaitDuration => 8f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "leaving strange markings";

    /// <summary>
    /// Called when the decision finishes.
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: add a clue for the detective (markings found later),
        // or increase suspicion if witnessed.
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// </summary>
    protected override bool ShouldFinish() => false;
}
