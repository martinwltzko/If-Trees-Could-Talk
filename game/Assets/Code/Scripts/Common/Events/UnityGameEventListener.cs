using System;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class UnityGameEventListener : MonoBehaviour, IGameEventListener
{
    [SerializeField] private GameEventListener _gameEventListener;

    void OnEnable()
    {
        _gameEventListener.OnEnable();
    }

    private void OnDisable()
    {
        _gameEventListener.OnDisable();
    }

    public void OnEventRaised()
    {
        _gameEventListener.OnEventRaised();
    }
}

[Serializable]
public class GameEventListener : IGameEventListener
{
    [SerializeField] private GameEvent @event;
    [SerializeField] private UnityEvent response;
    
    public GameEventListener(GameEvent gameEvent, UnityEvent response)
    {
        this.@event = gameEvent;
        this.response = response;

        if (@event != null) @event.RegisterListener(this);
    }
    
    public void OnEventRaised()
    {
        response?.Invoke();
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