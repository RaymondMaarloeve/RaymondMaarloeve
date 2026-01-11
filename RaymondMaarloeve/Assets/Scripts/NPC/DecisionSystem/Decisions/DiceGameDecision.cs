using UnityEngine;

/// <summary>
/// Represents a decision for an NPC to play a dice game inside a tavern.
/// </summary>
public class DiceGameDecision : VisitBuildingDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiceGameDecision"/> class.
    /// </summary>
    /// <param name="buildingGO">The GameObject representing the tavern or gathering place.</param>
    /// <param name="npc">The NPC performing the decision.</param>
    public DiceGameDecision(GameObject buildingGO, NPC npc) : base(buildingGO, npc)
    {
    }

    /// <summary>
    /// The stopping distance for the NPC when approaching the tavern.
    /// </summary>
    protected override float StoppingDistance => 1.2f;

    /// <summary>
    /// Whether the NPC should disappear after reaching the tavern.
    /// NPCs go inside to play dice, so they should temporarily disappear.
    /// </summary>
    protected override bool NpcShouldDisappear => true;

    /// <summary>
    /// Duration of time the NPC spends playing dice.
    /// </summary>
    protected override float WaitDuration => 10f;

    /// <summary>
    /// Human-readable name of the decision, used for speech bubbles and logs.
    /// </summary>
    public override string PrettyName => "playing dice";

    /// <summary>
    /// Called when the decision finishes (after the NPC finishes playing).
    /// </summary>
    protected override void OnFinished()
    {
        // Optionally: log the action or trigger a memory event.
        // NpcEventBus.Publish(new NpcActionEvent(npc.EntityID, PrettyName + " (finished)", npc.transform.position));
    }

    /// <summary>
    /// Determines whether the decision should finish prematurely.
    /// Return false to ensure full WaitDuration.
    /// </summary>
    /// <returns>False — decision completes only after the duration ends.</returns>
    protected override bool ShouldFinish() => false;
}
