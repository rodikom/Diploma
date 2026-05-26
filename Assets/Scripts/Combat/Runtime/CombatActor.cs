using Combat.Core;
using UnityEngine;

namespace Combat.Runtime
{
    public sealed class CombatActor : MonoBehaviour
    {
        [SerializeField] private CombatSide _side;
        [SerializeField] private Health _health;
        [SerializeField] private Stamina _stamina;
        [SerializeField] private Guard _guard;
        [SerializeField] private CombatStateController _state;
        [SerializeField] private AttackPhaseController _attackPhase;

        public CombatSide Side => _side;
        public Health Health => _health;
        public Stamina Stamina => _stamina;
        public Guard Guard => _guard;
        public CombatStateController State => _state;
        public AttackPhaseController AttackPhase => _attackPhase;
        public bool IsAlive => _health != null && !_health.IsDead;

        private void Awake()
        {
            if (_health == null)
                _health = GetComponent<Health>();

            if (_stamina == null)
                _stamina = GetComponent<Stamina>();

            if (_guard == null)
                _guard = GetComponent<Guard>();

            if (_state == null)
                _state = GetComponent<CombatStateController>();

            if (_attackPhase == null)
                _attackPhase = GetComponent<AttackPhaseController>();

            if (_health != null)
                _health.Died += OnDied;
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.Died -= OnDied;
        }

        private void OnDied()
        {
            if (_state != null)
                _state.SetState(CombatState.Dead);

            if (_attackPhase != null)
                _attackPhase.CancelAttack();

            if (_guard != null)
                _guard.Lower();
        }
    }
}