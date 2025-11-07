using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to cook a stew or meal at home.
/// </summary>
public class CookStewDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CookStewDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the house or kitchen area.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public CookStewDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the cooking spot.
    /// </summary>
    protected override float StoppingDistance => 1.0f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the target.
    /// Cooking happens inside.
    /// </summary>
    protected override bool NpcShouldDisappear => true;

    /// <summary>
    /// Duration of time the NPC spends cooking the stew.
    /// </summary>
    protected override float WaitDuration => 8f;

    /// <summary>
    /// Human-readable name used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "cooking stew";

    /// <summary>
    /// Called when the decision finishes (after the cooking duration).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: trigger a memory or log the action.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should end prematurely.
    /// Return false to let the action finish naturally.
    /// </summary>
    /// <returns>False — the decision completes only after WaitDuration elapses.</returns>
    protected override bool ShouldFinish() => false;
}
