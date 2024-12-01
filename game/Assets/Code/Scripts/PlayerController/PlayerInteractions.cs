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
        

        private void Awake() {
            player.OnUiLoaded += OnUiLoaded;
        }

        private void OnDestroy() {
            player.OnUiLoaded -= OnUiLoaded;
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
            
            Debug.Log("<color=green>PlayerInteractions initialized</color>");
        }
        
        public void SetOptionProvider(OptionProvider optionProvider)
        {
            if(optionProvider == null) return;
            
            Debug.Log("2. ==== Setting interaction options (" + optionProvider.name + ") ====");
            
            _optionProviders.AddLast(optionProvider);
            _currentOptionProvider = optionProvider;
            _uiController?.SetOptionProvider(optionProvider);
        }
        
        public void ClearCurrentOptionProvider()
        {
            if(_optionProviders.Count > 0) _optionProviders.RemoveLast();
            _currentOptionProvider = _optionProviders.Count > 0 ? _optionProviders.Last.Value : defaultOptionProvider;
            
            Debug.Log(" ==== Clearing interaction options ====");
            
            if(!TryGetSelectionCircle(out var selectionCircle)) return;
            selectionCircle.SetOptions(_currentOptionProvider);
        }
        
        private bool TryGetSelectionCircle(out SelectionCircle selectionCircle)
        {
            selectionCircle = _uiController?.SelectionCircle;
            return selectionCircle != null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(PlayerTransform.position, PlayerStats.aimingDistance);
        }
    }
}