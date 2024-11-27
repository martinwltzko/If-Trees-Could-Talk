using System;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class UnityDynamicGameEventListener : MonoBehaviour, IGameEventListener
{
    [SerializeField] private ObjectEventListener _gameEventListenerWithObject;
    [SerializeField] private FloatEventListener _gameEventListenerWithFloat;
    [SerializeField] private IntEventListener _gameEventListenerWithInt;
}

[Serializable]
public class ObjectEventListener : IGameEventListener
{
    public DynamicGameEvent @event;
    public UnityEvent<object> response;
    
    public void OnEventRaised()
    {
        response?.Invoke(@event.Meta);
    }

    public void OnEnable()
    {
        if(@event != null) @event.RegisterListener(this);
    }

    public void OnDisable()
    {
        if(@event != null) @event.UnregisterListener(this);
    }
}

[Serializable]
public class FloatEventListener : IGameEventListener
{
    public DynamicGameEvent @event;
    public UnityEvent<float> response;
    
    public void OnEventRaised()
    {
        response?.Invoke((float)@event.Meta);
    }

    public void OnEnable()
    {
        if(@event != null) @event.RegisterListener(this);
    }

    public void OnDisable()
    {
        if(@event != null) @event.UnregisterListener(this);
    }
}

[Serializable]
public class IntEventListener : IGameEventListener
{
    public DynamicGameEvent @event;
    public UnityEvent<int> response;
    
    public void OnEventRaised()
    {
        response?.Invoke((int)@event.Meta);
    }

    public void OnEnable()
    {
        if(@event != null) @event.RegisterListener(this);
    }

    public void OnDisable()
    {
        if(@event != null) @event.UnregisterListener(this);
    }
}