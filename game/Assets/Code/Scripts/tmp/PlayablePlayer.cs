using System;
using AdvancedController;
using Code.Scripts.UI;
using EPOOutline;
using Interaction;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityUtils.Aiming;

public class PlayablePlayer : MonoBehaviour, IAimingTarget, IInteractable
{
    [SerializeField] private TimelineAsset timeline;
    [SerializeField] private PlayableDirector director;
    [SerializeField] private Outlinable outlinable;
    [SerializeField] private PlayerInteractions playerInteractions; //TODO: Implement a robust way to handle this
    [SerializeField] private OptionProvider optionProvider;
    private OptionProvider _previousOptionProvider;

    [SerializeField] private float loopStart;
    [SerializeField] private float loopEnd;
    public Transform Transform => transform;
    
    private bool _isAiming, _inRange;
    public bool _cancelled;
    public float _time;

    private void Start()
    {
        outlinable.enabled = false;
    }

    public void OnAimingStart()
    {
        _isAiming = true;
        outlinable.enabled = _isAiming && _inRange;
    }

    public void OnAimingEnd()
    {
        _isAiming = false;
        outlinable.enabled = _isAiming && _inRange;
    }

    public void Focus(object sender)
    {
        _inRange = true;
        outlinable.enabled = _isAiming && _inRange;
    }

    public void Release(object sender)
    {
        _inRange = false;
        outlinable.enabled = _isAiming && _inRange;
    }
    
    public void Interact(object sender)
    {
        PlayScene();
    }
    
    public void PlayScene()
    {
        director.playableAsset = timeline;
        director.Play();
        _cancelled = false;
        playerInteractions.OverrideOptionProvider(optionProvider, out _previousOptionProvider);
    }

    //TODO: Implement a robust way to handle this
    public void CancelScene()
    {
        _cancelled = true;
        director.time = loopEnd;
        playerInteractions.OverrideOptionProvider(_previousOptionProvider, out _);
    }

    private void Update()
    {
        _time = (float)director.time;
        if (_time < loopStart || _cancelled) return;
        if (_time > loopEnd) director.time = loopStart;
    }
}
