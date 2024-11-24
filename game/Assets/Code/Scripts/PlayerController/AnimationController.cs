using UnityEngine;

namespace AdvancedController {
    [RequireComponent(typeof(PlayerController))]
    public class AnimationController : MonoBehaviour {
        PlayerController controller;

        readonly int speedHash = Animator.StringToHash("Velocity");
        readonly int isJumpingHash = Animator.StringToHash("IsJumping");
        
        readonly int groundedState = Animator.StringToHash("Grounded");
        readonly int jumpingState = Animator.StringToHash("Jumping");

        private int _currentAnimationHash;
        private float _currentVelocity;

        [SerializeField] private Animator animator;
        [SerializeField] private float smoothing = 10f;
        
        void Start() {
            controller = GetComponent<PlayerController>();
            
            controller.OnJump += HandleJump;
            controller.OnLand += HandleLand;
        }

        void Update() 
        {
            _currentVelocity = Mathf.Lerp(_currentVelocity, controller.GetVelocity().magnitude, Time.deltaTime * smoothing); //TODO: Add proper smoothing
            animator.SetFloat(speedHash, _currentVelocity);
        }
        
        private void PlayAnimation(int hash, float crossfadeTime = 0f)
        {
            if(_currentAnimationHash == hash) return;
            _currentAnimationHash = hash;
            
            animator.CrossFade(hash, crossfadeTime);
        }

        void HandleJump(Vector3 momentum)
        {
            PlayAnimation(jumpingState);
        }

        void HandleLand(Vector3 momentum)
        {
            PlayAnimation(groundedState);
        }
    }
}