using AI.Utility;
using UnityEngine;

namespace Characters.Enemy.AI.Actions
{
    [CreateAssetMenu(menuName = "AI/Enemy/Actions/Face Target")]
    public sealed class FaceTargetUtilityAction : UtilityAction
    {
        [Header("Scoring")]
        [SerializeField, Range(1f, 180f)] private float _minAngleToFace = 12f;
        [SerializeField, Range(1f, 180f)] private float _maxAngle = 120f;
        [SerializeField, Range(0f, 1f)] private float _baseScore = 0.35f;
        [SerializeField, Range(0f, 1f)] private float _closeTargetBonus = 0.35f;
        [SerializeField] private float _closeDistance = 2.5f;

        public override bool CanExecute(UtilityAIContext context)
        {
            if (context == null || context.Self == null || context.Motor == null)
                return false;

            if (!context.HasTarget || context.Perception == null)
                return false;

            if (context.Self.State == null || !context.Self.State.CanMove())
                return false;

            return GetAngleToTarget(context) > _minAngleToFace;
        }

        public override float Evaluate(UtilityAIContext context)
        {
            if (!CanExecute(context))
                return 0f;

            float angle = GetAngleToTarget(context);
            float angleScore = Mathf.Clamp01(angle / _maxAngle);

            float score = _baseScore + angleScore * 0.45f;

            if (context.Perception.PerceivedDistanceToTarget <= _closeDistance)
                score += _closeTargetBonus;

            if (context.Perception.IsThreatened)
                score *= 0.5f;

            return Mathf.Clamp01(score);
        }

        public override void Execute(UtilityAIContext context)
        {
            if (context == null || context.Perception == null || context.Motor == null)
                return;

            context.Motor.FaceDirection(context.Perception.DirectionToTarget);
            context.Motor.StopMoving();
        }

        public override bool IsFinished(UtilityAIContext context)
        {
            if (context == null || context.Perception == null)
                return true;

            return GetAngleToTarget(context) <= _minAngleToFace;
        }

        private float GetAngleToTarget(UtilityAIContext context)
        {
            Vector3 forward = context.Self.transform.forward;
            forward.y = 0f;

            Vector3 direction = context.Perception.DirectionToTarget;
            direction.y = 0f;

            if (forward.sqrMagnitude < 0.0001f || direction.sqrMagnitude < 0.0001f)
                return 0f;

            return Vector3.Angle(forward.normalized, direction.normalized);
        }
    }
}