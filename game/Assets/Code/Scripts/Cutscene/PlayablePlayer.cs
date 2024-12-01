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
    private OptionProvider _previousOptionProvider;
    private PlayableDirector _director;
    
    
    [Header("Debug")]
    [SerializeField, ReadOnly] private bool _cancelled;
    [SerializeField, ReadOnly] private float _time;

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
    }
    
    private void Update()
    {
        _time = (float)_director.time;
        if (_time < loopStart || _cancelled) return;
        if (_time > loopEnd) _director.time = loopStart;
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
