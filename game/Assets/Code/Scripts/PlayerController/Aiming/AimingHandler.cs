using System;
using UnityEngine;
using UnityUtils.Aiming;

namespace AdvancedController
{
    public class AimingHandler : MonoBehaviour
    {
        [SerializeField] private PlayerInstance player;

        private Transform PlayerTransform => player.PlayerController.transform;
        private PlayerStats Stats => player.PlayerStats;
        private Camera Cam => player.CameraController.Camera;

        public bool IsAiming { get; private set; } //TODO: Find a better name
        public Vector3 AimingPoint { get; private set; }
        public Vector3 AimingNormal { get; private set; }
        public Transform AimingTransform { get; private set; }
        
        private IAimingTarget _aimingTarget;
        private IAimingTarget _previousAimingTarget;


        private void FixedUpdate()
        {
            Ray ray = Cam.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
            IsAiming = Physics.Raycast(ray, out RaycastHit hit, Stats.aimingDistance, Stats.aimingLayerMask);
            IsAiming = Vector3.Distance(hit.point, PlayerTransform.position) <= Stats.aimingDistance && hit.transform!=null;
            
            AimingTransform = IsAiming ? hit.transform : null;
            _aimingTarget = AimingTransform?.GetComponent<IAimingTarget>();
            AimingPoint = IsAiming ? hit.point : Vector3.zero;
            AimingNormal = IsAiming ? hit.normal : Vector3.zero;
            
            UpdateAimingTargets();
        }

        private void UpdateAimingTargets()
        {
            if(IsAiming && _previousAimingTarget != _aimingTarget)
            {
                if (_previousAimingTarget!=null) {
                    _previousAimingTarget.OnAimingEnd(player);
                }
                if (_aimingTarget != null) {
                    _aimingTarget.OnAimingStart(player);
                }
                _previousAimingTarget = _aimingTarget;
            }
            else if (!IsAiming && _previousAimingTarget != null)
            {
                _previousAimingTarget.OnAimingEnd(player);
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