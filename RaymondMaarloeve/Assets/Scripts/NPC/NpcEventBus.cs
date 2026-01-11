using System;
using UnityEngine;

/// <summary>
/// Represents an event describing an action performed by an NPC.
/// </summary>
public class NpcActionEvent
{
    /// <summary>
    /// The source NPC's entity ID.
    /// </summary>
    public int SourceId;

    /// <summary>
    /// The action performed by the NPC.
    /// </summary>
    public string Action;

    /// <summary>
    /// The position where the action was performed.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// The timestamp when the event was created.
    /// </summary>
    public float Timestamp;

    /// <summary>
    /// Constructs a new NpcActionEvent.
    /// </summary>
    /// <param name="sourceId">The source NPC's entity ID.</param>
    /// <param name="action">The action performed.</param>
    /// <param name="position">The position of the action.</param>
    public NpcActionEvent(int sourceId, string action, Vector3 position)
    {
        SourceId = sourceId;
        Action = action;
        Position = position;
        Timestamp = Time.time;
    }
}

/// <summary>
/// Event bus for publishing and subscribing to NPC action events.
/// </summary>
public static class NpcEventBus
{
    /// <summary>
    /// Event triggered when an NPC action occurs.
    /// </summary>
    public static event Action<NpcActionEvent> OnNpcAction;

    /// <summary>
    /// Publishes an NPC action event to all subscribers.
    /// </summary>
    /// <param name="e">The event to publish.</param>
    public static void Publish(NpcActionEvent e)
    {
        OnNpcAction?.Invoke(e);
    }
}
