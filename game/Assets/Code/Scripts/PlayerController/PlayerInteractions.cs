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
        
        private InputReader Input => inputProvider.Input;
        
        private IInteractable _currentInteractable;
        private OptionProvider _currentOptionProvider;

        private bool _primaryDown;


        private void Start()
        {
            Input.Primary += OnPrimary;
        }
        
        private void Update()
        {
            CheckInteractions(out _currentOptionProvider);
            _currentOptionProvider ??= defaultOptionProvider;

            selectionCircle.SetOptions(_currentOptionProvider);
            if(_primaryDown) selectionCircle.UpdateSelection(Input.LookDirection);
        }
        
        private void OnPrimary(bool down)
        {
            if(down) cameraController.enabled = false;
            if(!down && _primaryDown) cameraController.enabled = true;
            
            _primaryDown = down;
            selectionCircle.OnPrimary(down);
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