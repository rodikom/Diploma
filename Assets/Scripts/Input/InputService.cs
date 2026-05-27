using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input
{
    public sealed class InputService : IInputService
    {
        private readonly GameInputActions _inputActions;

        public Vector2 Move => _inputActions.Player.Move.ReadValue<Vector2>();
        public Vector2 Look => _inputActions.Player.Look.ReadValue<Vector2>();
        public bool IsBlockHeld => _inputActions.Player.Block.IsPressed();

        public bool IsLookFromMouse =>
            _inputActions.Player.Look.activeControl != null &&
            _inputActions.Player.Look.activeControl.device is Mouse;
        
        public event System.Action LightAttackPressed;
        public event System.Action HeavyAttackPressed;
        public event System.Action DodgeOrStepPressed;
        public event System.Action LockOnPressed;
        public event System.Action InteractPressed;
        public event System.Action PausePressed;

        public InputService()
        {
            _inputActions = new GameInputActions();

            _inputActions.Player.LightAttack.performed += OnLightAttack;
            _inputActions.Player.HeavyAttack.performed += OnHeavyAttack;
            _inputActions.Player.DodgeOrStep.performed += OnDodgeOrStep;
            _inputActions.Player.LockOn.performed += OnLockOn;
            _inputActions.Player.Interact.performed += OnInteract;
            _inputActions.Player.Pause.performed += OnPause;
        }

        public void Enable()
        {
            _inputActions.Enable();
        }

        public void Disable()
        {
            _inputActions.Disable();
        }

        private void OnLightAttack(InputAction.CallbackContext context)
        {
            LightAttackPressed?.Invoke();
        }

        private void OnHeavyAttack(InputAction.CallbackContext context)
        {
            HeavyAttackPressed?.Invoke();
        }

        private void OnDodgeOrStep(InputAction.CallbackContext context)
        {
            DodgeOrStepPressed?.Invoke();
        }

        private void OnLockOn(InputAction.CallbackContext context)
        {
            LockOnPressed?.Invoke();
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            InteractPressed?.Invoke();
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            PausePressed?.Invoke();
        }
    }
}