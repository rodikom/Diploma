using Core.Services;
using UnityEngine;

namespace Game.Input
{
    public sealed class InputDebugView : MonoBehaviour
    {
        [SerializeField] private float _axisLogInterval = 0.25f;

        private IInputService _inputService;
        private float _nextAxisLogTime;

        private void Start()
        {
            if (!ServiceLocator.TryGet(out _inputService))
            {
                Debug.LogError("[InputDebugView] IInputService is not registered. Start the game from Bootstrap scene.");
                enabled = false;
                return;
            }

            _inputService.LightAttackPressed += OnLightAttack;
            _inputService.HeavyAttackPressed += OnHeavyAttack;
            _inputService.DodgeOrStepPressed += OnDodgeOrStep;
            _inputService.LockOnPressed += OnLockOn;
            _inputService.InteractPressed += OnInteract;
            _inputService.PausePressed += OnPause;

            Debug.Log("[InputDebugView] Input debug started.");
        }

        private void Update()
        {
            if (Time.time < _nextAxisLogTime)
                return;

            _nextAxisLogTime = Time.time + _axisLogInterval;

            Vector2 move = _inputService.Move;
            Vector2 look = _inputService.Look;
            bool block = _inputService.IsBlockHeld;

            if (move.sqrMagnitude > 0.01f)
                Debug.Log($"Move: {move}");

            if (look.sqrMagnitude > 0.01f)
                Debug.Log($"Look: {look}");

            if (block)
                Debug.Log("Block held");
        }

        private void OnDestroy()
        {
            if (_inputService == null)
                return;

            _inputService.LightAttackPressed -= OnLightAttack;
            _inputService.HeavyAttackPressed -= OnHeavyAttack;
            _inputService.DodgeOrStepPressed -= OnDodgeOrStep;
            _inputService.LockOnPressed -= OnLockOn;
            _inputService.InteractPressed -= OnInteract;
            _inputService.PausePressed -= OnPause;
        }

        private void OnLightAttack()
        {
            Debug.Log("LightAttack pressed");
        }

        private void OnHeavyAttack()
        {
            Debug.Log("HeavyAttack pressed");
        }

        private void OnDodgeOrStep()
        {
            Debug.Log("DodgeOrStep pressed");
        }

        private void OnLockOn()
        {
            Debug.Log("LockOn pressed");
        }

        private void OnInteract()
        {
            Debug.Log("Interact pressed");
        }

        private void OnPause()
        {
            Debug.Log("Pause pressed");
        }
    }
}