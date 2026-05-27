using Combat.Runtime;
using UnityEngine;

namespace Characters.Targeting
{
    public sealed class LockOnTarget : MonoBehaviour
    {
        [SerializeField] private CombatActor _actor;
        [SerializeField] private Transform _aimPoint;

        public CombatActor Actor => _actor;
        public Transform AimPoint => _aimPoint != null ? _aimPoint : transform;
        public Vector3 Position => AimPoint.position;
        public bool IsAvailable => _actor == null || _actor.IsAlive;

        private void Awake()
        {
            if (_actor == null)
                _actor = GetComponentInParent<CombatActor>();
        }
    }
}