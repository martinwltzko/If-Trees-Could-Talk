using UnityEngine;

namespace GameSystems.InputSystem
{
    public abstract class InputController : MonoBehaviour, IInputReceiver
    {
        private bool _enabled;
        
        public string InputIdentifier => name;
        
        public void EnableInput()
        {
            if (!_enabled) InputComposer.RegisterInput(this);
            _enabled = true;
        }

        public void DisableInput()
        {
            if (_enabled) InputComposer.ReleaseInput(this);
            _enabled = false;
        }

        private void OnEnable()
        {
            EnableInput();
        }

        private void OnDisable()
        {
            DisableInput();
        }
        
        public abstract void UpdateInput(FrameInput frameInput);
    }
}