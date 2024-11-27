using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class UnityGameEventBroadcaster : MonoBehaviour
{
    //[InfoBox("Game Event to listen to..")]
    [Tooltip("Event to fire.")]
    [SerializeField] private List<EventBroadcast> events;

    private void Start()
    {
        foreach (var e in events)
            e.enabled = true;

    }

    private void OnDisable()
    {
        foreach (var e in events)
            e.enabled = false;
    }


    [Serializable]
    public class EventBroadcast
    {
        [SerializeField] GameEvent m_event;
        [HideInInspector] public bool enabled;

        [EnableIf("enabled"), Button("Raise", ButtonSizes.Gigantic)]
        public void RaiseEvent()
        {
            m_event.Raise();
        }
    }
}