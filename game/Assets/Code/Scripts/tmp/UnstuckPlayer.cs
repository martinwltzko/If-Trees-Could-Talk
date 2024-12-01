using System;
using EventHandling;
using UnityEngine;

public class UnstuckPlayer : MonoBehaviour
{
    [SerializeField] private PlayerInstance player;
    [SerializeField] private Transform unstuckTransform;
    
    private EventBinding<PlayerLoadedEvent> _playerLoadedEventBinding;


    private void Awake()
    {
        _playerLoadedEventBinding = new EventBinding<PlayerLoadedEvent>((e) => {
            if(e.Loaded) player = e.PlayerInstance;
        });
        EventBus<PlayerLoadedEvent>.Register(_playerLoadedEventBinding);
    }

    private void OnDestroy()
    {
        EventBus<PlayerLoadedEvent>.Unregister(_playerLoadedEventBinding);
    }

    public void Unstuck()
    {
        if(player == null)
        {
            Debug.LogError("PlayerInstance is null. Cant unstuck player!");
            return;
        }
        
        player.PlayerController.transform.position = unstuckTransform.position;
    }
}
