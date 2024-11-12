using UnityEngine;
using UnityEngine.Events;

namespace Interaction
{
    public class HoverableRelay : MonoBehaviour, IHoverable
    {
        [SerializeField] private UnityEvent _onHoverEnter;
        [SerializeField] private UnityEvent _onHoverExit;

        public void OnHoverEnter()
        {
            Debug.Log("Hover started!", this);
            _onHoverEnter.Invoke();
        }

        public void OnHoverExit()
        {
            Debug.Log("Hover ended!", this);
            _onHoverExit.Invoke();
        }
    }
}