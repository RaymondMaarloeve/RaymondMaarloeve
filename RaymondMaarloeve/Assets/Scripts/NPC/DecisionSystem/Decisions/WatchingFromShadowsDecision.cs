using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to watch someone or something from the shadows,
/// staying partially hidden near a wall, archway, or corner.
/// </summary>
public class WatchingFromShadowsDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WatchingFromShadowsDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">
    /// The GameObject representing the shadowy spot (e.g., a tavern).
    /// </param>
    /// <param name="npc">The NPC performing the decision.</param>
    public WatchingFromShadowsDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the shadowy spot.
    /// A small distance so the NPC stands close to the wall or corner.
    /// </summary>
    protected override float StoppingDistance => 0.8f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// Watching from the shadows should remain visible, so the NPC stays in the scene.
    /// </summary>
    protected override bool NpcShouldDisappear => false;

    /// <summary>
    /// Duration of time the NPC spends watching from the shadows.
    /// </summary>
    protected override float WaitDuration => 7f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "watching from the shadows";

    /// <summary>
    /// Called when the decision finishes (after the watching duration).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: mark this as suspicious or add a memory entry.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Returns false so the NPC always completes the full duration.
    /// </summary>
    /// <returns>False — the decision completes only after WaitDuration elapses.</returns>
    protected override bool ShouldFinish() => false;
}
