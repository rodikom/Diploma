using Combat.Core;
using Combat.Runtime;
using UnityEngine;

namespace AI.Perception
{
    public sealed class EnemyPerception : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CombatActor _self;
        [SerializeField] private CombatActor _target;
        [SerializeField] private EnemySkillProfile _skillProfile;

        [Header("Fallback")]
        [SerializeField] private float _defaultSenseInterval = 0.08f;

        private float _nextSenseTime;

        public CombatActor Self => _self;
        public CombatActor Target => _target;
        public EnemySkillProfile SkillProfile => _skillProfile;

        public bool HasTarget => _target != null && _target.IsAlive;
        public float DistanceToTarget { get; private set; }
        public float PerceivedDistanceToTarget { get; private set; }
        public Vector3 DirectionToTarget { get; private set; }
        public CombatThreatInfo IncomingThreat { get; private set; }
        public float PerceivedDanger { get; private set; }
        public float PerceivedTimeUntilImpact { get; private set; }
        public float PerceivedAngle { get; private set; }
        public bool IsThreatened { get; private set; }
        public bool TargetIsAttacking { get; private set; }
        public bool TargetIsRecovering { get; private set; }
        public bool TargetIsBlocking { get; private set; }
        public bool TargetIsVulnerable { get; private set; }

        private void Awake()
        {
            if (_self == null)
                _self = GetComponent<CombatActor>();
        }

        private void Update()
        {
            if (_self == null || !_self.IsAlive)
                return;

            if (Time.time < _nextSenseTime)
                return;

            float interval = _skillProfile != null ? _skillProfile.SenseInterval : _defaultSenseInterval;
            _nextSenseTime = Time.time + interval;

            Sense();
        }

        public void SetTarget(CombatActor target)
        {
            _target = target;
        }

        private void Sense()
        {
            ResetPerception();

            if (_target == null || !_target.IsAlive)
                return;

            UpdateTargetGeometry();
            UpdateTargetState();
            UpdateIncomingThreat();
            ApplyPerceptionError();
        }

        private void ResetPerception()
        {
            DistanceToTarget = float.PositiveInfinity;
            PerceivedDistanceToTarget = float.PositiveInfinity;
            DirectionToTarget = Vector3.zero;
            IncomingThreat = CombatThreatInfo.None(null, null);
            PerceivedDanger = 0f;
            PerceivedTimeUntilImpact = 0f;
            PerceivedAngle = 0f;
            IsThreatened = false;
            TargetIsAttacking = false;
            TargetIsRecovering = false;
            TargetIsBlocking = false;
            TargetIsVulnerable = false;
        }

        private void UpdateTargetGeometry()
        {
            Vector3 offset = _target.transform.position - _self.transform.position;
            offset.y = 0f;

            DistanceToTarget = offset.magnitude;
            DirectionToTarget = offset.sqrMagnitude > 0.0001f ? offset.normalized : Vector3.zero;
        }

        private void UpdateTargetState()
        {
            if (_target.State == null)
                return;

            TargetIsAttacking = _target.State.IsAttacking;
            TargetIsRecovering = _target.State.IsRecovering;
            TargetIsBlocking = _target.State.IsBlocking;
            TargetIsVulnerable = _target.State.IsVulnerable();
        }

        private void UpdateIncomingThreat()
        {
            if (_target.ThreatProvider == null)
                return;

            IncomingThreat = _target.ThreatProvider.EvaluateThreatAgainst(_self);
            IsThreatened = IncomingThreat.HasThreat && IncomingThreat.IsTargetThreatened;
        }

        private void ApplyPerceptionError()
        {
            PerceivedDistanceToTarget = DistanceToTarget;
            PerceivedDanger = IncomingThreat.Danger;
            PerceivedTimeUntilImpact = IncomingThreat.TimeUntilImpact;
            PerceivedAngle = IncomingThreat.Angle;

            if (_skillProfile == null)
                return;

            PerceivedDistanceToTarget = Mathf.Max(
                0f,
                DistanceToTarget + Random.Range(-_skillProfile.DistanceError, _skillProfile.DistanceError)
            );

            PerceivedDanger = Mathf.Clamp01(
                IncomingThreat.Danger + Random.Range(-_skillProfile.DangerNoise, _skillProfile.DangerNoise)
            );

            PerceivedTimeUntilImpact = Mathf.Max(
                0f,
                IncomingThreat.TimeUntilImpact + Random.Range(-_skillProfile.TimingError, _skillProfile.TimingError)
            );

            PerceivedAngle = Mathf.Max(
                0f,
                IncomingThreat.Angle + Random.Range(-_skillProfile.AngleError, _skillProfile.AngleError)
            );
        }
    }
}