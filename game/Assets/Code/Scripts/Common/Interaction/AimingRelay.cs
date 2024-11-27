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
            onAimingStart.Invoke();
        }

        public void OnAimingEnd(object sender)
        {
            onAimingEnd.Invoke();
        }
    }
}