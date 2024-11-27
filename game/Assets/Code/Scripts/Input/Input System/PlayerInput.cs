using System;
using System.ComponentModel;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace GameSystems.InputSystem
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        public FrameInput FrameInput { get; private set; }
        public ControlScheme CurrentScheme;

        public void SetInputEnabled(bool enabled) => _inputEnabled = enabled;
        private bool _inputEnabled = true;


        private void Update()
        {
            if (!_inputEnabled)
            {
                FrameInput = new FrameInput();
                return;
            }

#if ENABLE_INPUT_SYSTEM
            FrameInput = Gather();
#endif
        }

#if ENABLE_INPUT_SYSTEM
        private GameInput _gameInput;

        private InputAction _cancel;

        // Mouse controls
        private InputAction _mouseDelta, _mousePos, _scroll, _scrollDelta;

        // Player controls
        private InputAction _move, _jump, _primary, _secondary, _interact, _equip, _inventory;
        private InputAction _hot1, _hot2, _hot3, _hot4;
        private InputAction _rotateLeft, _rotateRight;


        // UI controls
        private InputAction _up, _down, _left, _right;
        private InputAction _shiftUp, _shiftDown;
        private InputAction _previous, _next;

        private void Awake()
        {
            _gameInput = new GameInput();

            // Mouse controls
            _mouseDelta = _gameInput.Player.MouseDelta;
            _mousePos = _gameInput.Player.MousePos;
            _scrollDelta = _gameInput.Player.ScrollDelta;
            _scroll = _gameInput.Player.Scroll;

            // Player controls
            _move = _gameInput.Player.Move;
            _jump = _gameInput.Player.Jump;
            _primary = _gameInput.Player.AttackPrimary;
            _secondary = _gameInput.Player.AttackSecondary;
            _interact = _gameInput.Player.Interact;
            _equip = _gameInput.Player.Equip;
            _rotateLeft = _gameInput.Player.RotateLeft;
            _rotateRight = _gameInput.Player.RotateRight;

            _hot1 = _gameInput.Player.Hotkey1;
            _hot2 = _gameInput.Player.Hotkey2;
            _hot3 = _gameInput.Player.Hotkey3;
            _hot4 = _gameInput.Player.Hotkey4;
            _cancel = _gameInput.UI.Escape;

            _inventory = _gameInput.Player.ToggleInventory;

            // UI controls
            // _shiftUp = _gameInput.UI.ShiftUp;
            // _shiftDown = _gameInput.UI.ShiftDown;
            // _up = _gameInput.UI.Up;
            // _down = _gameInput.UI.Down;
            // _left = _gameInput.UI.Left;
            // _right = _gameInput.UI.Right;
            // _previous = _gameInput.UI.Previous;
            // _next = _gameInput.UI.Next;
        }

        private void OnEnable()
        {
            _gameInput.Player.Enable();
            _gameInput.UI.Enable();
        }

        private void OnDisable()
        {
            _gameInput.Player.Disable();
            _gameInput.UI.Disable();
        }

        public void ClearInput()
        {
            FrameInput = new FrameInput();
        }

        private FrameInput Gather()
        {
            return new FrameInput
            {
                // Mouse controls
                MouseDelta = _mouseDelta.ReadValue<Vector2>(),
                MousePos = _mousePos.ReadValue<Vector2>(),
                Scroll = _scroll.ReadValue<Vector2>(),
                ScrollDelta = _scrollDelta.ReadValue<Vector2>(),
                
                MouseWorldPos2D = Vector3.ProjectOnPlane(_camera.ScreenToWorldPoint(_mousePos.ReadValue<Vector2>()),
                    Vector3.forward),

                // Player controls
                Move = _move.ReadValue<Vector2>(),
                Jump = (_jump.WasPressedThisFrame(), _jump.IsPressed()),
                Interact = (_interact.WasPressedThisFrame(), _interact.WasReleasedThisFrame(), _interact.IsPressed()),
                Equip = (_equip.WasPressedThisFrame(), _equip.IsPressed()),

                RotateRight = (_rotateRight.WasPressedThisFrame(), _rotateRight.WasReleasedThisFrame(),
                    _rotateRight.IsPressed()),
                RotateLeft = (_rotateLeft.WasPressedThisFrame(), _rotateLeft.WasReleasedThisFrame(),
                    _rotateLeft.IsPressed()),

                Primary = (_primary.WasPressedThisFrame(), _primary.WasReleasedThisFrame(), _primary.IsPressed()),
                Secondary = (_secondary.WasPressedThisFrame(), _secondary.WasReleasedThisFrame(),
                    _secondary.IsPressed()),

                Inventory = (_inventory.WasPressedThisFrame(), _inventory.WasReleasedThisFrame()),

                Hot1 = (_hot1.WasPressedThisFrame(), _hot1.IsPressed(), _hot1.WasReleasedThisFrame()),
                Hot2 = (_hot2.WasPressedThisFrame(), _hot2.IsPressed(), _hot2.WasReleasedThisFrame()),
                Hot3 = (_hot3.WasPressedThisFrame(), _hot3.IsPressed(), _hot3.WasReleasedThisFrame()),
                Hot4 = (_hot4.WasPressedThisFrame(), _hot4.IsPressed(), _hot4.WasReleasedThisFrame()),

                Escape = (_cancel.WasPressedThisFrame(), _cancel.IsPressed())
            };
        }
#endif
    }

    public struct FrameInput
    {
        // Mouse controls
        public Vector2 MouseDelta;
        public Vector2 MousePos;
        public Vector2 MouseWorldPos2D;
        public Vector2 Scroll;
        public Vector2 ScrollDelta;

        // Player controls
        public Vector2 Move;
        public (bool down, bool held) Jump;
        public (bool down, bool held) Equip;
        public (bool down, bool held) Escape;
        public (bool down, bool up, bool held) Interact;
        public (bool down, bool up, bool held) Primary;
        public (bool down, bool up, bool held) Secondary;

        public (bool down, bool up, bool held) RotateRight;
        public (bool down, bool up, bool held) RotateLeft;

        public (bool down, bool up) Inventory;

        public (bool down, bool up, bool held) Hot1;
        public (bool down, bool up, bool held) Hot2;
        public (bool down, bool up, bool held) Hot3;
        public (bool down, bool up, bool held) Hot4;
    }

    public struct EventInput
    {
        // UI controls
        public (bool down, bool up) ShiftUp;
        public (bool down, bool up) ShiftDown;
        public (bool down, bool up) Up;
        public (bool down, bool up) Down;
        public (bool down, bool up) Left;
        public (bool down, bool up) Right;
        public (bool down, bool up) Previous;
        public (bool down, bool up) Next;
    }

    public enum ControlScheme
    {
        KEYBOARD,
        GAMEPAD
    }
}


