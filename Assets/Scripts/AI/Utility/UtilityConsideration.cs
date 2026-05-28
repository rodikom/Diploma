using UnityEngine;

namespace AI.Utility
{
    public abstract class UtilityConsideration : ScriptableObject
    {
        [SerializeField, Range(0f, 1f)] private float _weight = 1f;

        public float Weight => _weight;

        public float EvaluateWeighted(UtilityAIContext context)
        {
            return Mathf.Clamp01(Evaluate(context)) * _weight;
        }

        protected abstract float Evaluate(UtilityAIContext context);
    }
}