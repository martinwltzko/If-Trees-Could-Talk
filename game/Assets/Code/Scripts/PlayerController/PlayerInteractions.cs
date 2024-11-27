using System;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.UI;
using Cysharp.Threading.Tasks;
using Interaction;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

namespace AdvancedController
{
    public class PlayerInteractions : MonoBehaviour
    {
        [SerializeField] private PlayerInstance player;
        
        private PlayerStats PlayerStats => player.PlayerStats;
        private CameraController CameraController => player.CameraController;
        private Transform PlayerTransform => player.PlayerController.transform;
        
        private readonly LinkedList<OptionProvider> _optionProviders = new();
        
        [SerializeField] private OptionProvider defaultOptionProvider;

        private UIController _uiController;
        private IInteractable _currentInteractable;
        private OptionProvider _currentOptionProvider;

        private bool _primaryDown;
        private bool _initialized;
        private bool _cameraEnabled;
        

        private void OnEnable()
        {
            player.InputReader.Primary += OnPrimary;
            player.OnUiLoaded += OnUiLoaded;
        }

        private void OnDisable()
        {
            player.InputReader.Primary -= OnPrimary;
            player.OnUiLoaded -= OnUiLoaded;
        }
        
        private void OnPrimary(bool down)
        {
            if (!enabled) return;
            HandleCameraActivation(down);
        }
        
        private void OnOptionPressed([NotNull] OptionProvider.Option option) {
            Debug.Log("Option pressed: " + option);
            option.Interact.Invoke(player);
        }
        
        private void OnUiLoaded(UIController uiController)
        {
            _uiController = uiController;
            _uiController.OnOptionPressed += OnOptionPressed;
            SetOptionProvider(defaultOptionProvider);
            _initialized = true;
        }
        
        public void SetOptionProvider(OptionProvider optionProvider)
        {
            if(optionProvider == null) return;
            
            _optionProviders.AddLast(optionProvider);
            _currentOptionProvider = optionProvider;
            _uiController.SetOptionProvider(optionProvider);
        }
        
        public void ClearCurrentOptionProvider()
        {
            if(_optionProviders.Count > 0) _optionProviders.RemoveLast();
            _currentOptionProvider = _optionProviders.Count > 0 ? _optionProviders.Last.Value : defaultOptionProvider;
            
            if(!TryGetSelectionCircle(out var selectionCircle)) return;
            selectionCircle.SetOptions(_currentOptionProvider);
        }
        
        
        
        private bool TryGetSelectionCircle(out SelectionCircle selectionCircle)
        {
            selectionCircle = _uiController?.SelectionCircle;
            return selectionCircle != null;
        }
        
        //TODO: Implement this in a separate class, single responsibility principle
        private void HandleCameraActivation(bool down)
        {
            if (down) {
                _cameraEnabled = CameraController.enabled;
                CameraController.enabled = false;
            }

            if(!down && _primaryDown) CameraController.enabled = _cameraEnabled;
            _primaryDown = down;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(PlayerTransform.position, PlayerStats.aimingDistance);
        }
    }
}