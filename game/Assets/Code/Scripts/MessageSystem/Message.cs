using System;
using System.Collections.Generic;
using Code.Scripts.UI;
using EPOOutline;
using Interaction;
using UnityEngine;
using UnityUtils.Aiming;

public class Message : MonoBehaviour, IAimingTarget, IInteractable
{
    [SerializeField] private Outlinable outlinable;
    
    public string message;
    private bool _aimingAt, _inRange;
    
    public Transform Transform => transform;

    
    private void Start()
    {
        outlinable.enabled = false;
    }

    public void Interact(object sender)
    {
        PrintMessage();
    }
    public void SetMessage(string newMessage)
    {
        message = newMessage;
    }
    public void PrintMessage()
    {
        Debug.Log(message);
    }
    
    public void OnAimingStart()
    {
        _aimingAt = true;
        outlinable.enabled = _aimingAt && _inRange;
    }

    public void OnAimingEnd()
    {
        _aimingAt = false;
        outlinable.enabled = _aimingAt && _inRange;
    }

    public void Focus(object sender)
    {
        _inRange = true;
        outlinable.enabled = _aimingAt && _inRange;
    }

    public void Release(object sender)
    {
        _inRange = false;
        outlinable.enabled = _aimingAt && _inRange;
    }
}