public interface IGameEventListener
{
    virtual void OnEventRaised() { }
}

public interface IGameEventListener<T>
{
    void OnEventRaised(T value);
}
