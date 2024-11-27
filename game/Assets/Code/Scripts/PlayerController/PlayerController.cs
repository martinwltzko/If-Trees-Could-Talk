using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtils;
using UnityUtils.StateMachine;
using ImprovedTimers;

namespace AdvancedController {
    [RequireComponent(typeof(PlayerMover))]
    public class PlayerController : MonoBehaviour {
        #region Fields
        [SerializeField] private PlayerInstance player;
        private InputReader Input => player.InputReader;
        private PlayerStats Stats => player.PlayerStats;
        private Transform CameraTransform => player.CameraController.Camera.transform;

        private Transform _tr;
        private PlayerMover _mover;
        private CeilingDetector _ceilingDetector;

        private bool _jumpKeyIsPressed;    // Tracks whether the jump key is currently being held down by the player
        private bool _jumpKeyWasPressed;   // Indicates if the jump key was pressed since the last reset, used to detect jump initiation
        private bool _jumpKeyWasLetGo;     // Indicates if the jump key was released since it was last pressed, used to detect when to stop jumping
        private bool _jumpInputIsLocked;   // Prevents jump initiation when true, used to ensure only one jump action per press
        
        private StateMachine _stateMachine;
        private CountdownTimer _jumpTimer;
        
        private Vector3 _momentum, _savedMovementVelocity;
        
        public event Action<Vector3> OnJump = delegate { };
        public event Action<Vector3> OnLand = delegate { };
        #endregion
        
        bool IsGrounded() => _stateMachine.CurrentState is GroundedState or SlidingState;
        public float GetMovementSpeed() => Input.IsRunningKeyPressed() ? Stats.runningSpeed : Stats.walkingSpeed;
        public Vector3 GetMomentum() => Stats.useLocalMomentum ? _tr.localToWorldMatrix * _momentum : _momentum;
        public Vector3 GetVelocity() => _savedMovementVelocity;

        private void Awake() {
            _tr = transform;
            _mover = GetComponent<PlayerMover>();
            _ceilingDetector = GetComponent<CeilingDetector>();
            
            _jumpTimer = new CountdownTimer(Stats.jumpDuration);
            SetupStateMachine();
        }

        private void Start()
        {
            Input.Jump += HandleJumpKeyInput;
        }

        private void HandleJumpKeyInput(bool isButtonPressed) {
            if (!_jumpKeyIsPressed && isButtonPressed) {
                _jumpKeyWasPressed = true;
            }

            if (_jumpKeyIsPressed && !isButtonPressed) {
                _jumpKeyWasLetGo = true;
                _jumpInputIsLocked = false;
            }
            
            _jumpKeyIsPressed = isButtonPressed;
        }

        private void SetupStateMachine() {
            _stateMachine = new StateMachine();
            
            var grounded = new GroundedState(this);
            var falling = new FallingState(this);
            var sliding = new SlidingState(this);
            var rising = new RisingState(this);
            var jumping = new JumpingState(this);
            
            At(grounded, rising, () => IsRising());
            At(grounded, sliding, () => _mover.IsGrounded() && IsGroundTooSteep());
            At(grounded, falling, () => !_mover.IsGrounded());
            At(grounded, jumping, () => (_jumpKeyIsPressed || _jumpKeyWasPressed) && !_jumpInputIsLocked);
            
            At(falling, rising, () => IsRising());
            At(falling, grounded, () => _mover.IsGrounded() && !IsGroundTooSteep());
            At(falling, sliding, () => _mover.IsGrounded() && IsGroundTooSteep());
            
            At(sliding, rising, () => IsRising());
            At(sliding, falling, () => !_mover.IsGrounded());
            At(sliding, grounded, () => _mover.IsGrounded() && !IsGroundTooSteep());
            
            At(rising, grounded, () => _mover.IsGrounded() && !IsGroundTooSteep());
            At(rising, sliding, () => _mover.IsGrounded() && IsGroundTooSteep());
            At(rising, falling, () => IsFalling());
            At(rising, falling, () => _ceilingDetector != null && _ceilingDetector.HitCeiling());
            
            At(jumping, rising, () => _jumpTimer.IsFinished || _jumpKeyWasLetGo);
            At(jumping, falling, () => _ceilingDetector != null && _ceilingDetector.HitCeiling());
            
            _stateMachine.SetState(falling);
        }
        
