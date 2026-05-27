using Combat.Data;
using UnityEngine;

namespace Combat.Runtime
{
    public sealed class MeleeHitDetector : MonoBehaviour
    {
        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private int _maxHits = 16;

        private Collider[] _hits;

        private void Awake()
        {
            _hits = new Collider[_maxHits];
        }

        public bool TryFindTarget(CombatActor attacker, AttackData attackData, out CombatActor target)
        {
            target = null;

            if (attacker == null || attackData == null)
                return false;

            Transform attackerTransform = attacker.transform;

            Vector3 forward = attackerTransform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.0001f)
                return false;

            forward.Normalize();

            Vector3 origin = attackerTransform.position;
            origin += Vector3.up * attackData.HitOriginHeight;
            origin += forward * attackData.HitOriginForwardOffset;

            int hitCount = Physics.OverlapSphereNonAlloc(
                origin,
                attackData.Range + attackData.HitRadius,
                _hits,
                _targetMask,
                QueryTriggerInteraction.Ignore
            );

            float bestScore = float.PositiveInfinity;
            CombatActor bestTarget = null;

            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = _hits[i];

                if (hit == null)
                    continue;

                CombatActor candidate = hit.GetComponentInParent<CombatActor>();

                if (candidate == null)
                    continue;

                if (candidate == attacker)
                    continue;

                if (!candidate.IsAlive)
                    continue;

                if (candidate.Side == attacker.Side)
                    continue;

                Vector3 targetPoint = candidate.transform.position + Vector3.up * attackData.HitOriginHeight;
                Vector3 toTarget = targetPoint - origin;
                toTarget.y = 0f;

                if (toTarget.sqrMagnitude < 0.0001f)
                    continue;

                float distance = toTarget.magnitude;

                if (distance > attackData.Range + attackData.HitRadius)
                    continue;

                float angle = Vector3.Angle(forward, toTarget.normalized);

                if (angle > attackData.HitAngle * 0.5f)
                    continue;

                float score = distance + angle * 0.03f;

                if (score < bestScore)
                {
                    bestScore = score;
                    bestTarget = candidate;
                }
            }

            target = bestTarget;
            return target != null;
        }
    }
}