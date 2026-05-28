using AI.Utility;
using UnityEngine;

namespace Characters.Enemy.AI.Considerations
{
    [CreateAssetMenu(menuName = "AI/Enemy/Considerations/Block Timing")]
    public sealed class BlockTimingConsideration : UtilityConsideration
    {
        [SerializeField] private float _minimumTimeToBlock = 0.06f;
        [SerializeField] private float _maximumTimeToBlock = 0.7f;

        protected override float Evaluate(UtilityAIContext context)
        {
            if (context.Perception == null || !context.Perception.IsThreatened)
                return 0f;

            float time = context.Perception.PerceivedTimeUntilImpact;

            if (time < _minimumTimeToBlock)
                return 0f;

            if (time > _maximumTimeToBlock)
                return 0f;

            float normalized = 1f - Mathf.Clamp01((time - _minimumTimeToBlock) / (_maximumTimeToBlock - _minimumTimeToBlock));
            return Mathf.Lerp(0.4f, 1f, normalized);
        }
    }
}