        private void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        private void Any<T>(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition(to, condition);
        
        private bool IsRising() => VectorMath.GetDotProduct(GetMomentum(), _tr.up) > 0f;
        private bool IsFalling() => VectorMath.GetDotProduct(GetMomentum(), _tr.up) < 0f;
        private bool IsGroundTooSteep() => !_mover.IsGrounded() || Vector3.Angle(_mover.GetGroundNormal(), _tr.up) > Stats.slopeLimit;
        
        private void Update() => _stateMachine.Update();

        private void FixedUpdate() {
            _stateMachine.FixedUpdate();
            _mover.CheckForGround();
            HandleMomentum();
            Vector3 velocity = _stateMachine.CurrentState is GroundedState ? CalculateMovementVelocity() : Vector3.zero;
            velocity += Stats.useLocalMomentum ? _tr.localToWorldMatrix * _momentum : _momentum;
            
            _mover.SetExtendSensorRange(IsGrounded());
            _mover.SetVelocity(velocity);
            
            _savedMovementVelocity = CalculateMovementVelocity();
            
            ResetJumpKeys();
            
            if (_ceilingDetector != null) _ceilingDetector.Reset();
        }

        private Vector3 CalculateMovementVelocity() {
            return Vector3.Lerp(GetVelocity(), CalculateMovementDirection() * GetMovementSpeed(), Time.fixedDeltaTime*Stats.acceleration);
        }

        private Vector3 CalculateMovementDirection() {
            Vector3 direction = CameraTransform == null 
                ? _tr.right * Input.Direction.x + _tr.forward * Input.Direction.y 
                : Vector3.ProjectOnPlane(CameraTransform.right, _tr.up).normalized * Input.Direction.x + 
                  Vector3.ProjectOnPlane(CameraTransform.forward, _tr.up).normalized * Input.Direction.y;
            
            return direction.magnitude > 1f ? direction.normalized : direction;
        }

        private void HandleMomentum() {
            if (Stats.useLocalMomentum) _momentum = _tr.localToWorldMatrix * _momentum;
            
            Vector3 verticalMomentum = VectorMath.ExtractDotVector(_momentum, _tr.up);
            Vector3 horizontalMomentum = _momentum - verticalMomentum;
            
            verticalMomentum -= _tr.up * (Stats.gravity * Time.deltaTime);
            if (_stateMachine.CurrentState is GroundedState && VectorMath.GetDotProduct(verticalMomentum, _tr.up) < 0f) {
                verticalMomentum = Vector3.zero;
            }

            if (!IsGrounded()) {
                AdjustHorizontalMomentum(ref horizontalMomentum, CalculateMovementVelocity());
            }

            if (_stateMachine.CurrentState is SlidingState) {
                HandleSliding(ref horizontalMomentum);
            }
            
            float friction = _stateMachine.CurrentState is GroundedState ? Stats.groundFriction : Stats.airFriction;
            horizontalMomentum = Vector3.MoveTowards(horizontalMomentum, Vector3.zero, friction * Time.deltaTime);
            
            _momentum = horizontalMomentum + verticalMomentum;

            if (_stateMachine.CurrentState is JumpingState) {
                HandleJumping();
            }
            
            if (_stateMachine.CurrentState is SlidingState) {
                _momentum = Vector3.ProjectOnPlane(_momentum, _mover.GetGroundNormal());
                if (VectorMath.GetDotProduct(_momentum, _tr.up) > 0f) {
                    _momentum = VectorMath.RemoveDotVector(_momentum, _tr.up);
                }
            
                Vector3 slideDirection = Vector3.ProjectOnPlane(-_tr.up, _mover.GetGroundNormal()).normalized;
                _momentum += slideDirection * (Stats.slideGravity * Time.deltaTime);
            }
            
            if (Stats.useLocalMomentum) _momentum = _tr.worldToLocalMatrix * _momentum;
        }

        private void HandleJumping() {
            _momentum = VectorMath.RemoveDotVector(_momentum, _tr.up);
            _momentum += _tr.up * Stats.jumpSpeed;
        }

        private void ResetJumpKeys() {
            _jumpKeyWasLetGo = false;
            _jumpKeyWasPressed = false;
        }

        public void OnJumpStart() {
            if (Stats.useLocalMomentum) _momentum = _tr.localToWorldMatrix * _momentum;
            
            _momentum += _tr.up * Stats.jumpSpeed;
            _jumpTimer.Start();
            _jumpInputIsLocked = true;
            OnJump.Invoke(_momentum);
            
            if (Stats.useLocalMomentum) _momentum = _tr.worldToLocalMatrix * _momentum;
        }

        public void OnGroundContactLost() {
            if (Stats.useLocalMomentum) _momentum = _tr.localToWorldMatrix * _momentum;
            
            Vector3 velocity = GetVelocity();
            if (velocity.sqrMagnitude >= 0f && _momentum.sqrMagnitude > 0f) {
                Vector3 projectedMomentum = Vector3.Project(_momentum, velocity.normalized);
                float dot = VectorMath.GetDotProduct(projectedMomentum.normalized, velocity.normalized);
                
                if (projectedMomentum.sqrMagnitude >= velocity.sqrMagnitude && dot > 0f) velocity = Vector3.zero;
                else if (dot > 0f) velocity -= projectedMomentum;
            }
            _momentum += velocity;
            
            if (Stats.useLocalMomentum) _momentum = _tr.worldToLocalMatrix * _momentum;
        }

        public void OnGroundContactRegained() {
            Vector3 collisionVelocity = Stats.useLocalMomentum ? _tr.localToWorldMatrix * _momentum : _momentum;
            OnLand.Invoke(collisionVelocity);
        }

        public void OnFallStart() {
            var currentUpMomemtum = VectorMath.ExtractDotVector(_momentum, _tr.up);
            _momentum = VectorMath.RemoveDotVector(_momentum, _tr.up);
            _momentum -= _tr.up * currentUpMomemtum.magnitude;
        }
        
        private void AdjustHorizontalMomentum(ref Vector3 horizontalMomentum, Vector3 movementVelocity) {
            if (horizontalMomentum.magnitude > GetMovementSpeed()) {
                if (VectorMath.GetDotProduct(movementVelocity, horizontalMomentum.normalized) > 0f) {
                    movementVelocity = VectorMath.RemoveDotVector(movementVelocity, horizontalMomentum.normalized);
                }
                horizontalMomentum += movementVelocity * (Time.deltaTime * Stats.airControlRate * 0.25f);
            }
            else {
                horizontalMomentum += movementVelocity * (Time.deltaTime * Stats.airControlRate);
                horizontalMomentum = Vector3.ClampMagnitude(horizontalMomentum, GetMovementSpeed());
            }
        }

        private void HandleSliding(ref Vector3 horizontalMomentum) {
            Vector3 pointDownVector = Vector3.ProjectOnPlane(_mover.GetGroundNormal(), _tr.up).normalized;
            Vector3 movementVelocity = CalculateMovementVelocity();
            movementVelocity = VectorMath.RemoveDotVector(movementVelocity, pointDownVector);
            horizontalMomentum += movementVelocity * Time.fixedDeltaTime;
        }
    }
} 