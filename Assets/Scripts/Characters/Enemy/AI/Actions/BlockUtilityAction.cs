using AI.Utility;
using Combat.Core;
using UnityEngine;

namespace Characters.Enemy.AI.Actions
{
    [CreateAssetMenu(menuName = "AI/Enemy/Actions/Block")]
    public sealed class BlockUtilityAction : UtilityAction
    {
        [SerializeField] private UtilityConsideration[] _considerations;

        public override bool CanExecute(UtilityAIContext context)
        {
            if (context.Self == null || context.Self.Guard == null || context.Self.State == null)
                return false;

            if (!context.Self.State.CanBlock())
                return false;

            if (context.Self.Guard.IsBroken)
                return false;

            return true;
        }

        public override float Evaluate(UtilityAIContext context)
        {
            return UtilityScoreMath.MultiplyConsiderations(context, _considerations);
        }

        public override void OnEnter(UtilityAIContext context)
        {
            RaiseGuard(context);
        }

        public override void Execute(UtilityAIContext context)
        {
            RaiseGuard(context);
        }

        private void RaiseGuard(UtilityAIContext context)
        {
            if (context.Self == null || context.Self.Guard == null || context.Self.State == null)
                return;

            context.Self.Guard.Raise();
            context.Self.State.SetState(CombatState.Blocking);
        }
    }
}