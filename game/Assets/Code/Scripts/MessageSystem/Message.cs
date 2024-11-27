using System;
using System.Collections.Generic;
using AdvancedController;
using Code.Scripts.UI;
using EPOOutline;
using Interaction;
using UnityEngine;
using UnityUtils.Aiming;

public class Message : MonoBehaviour, IAimingTarget
{
    [SerializeField] private Outlinable outlinable;
    [SerializeField] private OptionProvider optionProvider;
    
    public string message;
    
    public Transform Transform => transform;
    public OptionProvider OptionProvider => optionProvider;
    private OptionProvider _previousOptionProvider;


    public void OpenMessage(object sender)
    {
        if (sender is not PlayerInstance playerInstance)
        {
            Debug.LogError("Sender is not a player instance");
            return;
        }

        Debug.Log("Message opened by " + playerInstance);
        playerInstance.GetUIController().OpenNote(this);
    }
    
    private void Start()
    {
        outlinable.enabled = false;
    }
    public void SetMessage(string newMessage)
    {
        message = newMessage;
    }
    
    public void OnAimingStart(object sender)
    {
        if(sender is not PlayerInstance playerInstance) return;
        playerInstance.PlayerInteractions.SetOptionProvider(optionProvider);
        outlinable.enabled = true;
    }

    public void OnAimingEnd(object sender)
    {
        if(sender is not PlayerInstance playerInstance) return;
        playerInstance.PlayerInteractions.ClearCurrentOptionProvider();
        outlinable.enabled = false;
    }
}