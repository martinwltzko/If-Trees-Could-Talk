using System;
using Code.Scripts.Input;
using EventHandling;
using Sirenix.OdinInspector;
using UnityEngine;

public class UILoadedEvent : IEvent
{
    public bool IsLoaded { get; }
    public UIController UIController { get; }
    public Action<PlayerInstance> Callback { get; }

    public UILoadedEvent(UIController uiController, bool isLoaded, Action<PlayerInstance> callback)
    {
        UIController = uiController;
        IsLoaded = isLoaded;
        Callback = callback;
    }
}