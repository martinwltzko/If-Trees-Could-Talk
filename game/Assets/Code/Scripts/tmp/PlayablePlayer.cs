using System;
using System.Collections.Generic;
using AdvancedController;
using Code.Scripts.UI;
using EPOOutline;
using EventHandling;
using Interaction;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityUtils.Aiming;

[RequireComponent(typeof(PlayableDirector))]
public class PlayablePlayer : MonoBehaviour
{
    //TODO: Probably put this into a separate timelineSettings class with conditions
    [Header("Timeline Settings")]
    [SerializeField] private float loopStart;
    [SerializeField] private float loopEnd;

    [Header("Interaction..")]
    [SerializeField] private PlayerInteractions playerInteractions; //TODO: Implement a robust way to handle this
    private OptionProvider _previousOptionProvider;

    private PlayableDirector _director;

    private EventBinding<PlayerLoadedEvent> _playerLoadedEventBinding;
    
    
    [Header("Debug")]
    [SerializeField, ReadOnly] private bool _cancelled;
    [SerializeField, ReadOnly] private float _time;

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
        _playerLoadedEventBinding = new EventBinding<PlayerLoadedEvent>((e) =>
        {
            if (e.Loaded) {
                playerInteractions = e.PlayerInstance.PlayerInteractions;
            }
        });
        EventBus<PlayerLoadedEvent>.Register(_playerLoadedEventBinding);
    }
    
    private void OnDestroy()
    {
        EventBus<PlayerLoadedEvent>.Unregister(_playerLoadedEventBinding);
    }
    
    private void Update()
    {
        _time = (float)_director.time;
        if (_time < loopStart || _cancelled) return;
        if (_time > loopEnd) _director.time = loopStart;
    }
    
    public void UpdateInteractionOptions(OptionProvider options)
    {
        playerInteractions.SetOptionProvider(options);
    }

    public void ClearInteractionOptions()
    {
        playerInteractions.ClearCurrentOptionProvider();
    }
    
    public void PlayScene()
    {
        _director.Play();
        _cancelled = false;
    }
    
    public void CancelScene()
    {
        _cancelled = true;
        _director.time = loopEnd;
    }

    
}
