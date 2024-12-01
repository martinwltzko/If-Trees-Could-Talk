using System;
using System.Collections.Generic;
using Code.Scripts.MessageSystem;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class MessageSystem : MonoBehaviour
{
    [SerializeField] private int messageCount;
    [SerializeField] private Message messagePrefab;
    [SerializeField] private float messageOffset;
    [SerializeField] private float messageScanDistance = .5f;
    [SerializeField] private int maxTryCount = 10;
    private int _currentTryCount;
    
    private readonly List<Message> _messages = new List<Message>();
    
    private void Awake()
    {
        WebHandler.Instance.OnGetFailed += async () => {
            Debug.LogError("Failed to get messages");
            RetryGetMessages();
        };
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        GetMessages();
        var messages = await WebHandler.Instance.GetMessages();
        SpawnMessages(messages);
    }

    private async void GetMessages()
    {
        if (_currentTryCount >= maxTryCount) {
            Debug.LogError("Max try count reached");
            return;
        }
        var messages = await WebHandler.Instance.GetMessages();
        SpawnMessages(messages);
        
        Debug.Log("Got messages after " + _currentTryCount + " tries");
    }
    
    public async void RetryGetMessages() {
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        _currentTryCount++;
        GetMessages();
    }

    [Button]
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