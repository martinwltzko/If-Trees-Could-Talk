using System;
using Code.Scripts.UI;
using EventHandling;
using UnityEngine;

namespace AdvancedController
{
    public class InteractionOptionRelay : MonoBehaviour
    {
        private EventBinding<PlayerLoadedEvent> _playerLoadedEventBinding;
        private PlayerInstance _player;
        private EventBinding<UILoadedEvent> _uiLoadedEventBinding;
        
        private PlayerInteractions PlayerInteractions => _player?.PlayerInteractions;

        private void Awake()
        {
            _playerLoadedEventBinding = new EventBinding<PlayerLoadedEvent>((e) => {
                _player = e.Loaded ? e.PlayerInstance : null;
            });
        }

        private void OnEnable()
        {
            EventBus<PlayerLoadedEvent>.Register(_playerLoadedEventBinding);
        }
        
        private void OnDisable()
        {
            EventBus<PlayerLoadedEvent>.Unregister(_playerLoadedEventBinding);
        }
        
        public void SetInteractionOptions(OptionProvider options) {
            Debug.Log($" 1. ==== Setting interaction options ({options.name}) ====");
            PlayerInteractions?.SetOptionProvider(options);
        }
        
        public void ClearInteractionOptions() {
            Debug.Log(" ==== Clearing interaction options ====");
            PlayerInteractions?.ClearCurrentOptionProvider();
        }
    }
}