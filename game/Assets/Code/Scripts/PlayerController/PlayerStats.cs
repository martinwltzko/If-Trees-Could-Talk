using UnityEngine;

namespace AdvancedController
{
    [CreateAssetMenu(menuName = "Player/Player Stats")]
    public class PlayerStats : ScriptableObject
    {
        public float walkingSpeed = 2f;
        public float runningSpeed = 5f;
        public float acceleration = 100;
        public float airControlRate = 2f;
        public float jumpSpeed = 10f;
        public float jumpDuration = 0.2f;
        public float airFriction = 0.5f;
        public float groundFriction = 100f;
        public float gravity = 30f;
        public float slideGravity = 5f;
        public float slopeLimit = 30f;
        public bool useLocalMomentum;
        
        public float aimingDistance = 10f;
        public LayerMask aimingLayerMask;
    }
}