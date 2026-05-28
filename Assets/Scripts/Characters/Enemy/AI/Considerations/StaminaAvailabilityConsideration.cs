using AI.Utility;
using UnityEngine;

namespace Characters.Enemy.AI.Considerations
{
    [CreateAssetMenu(menuName = "AI/Enemy/Considerations/Stamina Availability")]
    public sealed class StaminaAvailabilityConsideration : UtilityConsideration
    {
        [SerializeField, Range(0f, 1f)] private float _minimumStamina = 0.12f;

        protected override float Evaluate(UtilityAIContext context)
        {
            if (context.Self == null || context.Self.Stamina == null)
                return 1f;

            float stamina = context.Self.Stamina.Normalized;

            if (stamina < _minimumStamina)
                return 0f;

            return stamina;
        }
    }
}