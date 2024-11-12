using UnityEngine;

namespace AdvancedController
{
    /// <summary>
    /// Component to track an object's displacement and rotation during an update
    /// </summary>
    public class TransferMomentum : MonoBehaviour, IMovingGround
    {
        /// <summary>
        /// How much of the objects velocity should a player retain when leaving the surface of the object via jump or
        /// fall.
        /// </summary>
        [SerializeField]
        [Range(0, 1)]
        internal float transferMomentumWeight = 1.0f;

        [SerializeField]
        internal bool shouldAttach = false;

        /// <inheritdoc/>
        public bool ShouldAttach() => shouldAttach;

        /// <summary>
        /// Previously measured position of an object (previous frame)
        /// </summary>
        public Vector3 PreviousPosition { get; private set; }

        /// <summary>
        /// Previously measured attitude of an object (previous frame)
        /// </summary>
        public Quaternion PreviousAttitude { get; private set; }

        /// <summary>
        /// Finds the change in attitude (expressed as a quaternion) between
        /// the current and previous update. QFinal * Inv(QInitial)
        /// </summary>
        public Quaternion ChangeAttitude { get; private set; }

        /// <summary>
        /// Displacement between current and previous update
        /// </summary>
        public Vector3 Displacement { get; private set; }

        public void FixedUpdate()
        {
            ChangeAttitude = transform.rotation * Quaternion.Inverse(PreviousAttitude);
            Displacement = transform.position - PreviousPosition;

            // Update state
            PreviousPosition = transform.position;
            PreviousAttitude = transform.rotation;
        }
        
        public virtual Vector3 GetVelocityAtPoint(Vector3 point)
        {
            return GetDisplacementAtPoint(point) / Time.fixedDeltaTime;
        }
        
        public virtual Vector3 GetDisplacementAtPoint(Vector3 point)
        {
            // Get relative position to previous start
            Vector3 relativePosition = point - PreviousPosition;
            // Rotate point around center by change in attitude
            Vector3 rotatedFinalPosition = ChangeAttitude * relativePosition;
            // Get the delta due to rotation
            Vector3 deltaRotation = rotatedFinalPosition - relativePosition;
            // Shift point by total displacement
            return deltaRotation + Displacement;
        }

        /// <inheritdoc/>
        public virtual float GetMovementWeight(Vector3 point, Vector3 playerVelocity)
        {
            return 1.0f;
        }

        /// <inheritdoc/>
        public virtual float GetTransferMomentumWeight(Vector3 point, Vector3 playerVelocity)
        {
            return transferMomentumWeight;
        }
    }
}
