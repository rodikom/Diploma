using Combat.Core;
using Combat.Runtime;
using UnityEngine;

namespace Characters.Enemy
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class EnemyMotor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private CombatActor _actor;

        [Header("Movement")]
        [SerializeField] private float _gravity = -25f;
        [SerializeField] private float _groundedVerticalVelocity = -2f;
        [SerializeField] private float _rotationSpeedDegrees = 540f;

        private float _verticalVelocity;

        private void Awake()
        {
            if (_characterController == null)
                _characterController = GetComponent<CharacterController>();

            if (_actor == null)
                _actor = GetComponent<CombatActor>();
        }

        private void Update()
        {
            ApplyGravityOnly();
        }

        public void Move(Vector3 direction, float speed)
        {
            if (_actor == null || !_actor.IsAlive)
                return;

            if (_actor.State != null &&
                (_actor.State.IsAttacking ||
                 _actor.State.IsRecovering ||
                 _actor.State.IsStaggered ||
                 _actor.State.IsDead))
                return;

            direction.y = 0f;

            if (direction.sqrMagnitude > 1f)
                direction.Normalize();

            Vector3 velocity = direction * speed;
            velocity.y = _verticalVelocity;

            _characterController.Move(velocity * Time.deltaTime);

            if (_actor.State != null &&
                !_actor.State.IsBlocking &&
                !_actor.State.IsStepping)
                _actor.State.SetState(CombatState.Moving);
        }

        public void StopMoving()
        {
            if (_actor == null || _actor.State == null)
                return;

            if (_actor.State.IsMoving)
                _actor.State.SetState(CombatState.Idle);
        }

        public void FaceDirection(Vector3 direction)
        {
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.0001f)
                return;

            float targetYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float currentYaw = transform.eulerAngles.y;

            float newYaw = Mathf.MoveTowardsAngle(
                currentYaw,
                targetYaw,
                _rotationSpeedDegrees * Time.deltaTime
            );

            transform.rotation = Quaternion.Euler(0f, newYaw, 0f);
        }

        private void ApplyGravityOnly()
        {
            if (_characterController == null)
                return;

            if (_characterController.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = _groundedVerticalVelocity;

            _verticalVelocity += _gravity * Time.deltaTime;

            Vector3 velocity = Vector3.up * _verticalVelocity;
            _characterController.Move(velocity * Time.deltaTime);
        }
    }
}