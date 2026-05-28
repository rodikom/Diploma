using AI.Utility;
using UnityEngine;

namespace Characters.Enemy.AI.Actions
{
    [CreateAssetMenu(menuName = "AI/Enemy/Actions/Idle")]
    public sealed class IdleUtilityAction : UtilityAction
    {
        [SerializeField, Range(0f, 1f)] private float _score = 0.1f;

        public override float Evaluate(UtilityAIContext context)
        {
            return _score;
        }

        public override void OnEnter(UtilityAIContext context)
        {
            if (context.Self == null || context.Self.Guard == null)
                return;

            context.Self.Guard.Lower();

            if (context.Self.State != null && context.Self.State.IsBlocking)
                context.Self.State.SetState(Combat.Core.CombatState.Idle);
        }
    }
}