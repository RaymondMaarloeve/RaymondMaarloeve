using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to fetch firewood from a nearby woodpile or forest area.
/// </summary>
public class FetchFirewoodDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FetchFirewoodDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the target location (e.g. woodpile, forest edge).</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public FetchFirewoodDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the target location.
    /// A small value so the NPC stands near the woodpile or tree line.
    /// </summary>
    protected override float StoppingDistance => 1.0f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// Fetching firewood happens outside, so the NPC remains visible.
    /// </summary>
    protected override bool NpcShouldDisappear => false;

    /// <summary>
    /// How long the NPC should remain at the target location while fetching wood.
    /// </summary>
    protected override float WaitDuration => 8f;

    /// <summary>
    /// Human-readable name of the decision (used for UI bubbles or logs).
    /// </summary>
    public override string PrettyName => "fetching firewood";

    /// <summary>
    /// Called when the decision finishes (after waiting).
    /// You can trigger a memory event or adjust a resource variable here.
    /// </summary>
    protected override void OnFinished()
    {
        // Example: notify event system or log the action
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Returns false so the NPC always waits the full duration.
    /// </summary>
    /// <returns>False — decision lasts until WaitDuration elapses.</returns>
    protected override bool ShouldFinish() => false;
}
