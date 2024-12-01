using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Global/Events/Game Event")]
public class GameEvent : ScriptableObject
{
    private readonly List<IGameEventListener> _eventListeners = 
        new List<IGameEventListener>();
    
    public virtual void Raise()
    {
        Debug.Log("Event raised with listeners: " + _eventListeners.Count);
        for (int i = _eventListeners.Count - 1; i >= 0; i--)
        {
            _eventListeners[i].OnEventRaised();
        }
    }
    
    public void RegisterListener(IGameEventListener listener)
    {
        if (!_eventListeners.Contains(listener))
        {
            _eventListeners.Add(listener);
        }
    }
    
    public void UnregisterListener(IGameEventListener listener)
    {
        if (_eventListeners.Contains(listener))
        {
            _eventListeners.Remove(listener);
        }
    }
    
}
