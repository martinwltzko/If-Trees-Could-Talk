using Code.Scripts.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityUtils.Aiming;

namespace Interaction
{
    public class AimingRelay : MonoBehaviour, IAimingTarget
    {
        [SerializeField] private UnityEvent onAimingStart;
        [SerializeField] private UnityEvent onAimingEnd;

        public Transform Transform => transform;

        public void OnAimingStart(object sender)
        {
            if(!enabled) return;
            onAimingStart.Invoke();
        }

        public void OnAimingEnd(object sender)
        {
            if(!enabled) return;
            onAimingEnd.Invoke();
        }
    }
}