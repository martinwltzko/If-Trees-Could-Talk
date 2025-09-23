using AdvancedController;
using EventHandling;

public class PlayerLoadedEvent : IEvent
{
    public PlayerInstance PlayerInstance { get; }
    public bool Loaded { get; }
    
    public PlayerLoadedEvent(PlayerInstance playerInstance, bool loaded)
    {
        PlayerInstance = playerInstance;
        Loaded = loaded;
    }
}