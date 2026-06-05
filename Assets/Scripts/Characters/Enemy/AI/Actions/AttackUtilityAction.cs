using AI.Utility;
using Combat.Data;
using UnityEngine;

namespace Characters.Enemy.AI
{
    [CreateAssetMenu(menuName = "AI/Actions/Attack")]
    public sealed class AttackUtilityAction : UtilityAction
    {
        [Header("Attack")]
        [SerializeField] private AttackData _attackData;
        [SerializeField] private bool _heavyAttack;

        [Header("Scoring")]
        [SerializeField, Range(0f, 1f)] private float _baseScore = 0.3f;
        [SerializeField, Range(0f, 1f)] private float _vulnerableTargetBonus = 0.35f;
        [SerializeField, Range(0f, 1f)] private float _threatPenalty = 0.5f;
        [SerializeField] private float _distanceTolerance = 0.75f;
        [SerializeField, Range(1f, 180f)] private float _maxAngleToStartAttack = 35f;

        public override bool CanExecute(UtilityAIContext context)
        {
            if (context == null || context.Attack == null)
                return false;

            if (context.Self == null || !context.Self.IsAlive)
                return false;

            if (!context.HasTarget)
                return false;

            if (_attackData == null)
                return false;

            if (context.Self.State == null || !context.Self.State.CanAttack())
                return false;

            if (context.Self.AttackPhase != null && context.Self.AttackPhase.IsAttacking)
                return false;

            if (context.Perception == null)
                return false;

            float maxDistance = _attackData.Range + _attackData.HitRadius;

            if (context.Perception.PerceivedDistanceToTarget > maxDistance)
                return false;

            return IsTargetInFront(context);
        }

        public override float Evaluate(UtilityAIContext context)
        {
            if (!CanExecute(context))
                return 0f;

            float preferredDistance = Mathf.Max(0.01f, _attackData.Range * 0.75f);
            float distance = context.Perception.PerceivedDistanceToTarget;

            float distanceScore = 1f - Mathf.Clamp01(
                Mathf.Abs(distance - preferredDistance) / Mathf.Max(0.01f, _distanceTolerance)
            );

            float angleScore = GetAngleScore(context);

            float score = _baseScore;
            score += distanceScore * 0.35f;
            score += angleScore * 0.25f;

            if (context.Perception.TargetIsVulnerable)
                score += _vulnerableTargetBonus;

            if (context.Perception.IsThreatened)
                score *= 1f - _threatPenalty * context.Perception.PerceivedDanger;

            if (context.Self.Stamina != null && context.Self.Stamina.Normalized < 0.25f)
                score *= 0.35f;

            return Mathf.Clamp01(score);
        }

        public override void OnEnter(UtilityAIContext context)
        {
            if (context == null || context.Attack == null)
                return;

            if (context.Perception != null && context.Motor != null)
                context.Motor.FaceDirection(context.Perception.DirectionToTarget);

            context.Attack.TryStartAttack(_attackData, _heavyAttack);
        }

        public override bool IsFinished(UtilityAIContext context)
        {
            return context == null ||
                   context.Attack == null ||
                   !context.Attack.IsAttacking;
        }

        private bool IsTargetInFront(UtilityAIContext context)
        {
            if (context.Perception.DirectionToTarget.sqrMagnitude < 0.0001f)
                return false;

            Vector3 forward = context.Self.transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.0001f)
                return false;

            forward.Normalize();

            float angle = Vector3.Angle(forward, context.Perception.DirectionToTarget);

            return angle <= _maxAngleToStartAttack;
        }

        private float GetAngleScore(UtilityAIContext context)
        {
            Vector3 forward = context.Self.transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.0001f)
                return 0f;

            forward.Normalize();

            float angle = Vector3.Angle(forward, context.Perception.DirectionToTarget);
            return 1f - Mathf.Clamp01(angle / Mathf.Max(1f, _maxAngleToStartAttack));
        }
    }
}