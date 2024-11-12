using UnityEngine;

namespace Interaction
{
    public interface IInteractable
    {
        public Transform Transform { get; }
        public void Interact(object sender);

        public void Focus(object sender);

        public void Release(object sender);
    }

    public enum InteractionMoment
    {
        Focus,
        Open,
        Close,
    }
}