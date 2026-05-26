using Combat.Core;
using UnityEngine;

namespace Combat.Data
{
    [CreateAssetMenu(menuName = "Combat/Attack Data")]
    public sealed class AttackData : ScriptableObject
    {
        [Header("Damage")]
        [SerializeField] private DamageType _damageType = DamageType.Slash;
        [SerializeField] private float _healthDamage = 35f;
        [SerializeField] private float _staminaDamage = 20f;
        [SerializeField] private float _guardDamage = 25f;

        [Header("Lethality")]
        [SerializeField, Range(0f, 1f)] private float _criticalThreshold = 0.65f;
        [SerializeField, Range(0f, 1f)] private float _fatalThreshold = 0.9f;

        [Header("Timing")]
        [SerializeField] private float _windUpTime = 0.35f;
        [SerializeField] private float _activeTime = 0.18f;
        [SerializeField] private float _recoveryTime = 0.45f;

        [Header("Cost")]
        [SerializeField] private float _staminaCost = 18f;

        public DamageType DamageType => _damageType;
        public float HealthDamage => _healthDamage;
        public float StaminaDamage => _staminaDamage;
        public float GuardDamage => _guardDamage;
        public float CriticalThreshold => _criticalThreshold;
        public float FatalThreshold => _fatalThreshold;
        public float WindUpTime => _windUpTime;
        public float ActiveTime => _activeTime;
        public float RecoveryTime => _recoveryTime;
        public float StaminaCost => _staminaCost;
    }
}