using AI.Perception;
using Characters.Enemy;
using Combat.Runtime;

namespace AI.Utility
{
    public sealed class UtilityAIContext
    {
        public CombatActor Self { get; }
        public EnemyPerception Perception { get; }
        public EnemyMotor Motor { get; }

        public CombatActor Target => Perception != null ? Perception.Target : null;
        public bool HasTarget => Perception != null && Perception.HasTarget;

        public UtilityAIContext(CombatActor self, EnemyPerception perception, EnemyMotor motor)
        {
            Self = self;
            Perception = perception;
            Motor = motor;
        }
    }
}