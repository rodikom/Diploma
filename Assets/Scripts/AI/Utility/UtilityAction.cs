using UnityEngine;

namespace AI.Utility
{
    public abstract class UtilityAction : ScriptableObject
    {
        [SerializeField] private bool _canBeInterrupted = true;
        [SerializeField, Range(0f, 1f)] private float _minimumScoreToRun = 0f;

        public bool CanBeInterrupted => _canBeInterrupted;
        public float MinimumScoreToRun => _minimumScoreToRun;

        public virtual bool CanExecute(UtilityAIContext context)
        {
            return true;
        }

        public abstract float Evaluate(UtilityAIContext context);

        public virtual void OnEnter(UtilityAIContext context)
        {
        }

        public virtual void Execute(UtilityAIContext context)
        {
        }

        public virtual void OnExit(UtilityAIContext context)
        {
        }

        public virtual bool IsFinished(UtilityAIContext context)
        {
            return false;
        }
    }
}