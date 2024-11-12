using UnityEngine;
using UnityUtils;

namespace AdvancedController {
    public class CameraController : MonoBehaviour {
        #region Fields
        private float _currentXAngle;
        private float _currentYAngle;
        private Transform _tr;
        
        [Range(0f, 90f)] public float upperVerticalLimit = 35f;
        [Range(0f, 90f)] public float lowerVerticalLimit = 35f;
        
        public float cameraSpeed = 50f;
        public bool smoothCameraRotation;
        [Range(1f, 50f)] public float cameraSmoothingFactor = 25f;
        [SerializeField] InputProvider inputProvider;
        private InputReader Input => inputProvider.Input;
        #endregion
        
        public Vector3 GetUpDirection() => _tr.up;
        public Vector3 GetFacingDirection () => _tr.forward;

        void Awake() {
            _tr = transform;
            
            _currentXAngle = _tr.localRotation.eulerAngles.x;
            _currentYAngle = _tr.localRotation.eulerAngles.y;
        }

        void Update() {
            RotateCamera(Input.LookDirection.x, -Input.LookDirection.y);
        }

        void RotateCamera(float horizontalInput, float verticalInput) {
            if (smoothCameraRotation) {
                horizontalInput = Mathf.Lerp(0, horizontalInput, Time.deltaTime * cameraSmoothingFactor);
                verticalInput = Mathf.Lerp(0, verticalInput, Time.deltaTime * cameraSmoothingFactor);
            }
            
            _currentXAngle += verticalInput * cameraSpeed * Time.deltaTime;
            _currentYAngle += horizontalInput * cameraSpeed * Time.deltaTime;
            
            _currentXAngle = Mathf.Clamp(_currentXAngle, -upperVerticalLimit, lowerVerticalLimit);
            
            _tr.localRotation = Quaternion.Euler(_currentXAngle, _currentYAngle, 0);
        }
    }
}