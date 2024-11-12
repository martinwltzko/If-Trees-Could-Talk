using System;
using UnityEngine;
using UnityUtils.Aiming;

namespace AdvancedController
{
    public class AimingHandler : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private PlayerStats playerStats;
        
        private Transform _previousAimingTarget;
            
        public bool IsAiming { get; private set; } //TODO: Find a better name
        public Transform AimingTarget { get; private set; }
        public Vector3 AimingPoint { get; private set; }
        public Vector3 AimingNormal { get; private set; }


        private void FixedUpdate()
        {
            Ray ray = cam.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
            IsAiming = Physics.Raycast(ray, out RaycastHit hit);
            IsAiming = hit.distance <= playerStats.aimingDistance && hit.transform!=null;
            
            AimingTarget = IsAiming ? hit.transform : null;
            AimingPoint = IsAiming ? hit.point : Vector3.zero;
            AimingNormal = IsAiming ? hit.normal : Vector3.zero;
            
            UpdateAimingTargets();
        }

        private void UpdateAimingTargets()
        {
            if(IsAiming && _previousAimingTarget != AimingTarget)
            {
                if (_previousAimingTarget!=null && _previousAimingTarget.TryGetComponent(out IAimingTarget oldAimingTarget)) {
                    oldAimingTarget.OnAimingEnd();
                }
                if (AimingTarget?.TryGetComponent(out IAimingTarget newAimingTarget) ?? false) {
                    newAimingTarget.OnAimingStart();
                }
                _previousAimingTarget = AimingTarget;
            }
            else if (!IsAiming && _previousAimingTarget != null)
            {
                if (_previousAimingTarget.TryGetComponent(out IAimingTarget oldAimingTarget)) {
                    oldAimingTarget.OnAimingEnd();
                }
                _previousAimingTarget = null;
            }
        }

        private void OnDrawGizmos()
        {
            if(IsAiming)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(AimingPoint, 0.1f);
                Gizmos.DrawRay(AimingPoint, AimingNormal*.25f);
            }
        }
    }
}