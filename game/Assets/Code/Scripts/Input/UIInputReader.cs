using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

namespace Code.Scripts.Input
{
    [CreateAssetMenu(fileName = "UIInputReader", menuName = "Input/UIInput")]
    public class UIInputReader : ScriptableObject, IUIActions, IInputReader {

        public PlayerInputActions InputActions;
        public Vector2 Direction { get; }

        public event UnityAction<bool> Cancel = delegate { };
        public event UnityAction<Vector2> Navigate = delegate { };
        public event UnityAction Submit = delegate { };
        public event UnityAction<Vector2> Point = delegate { };
        public event UnityAction<Vector2, bool> Delta = delegate { };
        public event UnityAction<Vector2> ScrollWheel = delegate { };
        public event UnityAction<Vector3> TrackedDevicePosition = delegate { };
        public event UnityAction<Quaternion> TrackedDeviceOrientation = delegate { };
        
        public event UnityAction<bool> Primary = delegate { };
        public event UnityAction<bool> Secondary = delegate { };
        
        public Vector2 MouseDelta => InputActions.UI.Look.ReadValue<Vector2>();

        public void EnablePlayerActions() {
            if (InputActions == null) {
                InputActions = new PlayerInputActions();
                InputActions.UI.SetCallbacks(this);
            }
            InputActions.Enable();
        }
        public void DisablePlayerActions() {
            InputActions.Disable();
        }
        
        bool IsDeviceMouse(InputAction.CallbackContext context) {
            // Debug.Log($"Device name: {context.control.device.name}");
            return context.control.device.name == "Mouse";
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                Navigate.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                Submit.Invoke();
            }
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Cancel.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Cancel.Invoke(false);
                    break;
            }
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                Point.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Delta.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }

        public void OnPrimary(InputAction.CallbackContext context) {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Primary.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Primary.Invoke(false);
                    break;
            }
        }
    
        public void OnSecondary(InputAction.CallbackContext context) {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Secondary.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Secondary.Invoke(false);
                    break;
            }
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                ScrollWheel.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                TrackedDevicePosition.Invoke(context.ReadValue<Vector3>());
            }
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                TrackedDeviceOrientation.Invoke(context.ReadValue<Quaternion>());
            }
        }
    }
}