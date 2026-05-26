namespace Combat.Core
{
    public readonly struct HitResult
    {
        public readonly bool WasHit;
        public readonly GuardResult GuardResult;
        public readonly HitQuality HitQuality;
        public readonly float HealthDamage;
        public readonly float StaminaDamage;
        public readonly float GuardDamage;
        public readonly bool IsFatal;

        public HitResult(
            bool wasHit,
            GuardResult guardResult,
            HitQuality hitQuality,
            float healthDamage,
            float staminaDamage,
            float guardDamage,
            bool isFatal)
        {
            WasHit = wasHit;
            GuardResult = guardResult;
            HitQuality = hitQuality;
            HealthDamage = healthDamage;
            StaminaDamage = staminaDamage;
            GuardDamage = guardDamage;
            IsFatal = isFatal;
        }

        public static HitResult Miss()
        {
            return new HitResult(
                false,
                GuardResult.None,
                HitQuality.Miss,
                0f,
                0f,
                0f,
                false
            );
        }
    }
}