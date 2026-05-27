using Combat.Core;
using Combat.Data;

namespace Combat.Runtime
{
    public readonly struct CombatThreatInfo
    {
        public readonly bool HasThreat;
        public readonly CombatActor Attacker;
        public readonly CombatActor Target;
        public readonly AttackData AttackData;
        public readonly AttackPhase Phase;
        public readonly float TimeUntilImpact;
        public readonly float Distance;
        public readonly float Angle;
        public readonly bool IsInRange;
        public readonly bool IsInAngle;
        public readonly bool IsTargetThreatened;
        public readonly float Danger;

        public CombatThreatInfo(
            bool hasThreat,
            CombatActor attacker,
            CombatActor target,
            AttackData attackData,
            AttackPhase phase,
            float timeUntilImpact,
            float distance,
            float angle,
            bool isInRange,
            bool isInAngle,
            bool isTargetThreatened,
            float danger)
        {
            HasThreat = hasThreat;
            Attacker = attacker;
            Target = target;
            AttackData = attackData;
            Phase = phase;
            TimeUntilImpact = timeUntilImpact;
            Distance = distance;
            Angle = angle;
            IsInRange = isInRange;
            IsInAngle = isInAngle;
            IsTargetThreatened = isTargetThreatened;
            Danger = danger;
        }

        public static CombatThreatInfo None(CombatActor attacker, CombatActor target)
        {
            return new CombatThreatInfo(
                false,
                attacker,
                target,
                null,
                AttackPhase.None,
                0f,
                0f,
                0f,
                false,
                false,
                false,
                0f
            );
        }
    }
}