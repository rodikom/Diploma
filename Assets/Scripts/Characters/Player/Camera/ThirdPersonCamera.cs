using Core.Services;
using Game.Input;
using UnityEngine;

namespace Characters.Player
{
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _followTarget;
        [SerializeField] private PlayerLockOn _lockOn;

        [Header("Camera")]
        [SerializeField] private float _distance = 4.5f;
        [SerializeField] private float _height = 0.4f;
        [SerializeField] private float _followSharpness = 35f;

        [Header("Sensitivity")]
        [SerializeField] private float _mouseSensitivity = 0.12f;
        [SerializeField] private float _gamepadSensitivity = 140f;

        [Header("Pitch")]
        [SerializeField] private float _minPitch = -35f;
        [SerializeField] private float _maxPitch = 65f;

        [Header("Lock-On")]
        [SerializeField] private float _lockOnSharpness = 12f;

        private IInputService _inputService;
        private Vector3 _smoothedTargetPosition;
        private float _yaw;
        private float _pitch = 20f;

        public float Yaw => _yaw;

        public Vector3 PlanarForward
        {
            get
            {
                Quaternion rotation = Quaternion.Euler(0f, _yaw, 0f);
                return rotation * Vector3.forward;
            }
        }

        public Vector3 PlanarRight
        {
            get
            {
                Quaternion rotation = Quaternion.Euler(0f, _yaw, 0f);
                return rotation * Vector3.right;
            }
        }

        private void Start()
        {
            if (!ServiceLocator.TryGet(out _inputService))
            {
                Debug.LogError("[ThirdPersonCamera] IInputService is not registered. Start the game from Bootstrap scene.");
                enabled = false;
                return;
            }

            if (_followTarget != null)
                _smoothedTargetPosition = _followTarget.position;

            Vector3 euler = transform.eulerAngles;
            _yaw = euler.y;
            _pitch = NormalizePitch(euler.x);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            if (_followTarget == null)
                return;

            UpdateTargetPosition();

            if (_lockOn != null && _lockOn.HasTarget)
                UpdateLockOnRotation();
            else
                UpdateFreeRotation();

            ApplyCameraTransform();
        }

        private void UpdateTargetPosition()
        {
            float t = 1f - Mathf.Exp(-_followSharpness * Time.deltaTime);
            _smoothedTargetPosition = Vector3.Lerp(_smoothedTargetPosition, _followTarget.position, t);
        }

        private void UpdateFreeRotation()
        {
            Vector2 look = _inputService.Look;

            if (_inputService.IsLookFromMouse)
            {
                _yaw += look.x * _mouseSensitivity;
                _pitch -= look.y * _mouseSensitivity;
            }
            else
            {
                _yaw += look.x * _gamepadSensitivity * Time.deltaTime;
                _pitch -= look.y * _gamepadSensitivity * Time.deltaTime;
            }

            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
        }

        private void UpdateLockOnRotation()
        {
            Vector3 toTarget = _lockOn.CurrentTarget.Position - _smoothedTargetPosition;

            if (toTarget.sqrMagnitude < 0.0001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            Vector3 targetEuler = targetRotation.eulerAngles;

            float targetYaw = targetEuler.y;
            float targetPitch = NormalizePitch(targetEuler.x);

            float t = 1f - Mathf.Exp(-_lockOnSharpness * Time.deltaTime);

            _yaw = Mathf.LerpAngle(_yaw, targetYaw, t);
            _pitch = Mathf.LerpAngle(_pitch, targetPitch, t);
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
        }

        private void ApplyCameraTransform()
        {
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 offset = rotation * new Vector3(0f, _height, -_distance);

            transform.SetPositionAndRotation(
                _smoothedTargetPosition + offset,
                rotation
            );
        }

        private float NormalizePitch(float pitch)
        {
            if (pitch > 180f)
                pitch -= 360f;

            return pitch;
        }
    }
}