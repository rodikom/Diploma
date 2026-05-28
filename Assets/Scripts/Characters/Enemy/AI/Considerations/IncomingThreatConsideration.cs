using AI.Utility;
using UnityEngine;

namespace Characters.Enemy.AI.Considerations
{
    [CreateAssetMenu(menuName = "AI/Enemy/Considerations/Incoming Threat")]
    public sealed class IncomingThreatConsideration : UtilityConsideration
    {
        [SerializeField, Range(0f, 1f)] private float _minimumDanger = 0.1f;

        protected override float Evaluate(UtilityAIContext context)
        {
            if (context.Perception == null)
                return 0f;

            if (!context.Perception.IsThreatened)
                return 0f;

            float danger = context.Perception.PerceivedDanger;

            if (danger < _minimumDanger)
                return 0f;

            return danger;
        }
    }
}