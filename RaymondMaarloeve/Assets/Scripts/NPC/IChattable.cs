using UnityEngine;

public interface IChattable
{
    void StartChatting(IChattable otherChatter);

    void FinishChatting();

    void Chat(string message);

    Transform LookTarget { get; }

    string Name { get; }

    bool AvailableToChat { get; }
}
