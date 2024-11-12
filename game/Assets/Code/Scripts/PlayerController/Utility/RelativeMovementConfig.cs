using UnityEngine;

namespace AdvancedController
{
    /// <summary>
    /// Relative parent configuration for saving
    /// the position of an object relative to a given parent object.
    /// </summary>
    public class RelativeMovementConfig
    {
        /// <summary>
        /// Relative rotation in local space.
        /// TODO: Adjust this configuration to work as expected.
        /// </summary>
        public Quaternion RelativeRotation { get; internal set; }
        public Vector3 RelativePosition { get; internal set; }

        /// <summary>
        /// Previous parent for saving relative transform position.
        /// </summary>
        public Transform PreviousParent { get; internal set; }

        /// <summary>
        /// Check if the player is standing on moving ground.
        /// </summary>
        public bool OnMovingGround => PreviousParent != null;

        /// <summary>
        /// Reset the relative parent transform.
        /// </summary>
        public void Reset()
        {
            RelativeRotation = Quaternion.identity;
            PreviousParent = null;
        }
        
        Vector3 lastVelocity;
        
        public virtual Vector3 UpdateMovingGround(Vector3 position, Quaternion rotation, Transform parent, Vector3 delta)
        {
            if (parent != null)
            {
                IMovingGround ground = parent.GetComponent<IMovingGround>();

                if (ground == null || ground.ShouldAttach())
                {
                    RelativeRotation = rotation * Quaternion.Inverse(parent.rotation);

                    if (parent != PreviousParent)
                    {
                        RelativePosition = position - parent.position;
                        RelativePosition = Quaternion.Inverse(parent.rotation) * RelativePosition;
                    }
                    else
                    {
                        RelativePosition += Quaternion.Inverse(parent.rotation) * delta; //TODO: Add delta
                    }
                    
                    PreviousParent = parent;
                    lastVelocity = Vector3.zero;
                }
                else
                {
                    PreviousParent = null;
                    lastVelocity = ground.GetVelocityAtPoint(position);
                }
            }
            else {
                Reset();
            }
            
            
            return lastVelocity;
        }
        
        public virtual Vector3 GetGroundVelocity(Vector3 position)
        {
            if(PreviousParent == null) return Vector3.zero;
            return GetGroundDelta(position) / Time.fixedDeltaTime;
        }
        
        public virtual Vector3 GetGroundDelta(Vector3 position)
        {
            if(PreviousParent == null) return Vector3.zero;
            return PreviousParent.position + PreviousParent.rotation * RelativePosition - position;
        }

        public void DebugGizmos()
        {
            if(PreviousParent == null) return;
            Gizmos.color = Color.red;
            var pos = PreviousParent.position + PreviousParent.rotation * RelativePosition;
            Gizmos.DrawSphere(pos, .25f);
        }
    }
}
