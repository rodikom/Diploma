using Combat.Core;
using Combat.Data;
using UnityEngine;

namespace Combat.Runtime
{
    public sealed class AttackThreatProvider : MonoBehaviour
    {
        [SerializeField] private CombatActor _actor;
        [SerializeField, Range(0f, 1f)] private float _windUpBaseDanger = 0.35f;
        [SerializeField, Range(0f, 1f)] private float _activeDanger = 1f;
        [SerializeField, Range(0f, 1f)] private float _recoveryDanger = 0f;

        public CombatActor Actor => _actor;

        public bool HasThreat
        {
            get
            {
                if (_actor == null || !_actor.IsAlive || _actor.AttackPhase == null)
                    return false;

                return _actor.AttackPhase.IsInWindUp || _actor.AttackPhase.IsActive;
            }
        }

        private void Awake()
        {
            if (_actor == null)
                _actor = GetComponent<CombatActor>();
        }

        public CombatThreatInfo EvaluateThreatAgainst(CombatActor target)
        {
            if (_actor == null || target == null)
                return CombatThreatInfo.None(_actor, target);

            if (!_actor.IsAlive || !target.IsAlive)
                return CombatThreatInfo.None(_actor, target);

            AttackPhaseController attackPhase = _actor.AttackPhase;

            if (attackPhase == null || attackPhase.CurrentAttack == null)
                return CombatThreatInfo.None(_actor, target);

            AttackData attackData = attackPhase.CurrentAttack;
            AttackPhase phase = attackPhase.CurrentPhase;

            if (phase == AttackPhase.None || phase == AttackPhase.Recovery)
                return CombatThreatInfo.None(_actor, target);

            Vector3 forward = transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.0001f)
                return CombatThreatInfo.None(_actor, target);

            forward.Normalize();

            Vector3 origin = GetThreatOrigin(attackData, forward);
            Vector3 targetPoint = target.transform.position + Vector3.up * attackData.HitOriginHeight;
            Vector3 toTarget = targetPoint - origin;
            toTarget.y = 0f;

            if (toTarget.sqrMagnitude < 0.0001f)
                return CombatThreatInfo.None(_actor, target);

            float distance = toTarget.magnitude;
            float angle = Vector3.Angle(forward, toTarget.normalized);

            bool isInRange = distance <= attackData.Range + attackData.HitRadius;
            bool isInAngle = angle <= attackData.HitAngle * 0.5f;
            bool isTargetThreatened = isInRange && isInAngle;

            float danger = CalculateDanger(
                phase,
                attackPhase.TimeUntilImpact,
                attackData,
                distance,
                angle,
                isTargetThreatened
            );

            return new CombatThreatInfo(
                true,
                _actor,
                target,
                attackData,
                phase,
                attackPhase.TimeUntilImpact,
                distance,
                angle,
                isInRange,
                isInAngle,
                isTargetThreatened,
                danger
            );
        }

        private Vector3 GetThreatOrigin(AttackData attackData, Vector3 forward)
        {
            Vector3 origin = transform.position;
            origin += Vector3.up * attackData.HitOriginHeight;
            origin += forward * attackData.HitOriginForwardOffset;

            return origin;
        }

        private float CalculateDanger(
            AttackPhase phase,
            float timeUntilImpact,
            AttackData attackData,
            float distance,
            float angle,
            bool isTargetThreatened)
        {
            if (!isTargetThreatened)
                return 0f;

            float phaseDanger = GetPhaseDanger(phase, timeUntilImpact, attackData);
            float distanceFactor = GetDistanceFactor(distance, attackData);
            float angleFactor = GetAngleFactor(angle, attackData);

            return Mathf.Clamp01(phaseDanger * distanceFactor * angleFactor);
        }

        private float GetPhaseDanger(AttackPhase phase, float timeUntilImpact, AttackData attackData)
        {
            if (phase == AttackPhase.Active)
                return _activeDanger;

            if (phase == AttackPhase.Recovery)
                return _recoveryDanger;

            if (phase != AttackPhase.WindUp)
                return 0f;

            float windUp = Mathf.Max(0.01f, attackData.WindUpTime);
            float progress = 1f - Mathf.Clamp01(timeUntilImpact / windUp);

            return Mathf.Lerp(_windUpBaseDanger, _activeDanger, progress);
        }

        private float GetDistanceFactor(float distance, AttackData attackData)
        {
            float maxDistance = Mathf.Max(0.01f, attackData.Range + attackData.HitRadius);
            float normalizedDistance = Mathf.Clamp01(distance / maxDistance);

            return 1f - normalizedDistance * 0.35f;
        }

        private float GetAngleFactor(float angle, AttackData attackData)
        {
            float halfAngle = Mathf.Max(1f, attackData.HitAngle * 0.5f);
            float normalizedAngle = Mathf.Clamp01(angle / halfAngle);

            return 1f - normalizedAngle * 0.45f;
        }

        private void OnDrawGizmosSelected()
        {
            if (_actor == null)
                _actor = GetComponent<CombatActor>();

            if (_actor == null || _actor.AttackPhase == null || _actor.AttackPhase.CurrentAttack == null)
                return;

            AttackData attackData = _actor.AttackPhase.CurrentAttack;

            Vector3 forward = transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.0001f)
                return;

            forward.Normalize();

            Vector3 origin = GetThreatOrigin(attackData, forward);

            Gizmos.DrawWireSphere(origin, attackData.Range + attackData.HitRadius);

            Quaternion leftRotation = Quaternion.AngleAxis(-attackData.HitAngle * 0.5f, Vector3.up);
            Quaternion rightRotation = Quaternion.AngleAxis(attackData.HitAngle * 0.5f, Vector3.up);

            Gizmos.DrawLine(origin, origin + leftRotation * forward * attackData.Range);
            Gizmos.DrawLine(origin, origin + rightRotation * forward * attackData.Range);
        }
    }
}