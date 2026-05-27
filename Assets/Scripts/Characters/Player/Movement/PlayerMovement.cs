using Combat.Runtime;
using Core.Services;
using Game.Input;
using UnityEngine;

namespace Characters.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private CombatActor _actor;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private PlayerLockOn _lockOn;
        
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 4.5f;
        [SerializeField] private float _blockingMoveSpeed = 2.2f;
        [SerializeField] private float _rotationSpeed = 14f;
        [SerializeField] private float _gravity = -25f;
        [SerializeField] private float _groundedVerticalVelocity = -2f;

        private IInputService _inputService;
        private float _verticalVelocity;

        private void Awake()
        {
            if (_characterController == null)
                _characterController = GetComponent<CharacterController>();

            if (_actor == null)
                _actor = GetComponent<CombatActor>();

            if (_cameraTransform == null && Camera.main != null)
                _cameraTransform = Camera.main.transform;
            
            if (_lockOn == null)
                _lockOn = GetComponent<PlayerLockOn>();
        }

        private void Start()
        {
            if (!ServiceLocator.TryGet(out _inputService))
            {
                Debug.LogError("[PlayerMovement] IInputService is not registered. Start the game from Bootstrap scene.");
                enabled = false;
                return;
            }
        }

        private void Update()
        {
            if (_actor == null || !_actor.IsAlive)
                return;

            ApplyMovement();
        }

        private void ApplyMovement()
        {
            Vector2 input = _inputService.Move;
            Vector3 moveDirection = GetCameraRelativeDirection(input);

            float speed = GetCurrentMoveSpeed();
            Vector3 horizontalVelocity = moveDirection * speed;

            ApplyRotation(moveDirection);
            ApplyGravity();

            Vector3 velocity = horizontalVelocity;
            velocity.y = _verticalVelocity;

            _characterController.Move(velocity * Time.deltaTime);

            UpdateMovementState(moveDirection);
        }

        private Vector3 GetCameraRelativeDirection(Vector2 input)
        {
            if (input.sqrMagnitude < 0.0001f)
                return Vector3.zero;

            Vector3 forward = Vector3.forward;
            Vector3 right = Vector3.right;

            if (_cameraTransform != null)
            {
                forward = _cameraTransform.forward;
                right = _cameraTransform.right;
            }

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 direction = forward * input.y + right * input.x;

            if (direction.sqrMagnitude > 1f)
                direction.Normalize();

            return direction;
        }

        private float GetCurrentMoveSpeed()
        {
            if (_actor.State == null)
                return _moveSpeed;

            if (_actor.State.IsAttacking ||
                _actor.State.IsStepping ||
                _actor.State.IsRecovering ||
                _actor.State.IsStaggered)
                return 0f;

            if (_actor.State.IsBlocking)
                return _blockingMoveSpeed;

            return _moveSpeed;
        }

        private void ApplyRotation(Vector3 moveDirection)
        {
            if (_lockOn != null && _lockOn.HasTarget)
            {
                Vector3 toTarget = _lockOn.CurrentTarget.Position - transform.position;
                toTarget.y = 0f;

                if (toTarget.sqrMagnitude < 0.0001f)
                    return;

                Quaternion targetRotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    _rotationSpeed * Time.deltaTime
                );

                return;
            }

            if (moveDirection.sqrMagnitude < 0.0001f)
                return;

            Quaternion movementRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                movementRotation,
                _rotationSpeed * Time.deltaTime
            );
        }

        private void ApplyGravity()
        {
            if (_characterController.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = _groundedVerticalVelocity;

            _verticalVelocity += _gravity * Time.deltaTime;
        }

        private void UpdateMovementState(Vector3 moveDirection)
        {
            if (_actor.State == null)
                return;

            if (!_actor.State.CanMove())
                return;

            if (_actor.State.IsBlocking)
                return;

            if (moveDirection.sqrMagnitude > 0.0001f)
                _actor.State.SetState(Combat.Core.CombatState.Moving);
            else if (_actor.State.IsMoving)
                _actor.State.SetState(Combat.Core.CombatState.Idle);
        }
    }
}