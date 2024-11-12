using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interaction
{
    public class InteractionSeeker : MonoBehaviour
    {
        public List<IInteractable> InteractionList => _interactionList;
        private List<IInteractable> _interactionList = new List<IInteractable>();
        public IInteractable CurrentInteraction => _currentInteraction;
        private IInteractable _currentInteraction;

        [SerializeField] private Transform _player;
        [SerializeField] private bool _showInteractions = true;

        [Header("Interaction Settings..")] 
        [SerializeField] private float _interactionRange;
        [SerializeField] private float _interactionDeactivationRange;
        [SerializeField] private LayerMask _interactLayer;

        private void Update()
        {
            HandleInteractions();
        }

        private void HandleInteractions()
        {
            var potentialInteractable = SearchInteractable();
            if (potentialInteractable != null)
            {
                var allowedInteraction = AllowInteraction(potentialInteractable);

                if (allowedInteraction)
                {
                    _currentInteraction = potentialInteractable;
                    return;
                }
                _currentInteraction = null;
            }
        }

        private IInteractable SearchInteractable()
        {
            foreach (var interactable in new List<IInteractable>(_interactionList))
            {
                if (Vector3.Distance(_player.position, interactable.Transform.position) > 
                    _interactionRange+_interactionDeactivationRange || !interactable.Transform.gameObject.activeInHierarchy)
                {
                    interactable.Release(_player);
                    _interactionList.Remove(interactable);
                }
            }
        
            Collider[] interactableCollider = Physics.OverlapSphere(_player.position, _interactionRange, _interactLayer);

            if (interactableCollider.Length <= 0) return null;

            foreach (var coll in interactableCollider)
            {
                if(!coll.TryGetComponent(out IInteractable interactable)) continue;
                if(Vector3.Distance(coll.bounds.center, _player.position) > _interactionRange) continue;

                if (!_interactionList.Contains(interactable))
                {
                    interactable.Focus(_player);
                    _interactionList.Add(interactable);
                }
            }
 
            var selected = _interactionList.OrderBy(SortingCondition).FirstOrDefault();
            return selected;
        }
    
        private float SortingCondition(IInteractable interactable)
        {
            return Vector3.Distance(interactable.Transform.position, _player.position);
        }

        private bool AllowInteraction(IInteractable interactable)
        {
            if (!interactable.Transform.gameObject.activeInHierarchy) return false;
            var distanceToPlayer = Vector3.Distance(interactable.Transform.position, _player.position);
            return distanceToPlayer < _interactionRange;
        }
    
        private List<IInteractable> GetAllInteractions()
        {
            return _interactionList;
        }
    
        private void OnDrawGizmos()
        {
            if(_showInteractions) InteractableGizmos();
        }
    
        private void InteractableGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(_player.position, _interactionRange);
        
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_player.position, _interactionRange+_interactionDeactivationRange);
        
            var interactionList = GetAllInteractions();
            foreach (var interactable in interactionList)
            {
                Gizmos.color = Color.red;
                Bounds interactableBounds = interactable.Transform.GetComponent<Collider>().bounds;
            
                var interactableDistance = Vector2.Distance(interactable.Transform.position, _player.position);
            
                if(interactableDistance < _interactionRange)
                    Gizmos.color = Color.green;
                else if (interactableDistance < _interactionRange+_interactionDeactivationRange)
                    Gizmos.color = Color.yellow;
    
                Gizmos.DrawLine(interactable.Transform.position, _player.position);
                Gizmos.DrawWireSphere(interactable.Transform.position, .15f);
                Gizmos.DrawWireCube(interactableBounds.center, interactableBounds.size);
            }
        }
    }
}
