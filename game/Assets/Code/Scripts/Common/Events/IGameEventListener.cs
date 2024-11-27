public interface IGameEventListener
{
    virtual void OnEventRaised() { }
    virtual void OnEventRaised<T>(T t) { }
}
