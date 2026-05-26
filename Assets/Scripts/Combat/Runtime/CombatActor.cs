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

        public CombatSide Side => _side;
        public Health Health => _health;
        public Stamina Stamina => _stamina;
        public Guard Guard => _guard;
        public bool IsAlive => _health != null && !_health.IsDead;

        private void Awake()
        {
            if (_health == null)
                _health = GetComponent<Health>();

            if (_stamina == null)
                _stamina = GetComponent<Stamina>();

            if (_guard == null)
                _guard = GetComponent<Guard>();
        }
    }
}