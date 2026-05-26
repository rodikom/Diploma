using System.Collections;
using Combat.Core;
using Combat.Runtime;
using Core.Services;
using Game.Input;
using UnityEngine;

namespace Characters.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerStep : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private CombatActor _actor;
        [SerializeField] private Transform _cameraTransform;

        [Header("Step")]
        [SerializeField] private float _stepDistance = 2.2f;
        [SerializeField] private float _stepDuration = 0.18f;
        [SerializeField] private float _recoveryDuration = 0.22f;
        [SerializeField] private float _staminaCost = 22f;

        [Header("Fallback")]
        [SerializeField] private Vector3 _fallbackLocalDirection = Vector3.back;

        private IInputService _inputService;
        private Coroutine _stepRoutine;

        public bool IsStepping => _actor != null &&
                                  _actor.State != null &&
                                  _actor.State.IsStepping;

        private void Awake()
        {
            if (_characterController == null)
                _characterController = GetComponent<CharacterController>();

            if (_actor == null)
                _actor = GetComponent<CombatActor>();

            if (_cameraTransform == null && Camera.main != null)
                _cameraTransform = Camera.main.transform;
        }

        private void Start()
        {
            if (!ServiceLocator.TryGet(out _inputService))
            {
                Debug.LogError("[PlayerStep] IInputService is not registered. Start the game from Bootstrap scene.");
                enabled = false;
                return;
            }

            _inputService.DodgeOrStepPressed += OnStepPressed;
        }

        private void OnDestroy()
        {
            if (_inputService == null)
                return;

            _inputService.DodgeOrStepPressed -= OnStepPressed;
        }

        private void OnStepPressed()
        {
            TryStep();
        }

        private void TryStep()
        {
            if (_actor == null || !_actor.IsAlive)
                return;

            if (_actor.State == null || !_actor.State.CanStep())
                return;

            if (_actor.Stamina != null && !_actor.Stamina.TrySpendStamina(_staminaCost))
                return;

            Vector3 direction = GetStepDirection();

            if (direction.sqrMagnitude < 0.0001f)
                return;

            if (_stepRoutine != null)
                StopCoroutine(_stepRoutine);

            _stepRoutine = StartCoroutine(StepRoutine(direction));
        }

        private IEnumerator StepRoutine(Vector3 direction)
        {
            _actor.Guard?.Lower();
            _actor.State.SetState(CombatState.Stepping);

            float elapsed = 0f;
            float speed = _stepDistance / Mathf.Max(0.01f, _stepDuration);

            while (elapsed < _stepDuration)
            {
                _characterController.Move(direction * (speed * Time.deltaTime));
                elapsed += Time.deltaTime;
                yield return null;
            }

            _actor.State.SetState(CombatState.Recovering);

            yield return new WaitForSeconds(_recoveryDuration);

            if (_actor.IsAlive && _actor.State != null)
                _actor.State.SetState(CombatState.Idle);

            _stepRoutine = null;
        }

        private Vector3 GetStepDirection()
        {
            Vector2 input = _inputService.Move;

            if (input.sqrMagnitude > 0.01f)
                return GetCameraRelativeDirection(input);

            Vector3 fallbackDirection = transform.TransformDirection(_fallbackLocalDirection);
            fallbackDirection.y = 0f;

            if (fallbackDirection.sqrMagnitude < 0.0001f)
                return -transform.forward;

            return fallbackDirection.normalized;
        }

        private Vector3 GetCameraRelativeDirection(Vector2 input)
        {
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
    }
}