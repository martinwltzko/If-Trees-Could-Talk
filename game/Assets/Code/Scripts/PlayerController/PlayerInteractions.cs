using System;
using System.Linq;
using Code.Scripts.UI;
using Interaction;
using UnityEngine;

namespace AdvancedController
{
    public class PlayerInteractions : MonoBehaviour
    {
        [SerializeField] private InputProvider inputProvider;
        [SerializeField] private InteractionSeeker interactionSeeker;
        [SerializeField] private AimingHandler aimingHandler;
        [SerializeField] private SelectionCircle selectionCircle;
        [SerializeField] private CameraController cameraController;
        
        [SerializeField] private OptionProvider defaultOptionProvider;
        
        //TODO: This is a bit of a hack, should be refactored probably
        public NoteDisplay NoteDisplay => noteDisplay;
        [SerializeField] private NoteDisplay noteDisplay;
        
        
        private InputReader Input => inputProvider.Input;
        
        private IInteractable _currentInteractable;
        private OptionProvider _currentOptionProvider;

        private bool _primaryDown;
        private bool _optionOverride;


        private void Start()
        {
            Input.Primary += OnPrimary;
        }
        
        public void OverrideOptionProvider(OptionProvider optionProvider, out OptionProvider previousOptionProvider)
        {
            if (optionProvider == defaultOptionProvider) {
                previousOptionProvider = null; //TODO: Not sure if this will cause problems further down the line
                _currentOptionProvider = optionProvider;
                Debug.Log("Possible error: Setting previous option provider to null");
                _optionOverride = false;
                return;
            }
            
            previousOptionProvider = _currentOptionProvider;
            _currentOptionProvider = optionProvider;
            _optionOverride = true;
        }
        public void ClearOptionProviderOverride()
        {
            _optionOverride = false;
        }
        
        private void Update()
        {
            CheckInteractions(out var optionProvider);
            _currentOptionProvider = _optionOverride ? _currentOptionProvider : optionProvider;
            _currentOptionProvider ??= defaultOptionProvider;

            selectionCircle.SetOptions(_currentOptionProvider);
            if(_primaryDown) selectionCircle.UpdateSelection(Input.LookDirection);
        }
        
        private bool _cameraEnabled;
        private void OnPrimary(bool down)
        {
            if (!enabled) return;
            
            if (down) {
                _cameraEnabled = cameraController.enabled;
                cameraController.enabled = false;
            }
            if(!down && _primaryDown) cameraController.enabled = _cameraEnabled;
            
            _primaryDown = down;
            selectionCircle.OnPrimary(down, out var option);
            if (option != null) option.Interact.Invoke(this);
        }
        
        private bool CheckInteractions(out OptionProvider optionProvider)
        {
            if (!aimingHandler.IsAiming) {
                _currentInteractable = null;
                optionProvider = null;
                return false;
            }
            
            //TODO: This is not elegant, I juts wanted top reuse my interaction code for now
            // Should also be no overhead as the list is very small
            _currentInteractable = interactionSeeker.InteractionList
                .FirstOrDefault(interactable => interactable.Transform == aimingHandler.AimingTarget);
            optionProvider = _currentInteractable?.Transform.GetComponent<OptionProvider>();
            
            return _currentInteractable != null;
        }
    }
}