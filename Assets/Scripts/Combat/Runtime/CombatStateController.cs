using System;
using Combat.Core;
using UnityEngine;

namespace Combat.Runtime
{
    public sealed class CombatStateController : MonoBehaviour
    {
        [SerializeField] private CombatState _currentState = CombatState.Idle;

        public CombatState CurrentState => _currentState;

        public bool IsIdle => _currentState == CombatState.Idle;
        public bool IsMoving => _currentState == CombatState.Moving;
        public bool IsAttacking => _currentState == CombatState.Attacking;
        public bool IsBlocking => _currentState == CombatState.Blocking;
        public bool IsStepping => _currentState == CombatState.Stepping;
        public bool IsRecovering => _currentState == CombatState.Recovering;
        public bool IsStaggered => _currentState == CombatState.Staggered;
        public bool IsDead => _currentState == CombatState.Dead;

        public event Action<CombatState, CombatState> Changed;

        public void SetState(CombatState state)
        {
            if (_currentState == state)
                return;

            CombatState previousState = _currentState;
            _currentState = state;
            Changed?.Invoke(previousState, _currentState);
        }

        public bool CanAttack()
        {
            return _currentState == CombatState.Idle ||
                   _currentState == CombatState.Moving ||
                   _currentState == CombatState.Blocking;
        }

        public bool CanBlock()
        {
            return _currentState == CombatState.Idle ||
                   _currentState == CombatState.Moving ||
                   _currentState == CombatState.Blocking;
        }

        public bool CanMove()
        {
            return _currentState == CombatState.Idle ||
                   _currentState == CombatState.Moving ||
                   _currentState == CombatState.Blocking;
        }

        public bool CanStep()
        {
            return _currentState == CombatState.Idle ||
                   _currentState == CombatState.Moving ||
                   _currentState == CombatState.Blocking;
        }

        public bool IsVulnerable()
        {
            return _currentState == CombatState.Recovering ||
                   _currentState == CombatState.Staggered;
        }
    }
}