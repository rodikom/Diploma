using Combat.Core;
using Combat.Runtime;
using Core.Services;
using Game.Input;
using UnityEngine;

namespace Characters.Player
{
    public sealed class PlayerGuardController : MonoBehaviour
    {
        [SerializeField] private CombatActor _actor;

        private IInputService _inputService;

        private void Awake()
        {
            if (_actor == null)
                _actor = GetComponent<CombatActor>();
        }

        private void Start()
        {
            if (!ServiceLocator.TryGet(out _inputService))
            {
                Debug.LogError("[PlayerGuardController] IInputService is not registered. Start the game from Bootstrap scene.");
                enabled = false;
                return;
            }
        }

        private void Update()
        {
            if (_actor == null || _actor.Guard == null || _actor.State == null)
                return;

            if (!_actor.IsAlive)
            {
                _actor.Guard.Lower();
                return;
            }

            if (!_actor.State.CanBlock())
            {
                _actor.Guard.Lower();
                return;
            }

            if (_inputService.IsBlockHeld)
            {
                _actor.Guard.Raise();
                _actor.State.SetState(CombatState.Blocking);
                return;
            }

            if (_actor.State.IsBlocking)
                _actor.State.SetState(CombatState.Idle);

            _actor.Guard.Lower();
        }
    }
}