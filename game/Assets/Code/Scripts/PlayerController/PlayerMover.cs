using System;
using System.Linq;
using UnityEngine;

namespace AdvancedController {
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class PlayerMover : MonoBehaviour {
        #region Fields
        [Header("Collider Settings:")]
        [Range(0f, 1f)] [SerializeField] private float stepHeightRatio = 0.1f;
        [SerializeField] private float colliderHeight = 2f;
        [SerializeField] private float colliderThickness = 1f;
        [SerializeField] private Vector3 colliderOffset = Vector3.zero;
        [SerializeField] private LayerMask collisionMask;
        
        private Rigidbody _rb;
        private Transform _tr;
        private CapsuleCollider _col;
        private RaycastSensor _sensor;
        
        private bool _isGrounded;
        private float _baseSensorRange;
        private Vector3 _currentGroundAdjustmentVelocity; // Velocity to adjust player position to maintain ground contact
        private float _currentGroundAdjustmentPenalty = 1f;
        private int _currentLayer;
        
        [Header("Sensor Settings:")]
        [SerializeField] bool isInDebugMode;
        private bool _isUsingExtendedSensorRange = true; // Use extended range for smoother ground transitions

        public RelativeMovementConfig RelativeMovementConfig { get; private set; } = new RelativeMovementConfig();
        
        public Vector3 LocalVelocity { get; private set; }
        public Vector3 RelativeVelocity { get; private set; }
        #endregion

        private void Awake() {
            Setup();
            RecalculateColliderDimensions();
        }

        private void OnValidate() {
            if (gameObject.activeInHierarchy) {
                RecalculateColliderDimensions();
            }
        }

        private void LateUpdate()
        {
            if (isInDebugMode) {
                _sensor.DrawDebug();
            }
        }

        public void CheckForGround() {
            if (_currentLayer != gameObject.layer) {
                RecalculateSensorLayerMask();
            }
            
            _currentGroundAdjustmentVelocity = Vector3.zero;
            _sensor.castLength = _isUsingExtendedSensorRange 
                ? _baseSensorRange + colliderHeight * _tr.localScale.x * stepHeightRatio
                : _baseSensorRange;
            _sensor.Cast();
            
            _isGrounded = _sensor.HasDetectedHit();
            if (!_isGrounded) return;
            
            float distance = _sensor.GetDistance();
            float upperLimit = colliderHeight * _tr.localScale.x * (1f - stepHeightRatio) * 0.5f;
            float middle = upperLimit + colliderHeight * _tr.localScale.x * stepHeightRatio;
            float distanceToGo = middle - distance;
            
            _currentGroundAdjustmentVelocity = _tr.up * (distanceToGo / Time.fixedDeltaTime);
            _currentGroundAdjustmentPenalty = 1f-Mathf.Clamp01(distanceToGo / middle); //TODO: Adjust this value to be persistent on stairs
        }

        public bool collisionFlag;

        private bool ComputeCollisionOverlap(out Vector3 delta)
        {
            var bottom = _tr.position + _col.center - Vector3.up * (_col.height / 2f - _col.radius);
            var top = _tr.position + _col.center + Vector3.up * (_col.height / 2f - _col.radius);
            var radius = _col.radius;

            delta = Vector3.zero;
            var overlaps = Physics.OverlapCapsule(bottom, top, radius).Where(c => c.transform != transform).ToArray();;
            foreach (var overlap in overlaps)
            {
                Physics.ComputePenetration(_col, _tr.position, _tr.rotation, overlap, overlap.transform.position,
                    overlap.transform.rotation, out var dir, out var depth);
                delta += dir * depth;
            }

            return delta != Vector3.zero;
        }
        
        private Vector3 AdjustVelocityForCollisions(Vector3 velocity)
        {
            var bottom = _tr.position + _col.center - Vector3.up * (_col.height / 2f - _col.radius);
            var top = _tr.position + _col.center + Vector3.up * (_col.height / 2f - _col.radius);
            var radius = _col.radius;

            var hit = Physics.CapsuleCastAll(bottom, top, radius, velocity.normalized,
                    velocity.magnitude*Time.fixedDeltaTime, collisionMask)
                .FirstOrDefault(hit => hit.transform != null && hit.transform != transform);
            
            if (hit.transform == null) return velocity;
            return Vector3.ProjectOnPlane(velocity, hit.normal);
        }
        
        public bool IsGrounded() => _isGrounded;
        public Vector3 GetGroundNormal() => _sensor.GetNormal();
        
        // NOTE: Older versions of Unity use rb.velocity instead
        public void SetVelocity(Vector3 velocity)
        {
            var overlapping = ComputeCollisionOverlap(out var overlap);
            var momentum = RelativeMovementConfig.UpdateMovingGround(_rb.position, _rb.rotation, 
                _sensor.GetTransform(),
                LocalVelocity * Time.fixedDeltaTime);
            //if(overlapping) RelativeMovementConfig.UpdateMovingGround(_rb.position, _rb.rotation, _sensor.GetTransform(), overlap);
            
            LocalVelocity = velocity * _currentGroundAdjustmentPenalty + _currentGroundAdjustmentVelocity;
            LocalVelocity = AdjustVelocityForCollisions(LocalVelocity);
            
            RelativeVelocity = RelativeMovementConfig.GetGroundVelocity(_rb.position);
            RelativeVelocity = AdjustVelocityForCollisions(RelativeVelocity);
            
            _rb.linearVelocity = LocalVelocity + RelativeVelocity + momentum;
            _rb.position += overlap;
        }
        public void SetExtendSensorRange(bool isExtended) => _isUsingExtendedSensorRange = isExtended;

        private void Setup() {
            _tr = transform;
            _rb = GetComponent<Rigidbody>();
            _col = GetComponent<CapsuleCollider>();
            
            _rb.freezeRotation = true;
            _rb.useGravity = false;
        }

        private void RecalculateColliderDimensions() {
            if (_col == null) {
                Setup();
            }
            
            _col.height = colliderHeight * (1f - stepHeightRatio);
            _col.radius = colliderThickness / 2f;
            _col.center = colliderOffset * colliderHeight + new Vector3(0f, stepHeightRatio * _col.height / 2f, 0f);

            if (_col.height / 2f < _col.radius) {
                _col.radius = _col.height / 2f;
            }
            
            RecalibrateSensor();
        }

        private void RecalibrateSensor() {
            _sensor ??= new RaycastSensor(_tr, collisionMask);
            
            _sensor.SetCastOrigin(_col.bounds.center);
            _sensor.SetCastDirection(RaycastSensor.CastDirection.Down);
            RecalculateSensorLayerMask();
            
            const float safetyDistanceFactor = 0.001f; // Small factor added to prevent clipping issues when the sensor range is calculated
            
            float length = colliderHeight * (1f - stepHeightRatio) * 0.5f + colliderHeight * stepHeightRatio;
            _baseSensorRange = length * (1f + safetyDistanceFactor) * _tr.localScale.x;
            _sensor.castLength = length * _tr.localScale.x;
        }

        private void RecalculateSensorLayerMask() {
            int objectLayer = gameObject.layer;
            int layerMask = Physics.AllLayers;

            for (int i = 0; i < 32; i++) {
                if (Physics.GetIgnoreLayerCollision(objectLayer, i)) {
                    layerMask &= ~(1 << i);
                }
            }
            
            int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
            layerMask &= ~(1 << ignoreRaycastLayer);
            
            _sensor.layermask = layerMask;
            _currentLayer = objectLayer;
        }

        private void OnDrawGizmos()
        {
            RelativeMovementConfig.DebugGizmos();
        }
    }
}