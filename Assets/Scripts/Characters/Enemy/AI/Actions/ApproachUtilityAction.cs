using AI.Utility;
using UnityEngine;

namespace Characters.Enemy.AI.Actions
{
    [CreateAssetMenu(menuName = "AI/Enemy/Actions/Approach")]
    public sealed class ApproachUtilityAction : UtilityAction
    {
        [Header("Scoring")]
        [SerializeField] private float _desiredDistance = 2.2f;
        [SerializeField] private float _maxUsefulDistance = 8f;
        [SerializeField, Range(0f, 1f)] private float _threatPenalty = 0.8f;
        [SerializeField, Range(0f, 1f)] private float _targetRecoveryBonus = 0.25f;

        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 3.2f;
        [SerializeField] private float _stopDistance = 2f;

        public override bool CanExecute(UtilityAIContext context)
        {
            if (context.Self == null || context.Self.State == null || context.Motor == null)
                return false;

            if (!context.HasTarget)
                return false;

            if (!context.Self.State.CanMove())
                return false;

            return true;
        }

        public override float Evaluate(UtilityAIContext context)
        {
            if (context.Perception == null || !context.Perception.HasTarget)
                return 0f;

            float distance = context.Perception.PerceivedDistanceToTarget;

            if (distance <= _desiredDistance)
                return 0f;

            float score = Mathf.InverseLerp(_desiredDistance, _maxUsefulDistance, distance);

            if (context.Perception.IsThreatened)
                score *= 1f - _threatPenalty;

            if (context.Perception.TargetIsRecovering)
                score += _targetRecoveryBonus;

            return Mathf.Clamp01(score);
        }

        public override void OnEnter(UtilityAIContext context)
        {
            FaceTarget(context);
        }

        public override void Execute(UtilityAIContext context)
        {
            if (context.Perception == null || context.Motor == null)
                return;

            if (!context.Perception.HasTarget)
            {
                context.Motor.StopMoving();
                return;
            }

            FaceTarget(context);

            if (context.Perception.DistanceToTarget <= _stopDistance)
            {
                context.Motor.StopMoving();
                return;
            }

            context.Motor.Move(context.Perception.DirectionToTarget, _moveSpeed);
        }

        public override void OnExit(UtilityAIContext context)
        {
            context.Motor?.StopMoving();
        }

        private void FaceTarget(UtilityAIContext context)
        {
            if (context.Perception == null || context.Motor == null)
                return;

            context.Motor.FaceDirection(context.Perception.DirectionToTarget);
        }
    }
}