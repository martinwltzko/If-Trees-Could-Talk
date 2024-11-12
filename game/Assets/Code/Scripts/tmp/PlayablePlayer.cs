using System;
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
    public Transform Transform => transform;
    
    private bool _isAiming, _inRange;

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
    }
}
