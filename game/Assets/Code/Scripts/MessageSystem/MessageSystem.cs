using System;
using System.Collections.Generic;
using Code.Scripts.MessageSystem;
using Sirenix.OdinInspector;
using UnityEngine;

public class MessageSystem : MonoBehaviour
{
    [SerializeField] private int messageCount;
    [SerializeField] private Message messagePrefab;
    [SerializeField] private float messageOffset;
    [SerializeField] private float messageScanDistance = .5f;
    
    private readonly List<Message> _messages = new List<Message>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        var messages = await WebHandler.Instance.GetMessages();
        SpawnMessages(messages);
    }
    
    public void SpawnMessages(WebMessage[] messages)
    {
        foreach (var message in _messages)
        {
            if(Application.isEditor)
                DestroyImmediate(message.gameObject);
            else
                Destroy(message.gameObject);
        }
        
        _messages.Clear();
        for (int i = 0; i < messages.Length; i++)
        {
            var message = messages[i].message;
            var position = messages[i].position;
            var normal = messages[i].normal;
            
            var messageInstance = Instantiate(messagePrefab, position, Quaternion.LookRotation(normal), transform);
            messageInstance.SetMessage(message);
            _messages.Add(messageInstance);
        }
    }
}