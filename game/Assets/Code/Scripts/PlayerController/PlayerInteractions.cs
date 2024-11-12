using System;
using System.Linq;
using Interaction;
using UnityEngine;

namespace AdvancedController
{
    public class PlayerInteractions : MonoBehaviour
    {
        [SerializeField] private InputProvider inputProvider;
        [SerializeField] private InteractionSeeker interactionSeeker;
        [SerializeField] private AimingHandler aimingHandler;
        
        private InputReader Input => inputProvider.Input;

        private bool CanInteract(out IInteractable interactable)
        {
            if (!aimingHandler.IsAiming)
            {
                interactable = null;
                return false;
            }
            
            interactable = interactionSeeker.InteractionList
                .FirstOrDefault(interactable => interactable.Transform == aimingHandler.AimingTarget);
            
            return interactable != null;
        }
        
        private void Start()
        {
            Input.Secondary += OnSecondary;
        }

        private void OnSecondary(bool down)
        {
            if (!down) return;
            
            if (!CanInteract(out IInteractable interactable)) return;
            interactable.Interact(this);
        }
    }
}