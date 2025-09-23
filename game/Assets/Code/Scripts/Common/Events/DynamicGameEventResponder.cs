using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

/// <summary>
/// Versatile class that allows to define responds for all scriptable events
/// </summary>
public class UnityDynamicGameEventListener : MonoBehaviour, IGameEventListener
{
    [SerializeField] private List<GameEventListener> gameEventListener;
    [SerializeField] private List<TransformEventListener> gameEventListenerWithTransform;
    
    [FormerlySerializedAs("_gameEventListenerWithFloat")] 
    [SerializeField] private List<FloatEventListener> gameEventListenerWithFloat;
    
    [FormerlySerializedAs("_gameEventListenerWithInt")] 
    [SerializeField] private List<IntEventListener> gameEventListenerWithInt;
    
    [FormerlySerializedAs("_gameEventListenerWithBool")] 
    [SerializeField] private List<BoolEventListener> gameEventListenerWithBool;

    private void OnEnable()
    {
        foreach (TransformEventListener eventListener in gameEventListenerWithTransform)
        {
            eventListener.OnEnable();
        }
        foreach (FloatEventListener eventListener in gameEventListenerWithFloat)
        {
            eventListener.OnEnable();
        }
        foreach (IntEventListener eventListener in gameEventListenerWithInt)
        {
            eventListener.OnEnable();
        }
        foreach (BoolEventListener eventListener in gameEventListenerWithBool)
        {
            eventListener.OnEnable();
        }
    }

    private void OnDisable()
    {
        foreach (TransformEventListener eventListener in gameEventListenerWithTransform)
        {
            eventListener.OnDisable();
        }
        foreach (FloatEventListener eventListener in gameEventListenerWithFloat)
        {
            eventListener.OnDisable();
        }
        foreach (IntEventListener eventListener in gameEventListenerWithInt)
        {
            eventListener.OnDisable();
        }
        foreach (BoolEventListener eventListener in gameEventListenerWithBool)
        {
            eventListener.OnDisable();
        }
    }
}

[Serializable]
public class TransformEventListener : IGameEventListener<Transform>
{
    public TransformGameEvent @event;
    public UnityEvent<Transform> response;
    
    public void OnEventRaised(Transform value)
    {
        Debug.Log("Transform event raised with transform: " + @event.Value.name);
        response?.Invoke(value);
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
public class FloatEventListener : IGameEventListener<float>
{
    public FloatGameEvent @event;
    public UnityEvent<float> response;
    
    public void OnEventRaised(float value)
    {
        Debug.Log("Float event raised with value: " + @event.Value);
        response?.Invoke(value);
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
public class BoolEventListener : IGameEventListener<bool>
{
    public BoolGameEvent @event;
    public UnityEvent<bool> response;
    
    public void OnEventRaised(bool value)
    {
        Debug.Log("Bool event raised with value: " + @event.Value);
        response?.Invoke(value);
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
public class IntEventListener : IGameEventListener<int>
{
    public IntGameEvent @event;
    public UnityEvent<int> response;
    
    public void OnEventRaised(int value)
    {
        Debug.Log("Int event raised with value: " + @event.Value);
        response?.Invoke(value);
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
public class ComponentEventListener : IGameEventListener<Component>
{
    public ComponentGameEvent @event;
    public UnityEvent<Component> response;
    
    public void OnEventRaised(Component value)
    {
        Debug.Log("Int event raised with value: " + @event.Value);
        response?.Invoke(value);
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