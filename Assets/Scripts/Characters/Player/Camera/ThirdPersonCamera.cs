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
        [SerializeField] private Vector3 _targetOffset = new Vector3(0f, 1.5f, 0f);
        [SerializeField] private float _distance = 4.5f;
        [SerializeField] private float _height = 0.4f;
        [SerializeField] private float _mouseSensitivity = 0.12f;
        [SerializeField] private float _gamepadSensitivity = 140f;        [SerializeField] private float _followSmoothTime = 0.06f;
        [SerializeField] private float _rotationSmoothSpeed = 18f;

        [Header("Pitch")]
        [SerializeField] private float _minPitch = -35f;
        [SerializeField] private float _maxPitch = 65f;

        [Header("Lock-On")]
        [SerializeField] private float _lockOnRotationSpeed = 8f;

        private IInputService _inputService;
        private Vector3 _currentTargetPosition;
        private Vector3 _followVelocity;
        private float _yaw;
        private float _pitch = 20f;

        private void Start()
        {
            if (!ServiceLocator.TryGet(out _inputService))
            {
                Debug.LogError("[ThirdPersonCamera] IInputService is not registered. Start the game from Bootstrap scene.");
                enabled = false;
                return;
            }

            if (_followTarget != null)
                _currentTargetPosition = _followTarget.position + _targetOffset;

            Vector3 euler = transform.eulerAngles;
            _yaw = euler.y;
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
            Vector3 desiredPosition = _followTarget.position + _targetOffset;

            _currentTargetPosition = Vector3.SmoothDamp(
                _currentTargetPosition,
                desiredPosition,
                ref _followVelocity,
                _followSmoothTime
            );
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
            Vector3 toTarget = _lockOn.CurrentTarget.Position - _currentTargetPosition;

            if (toTarget.sqrMagnitude < 0.0001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            Vector3 targetEuler = targetRotation.eulerAngles;

            _yaw = Mathf.LerpAngle(_yaw, targetEuler.y, _lockOnRotationSpeed * Time.deltaTime);
            _pitch = Mathf.LerpAngle(_pitch, NormalizePitch(targetEuler.x), _lockOnRotationSpeed * Time.deltaTime);
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
        }

        private void ApplyCameraTransform()
        {
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);

            Vector3 offset = rotation * new Vector3(0f, _height, -_distance);

            transform.position = _currentTargetPosition + offset;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotation,
                _rotationSmoothSpeed * Time.deltaTime
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