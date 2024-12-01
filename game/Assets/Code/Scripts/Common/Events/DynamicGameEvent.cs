using System.Collections.Generic;
using UnityEngine;

public class DynamicGameEvent<T> : GameEvent
{
    private readonly List<IGameEventListener<T>> _genericListeners = new();
    public T Value { get; private set; }

    public void Raise(T value)
    {
        Debug.Log($"Dynamic event raised with value: {value} and listeners: {_genericListeners.Count}");
        Value = value;
        
        for (int i = _genericListeners.Count - 1; i >= 0; i--)
        {
            _genericListeners[i].OnEventRaised(value);
        }
    }

    public void RegisterListener(IGameEventListener<T> listener)
    {
        if (!_genericListeners.Contains(listener))
        {
            _genericListeners.Add(listener);
        }
    }

    public void UnregisterListener(IGameEventListener<T> listener)
    {
        if (_genericListeners.Contains(listener))
        {
            _genericListeners.Remove(listener);
        }
    }
}