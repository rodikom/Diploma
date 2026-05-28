using AI.Utility;
using Combat.Core;
using UnityEngine;

namespace Characters.Enemy.AI.Actions
{
    [CreateAssetMenu(menuName = "AI/Enemy/Actions/Step Back")]
    public sealed class StepBackUtilityAction : UtilityAction
    {
        [Header("Scoring")]
        [SerializeField, Range(0f, 1f)] private float _minimumDanger = 0.45f;
        [SerializeField, Range(0f, 1f)] private float _lowGuardThreshold = 0.45f;
        [SerializeField, Range(0f, 1f)] private float _lowStaminaThreshold = 0.35f;
        [SerializeField, Range(0f, 1f)] private float _baseThreatScore = 0.25f;
        [SerializeField, Range(0f, 1f)] private float _lowGuardBonus = 0.35f;
        [SerializeField, Range(0f, 1f)] private float _lowStaminaBonus = 0.25f;

        [Header("Timing")]
        [SerializeField] private float _minimumTimeToEscape = 0.12f;
        [SerializeField] private float _maximumTimeToEscape = 0.75f;

        [Header("Movement")]
        [SerializeField] private float _stepSpeed = 5.5f;
        [SerializeField] private float _stepDuration = 0.22f;
        [SerializeField] private float _recoveryDuration = 0.18f;

        private float _finishStepAt;
        private float _finishRecoveryAt;
        private Vector3 _stepDirection;

        public override bool CanExecute(UtilityAIContext context)
        {
            if (context.Self == null || context.Self.State == null || context.Motor == null)
                return false;

            return context.Self.State.CanStep();
        }

        public override float Evaluate(UtilityAIContext context)
        {
            if (context.Perception == null || !context.Perception.IsThreatened)
                return 0f;

            float danger = context.Perception.PerceivedDanger;

            if (danger < _minimumDanger)
                return 0f;

            float time = context.Perception.PerceivedTimeUntilImpact;

            if (time < _minimumTimeToEscape)
                return 0f;

            if (time > _maximumTimeToEscape)
                return 0f;

            float score = _baseThreatScore;
            score += Mathf.InverseLerp(_minimumDanger, 1f, danger) * 0.3f;

            if (context.Self.Guard != null)
            {
                float guard = context.Self.Guard.Normalized;

                if (context.Self.Guard.IsBroken)
                    score += _lowGuardBonus;
                else if (guard < _lowGuardThreshold)
                    score += (1f - guard / _lowGuardThreshold) * _lowGuardBonus;
            }

            if (context.Self.Stamina != null)
            {
                float stamina = context.Self.Stamina.Normalized;

                if (stamina < _lowStaminaThreshold)
                    score += (1f - stamina / _lowStaminaThreshold) * _lowStaminaBonus;
            }

            return Mathf.Clamp01(score);
        }

        public override void OnEnter(UtilityAIContext context)
        {
            if (context.Self == null || context.Self.State == null)
                return;

            _stepDirection = GetStepDirection(context);
            _finishStepAt = Time.time + _stepDuration;
            _finishRecoveryAt = _finishStepAt + _recoveryDuration;

            context.Self.Guard?.Lower();
            context.Self.State.SetState(CombatState.Stepping);
        }

        public override void Execute(UtilityAIContext context)
        {
            if (context.Self == null || context.Self.State == null || context.Motor == null)
                return;

            if (Time.time < _finishStepAt)
            {
                context.Motor.Move(_stepDirection, _stepSpeed);
                FaceTarget(context);
                return;
            }

            if (Time.time < _finishRecoveryAt)
            {
                context.Self.State.SetState(CombatState.Recovering);
                FaceTarget(context);
                return;
            }

            context.Self.State.SetState(CombatState.Idle);
        }

        public override bool IsFinished(UtilityAIContext context)
        {
            return Time.time >= _finishRecoveryAt;
        }

        private Vector3 GetStepDirection(UtilityAIContext context)
        {
            if (context.Perception == null || context.Perception.DirectionToTarget == Vector3.zero)
                return -context.Self.transform.forward;

            return -context.Perception.DirectionToTarget;
        }

        private void FaceTarget(UtilityAIContext context)
        {
            if (context.Perception == null || context.Motor == null)
                return;

            context.Motor.FaceDirection(context.Perception.DirectionToTarget);
        }
    }
}