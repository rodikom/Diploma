using AI.Utility;
using UnityEngine;

namespace Characters.Enemy.AI.Considerations
{
    [CreateAssetMenu(menuName = "AI/Enemy/Considerations/Guard Availability")]
    public sealed class GuardAvailabilityConsideration : UtilityConsideration
    {
        [SerializeField, Range(0f, 1f)] private float _minimumGuard = 0.15f;

        protected override float Evaluate(UtilityAIContext context)
        {
            if (context.Self == null || context.Self.Guard == null)
                return 0f;

            if (context.Self.Guard.IsBroken)
                return 0f;

            float guard = context.Self.Guard.Normalized;

            if (guard < _minimumGuard)
                return 0f;

            return guard;
        }
    }
}