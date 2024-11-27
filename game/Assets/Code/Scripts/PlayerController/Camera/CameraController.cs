using Unity.Cinemachine;
using UnityEngine;
using UnityUtils;

namespace AdvancedController {
    
    [RequireComponent(typeof(CinemachineCamera))]
    public class CameraController : MonoBehaviour {
        
        [SerializeField] private PlayerInstance player;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private new Camera camera;
        [SerializeField] private Transform camAnchor;

        [SerializeField, Range(0f, 90f)] private float upperVerticalLimit = 35f;
        [SerializeField, Range(0f, 90f)] private float lowerVerticalLimit = 35f;
        [SerializeField, Range(1f, 50f)] private float cameraSmoothingFactor = 25f;

        [SerializeField] private float cameraSpeed = 50f;
        [SerializeField] private bool smoothCameraRotation;

        private InputReader Input => player.InputReader;
        public Camera Camera => camera;
        public CinemachineCamera CinemachineCamera => cinemachineCamera;
        public Vector3 GetUpDirection() => camAnchor.up;
        public Vector3 GetFacingDirection () => camAnchor.forward;
        
        private float _currentXAngle;
        private float _currentYAngle;

        void Awake() {
            _currentXAngle = camAnchor.localRotation.eulerAngles.x;
            _currentYAngle = camAnchor.localRotation.eulerAngles.y;
        }

        void FixedUpdate() {
            RotateCamera(Input.LookDirection.x, -Input.LookDirection.y);
        }

        void RotateCamera(float horizontalInput, float verticalInput) {
            if (smoothCameraRotation) {
                horizontalInput = Mathf.Lerp(0, horizontalInput, Time.fixedDeltaTime * cameraSmoothingFactor);
                verticalInput = Mathf.Lerp(0, verticalInput, Time.fixedDeltaTime * cameraSmoothingFactor);
            }
            
            _currentXAngle += verticalInput * cameraSpeed * Time.fixedDeltaTime;
            _currentYAngle += horizontalInput * cameraSpeed * Time.fixedDeltaTime;
            
            _currentXAngle = Mathf.Clamp(_currentXAngle, -upperVerticalLimit, lowerVerticalLimit);
            
            camAnchor.localRotation = Quaternion.Euler(_currentXAngle, _currentYAngle, 0);
        }
    }
}