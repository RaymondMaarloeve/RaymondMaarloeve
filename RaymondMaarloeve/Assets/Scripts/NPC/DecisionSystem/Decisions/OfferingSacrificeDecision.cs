using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to offer a small sacrifice an obelisk.
/// </summary>
public class OfferingSacrificeDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OfferingSacrificeDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the obelisk.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public OfferingSacrificeDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the shrine.
    /// </summary>
    protected override float StoppingDistance => 1.2f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// This happens outdoors at the obelisk, so the NPC should stay visible.
    /// </summary>
    protected override bool NpcShouldDisappear => false;

    /// <summary>
    /// Duration of time the NPC spends making an offering.
    /// </summary>
    protected override float WaitDuration => 10f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "offering a sacrifice";

    /// <summary>
    /// Called when the decision finishes.
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: add a memory, raise suspicion, or affect faction alignment.
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// </summary>
    protected override bool ShouldFinish() => false;
}
