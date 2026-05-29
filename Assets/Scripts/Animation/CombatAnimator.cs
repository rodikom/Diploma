using Combat.Core;
using Combat.Runtime;
using UnityEngine;

namespace Game.Animation
{
    public sealed class CombatAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private CombatActor _actor;

        [Header("Parameters")]
        [SerializeField] private string _moveXParameter = "MoveX";
        [SerializeField] private string _moveYParameter = "MoveY";
        [SerializeField] private string _moveSpeedParameter = "MoveSpeed";
        [SerializeField] private string _isBlockingParameter = "IsBlocking";
        [SerializeField] private string _lightAttackTrigger = "LightAttack";
        [SerializeField] private string _heavyAttackTrigger = "HeavyAttack";
        [SerializeField] private string _stepBackTrigger = "StepBack";
        [SerializeField] private string _hitTrigger = "Hit";
        [SerializeField] private string _dieTrigger = "Die";

        [Header("Movement")]
        [SerializeField] private float _movementAnimationScale = 0.35f;
        [SerializeField] private float _movementDampTime = 0.1f;

        private int _moveXHash;
        private int _moveYHash;
        private int _moveSpeedHash;
        private int _isBlockingHash;
        private int _lightAttackHash;
        private int _heavyAttackHash;
        private int _stepBackHash;
        private int _hitHash;
        private int _dieHash;

        private CombatState _previousState;
        private bool _deathPlayed;
        private Vector3 _previousPosition;

        private void Awake()
        {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();

            if (_actor == null)
                _actor = GetComponent<CombatActor>();

            _moveXHash = Animator.StringToHash(_moveXParameter);
            _moveYHash = Animator.StringToHash(_moveYParameter);
            _moveSpeedHash = Animator.StringToHash(_moveSpeedParameter);
            _isBlockingHash = Animator.StringToHash(_isBlockingParameter);
            _lightAttackHash = Animator.StringToHash(_lightAttackTrigger);
            _heavyAttackHash = Animator.StringToHash(_heavyAttackTrigger);
            _stepBackHash = Animator.StringToHash(_stepBackTrigger);
            _hitHash = Animator.StringToHash(_hitTrigger);
            _dieHash = Animator.StringToHash(_dieTrigger);

            _previousPosition = transform.position;

            if (_actor != null && _actor.State != null)
                _previousState = _actor.State.CurrentState;
        }

        private void Update()
        {
            if (_animator == null || _actor == null || _actor.State == null)
                return;

            UpdateMovementParameters();
            UpdateBlockingParameter();
            UpdateStateTriggers();
        }

        public void PlayLightAttack()
        {
            if (_animator == null)
                return;

            _animator.SetTrigger(_lightAttackHash);
        }

        public void PlayHeavyAttack()
        {
            if (_animator == null)
                return;

            _animator.SetTrigger(_heavyAttackHash);
        }

        public void PlayStepBack()
        {
            if (_animator == null)
                return;

            _animator.SetTrigger(_stepBackHash);
        }

        public void PlayHit()
        {
            if (_animator == null)
                return;

            _animator.SetTrigger(_hitHash);
        }

        public void PlayDeath()
        {
            if (_animator == null || _deathPlayed)
                return;

            _deathPlayed = true;
            _animator.SetTrigger(_dieHash);
        }

        private void UpdateMovementParameters()
        {
            Vector3 currentPosition = transform.position;
            Vector3 velocity = (currentPosition - _previousPosition) / Mathf.Max(Time.deltaTime, 0.0001f);
            _previousPosition = currentPosition;

            velocity.y = 0f;

            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            float moveX = Mathf.Clamp(localVelocity.x * _movementAnimationScale, -1f, 1f);
            float moveY = Mathf.Clamp(localVelocity.z * _movementAnimationScale, -1f, 1f);
            float moveSpeed = velocity.magnitude;

            _animator.SetFloat(_moveXHash, moveX, _movementDampTime, Time.deltaTime);
            _animator.SetFloat(_moveYHash, moveY, _movementDampTime, Time.deltaTime);
            _animator.SetFloat(_moveSpeedHash, moveSpeed, _movementDampTime, Time.deltaTime);
        }

        private void UpdateBlockingParameter()
        {
            bool isBlocking = _actor.Guard != null && _actor.Guard.IsRaised;
            _animator.SetBool(_isBlockingHash, isBlocking);
        }

        private void UpdateStateTriggers()
        {
            CombatState currentState = _actor.State.CurrentState;

            if (currentState == CombatState.Dead)
            {
                PlayDeath();
                _previousState = currentState;
                return;
            }

            if (currentState == _previousState)
                return;

            if (currentState == CombatState.Stepping)
                PlayStepBack();

            _previousState = currentState;
        }
    }
}