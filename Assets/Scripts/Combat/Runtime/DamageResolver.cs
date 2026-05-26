using Combat.Core;
using Combat.Data;
using UnityEngine;

namespace Combat.Runtime
{
    public sealed class DamageResolver : MonoBehaviour
    {
        public HitResult ResolveHit(CombatActor attacker, CombatActor defender, AttackData attackData, float hitQuality01)
        {
            if (attacker == null || defender == null || attackData == null)
                return HitResult.Miss();

            if (!attacker.IsAlive || !defender.IsAlive)
                return HitResult.Miss();

            if (defender.Health == null)
                return HitResult.Miss();

            hitQuality01 = Mathf.Clamp01(hitQuality01);

            GuardResult guardResult = GuardResult.None;
            HitQuality hitQuality = GetHitQuality(attackData, hitQuality01);

            float healthDamage = attackData.HealthDamage;
            float staminaDamage = attackData.StaminaDamage;
            float guardDamage = attackData.GuardDamage;

            Guard defenderGuard = defender.Guard;
            Stamina defenderStamina = defender.Stamina;
            Health defenderHealth = defender.Health;

            bool canBlock = defenderGuard != null &&
                            defenderGuard.IsRaised &&
                            !defenderGuard.IsBroken;

            if (canBlock)
            {
                defenderGuard.DamageGuard(guardDamage);

                if (defenderStamina != null)
                    defenderStamina.DrainStamina(staminaDamage * defenderGuard.BlockedStaminaDamageMultiplier);

                if (defenderGuard.IsBroken)
                {
                    guardResult = GuardResult.GuardBroken;
                    defenderHealth.TakeDamage(healthDamage);
                }
                else
                {
                    guardResult = GuardResult.Blocked;
                    healthDamage *= defenderGuard.BlockedHealthDamageMultiplier;
                    defenderHealth.TakeDamage(healthDamage);
                }
            }
            else
            {
                defenderHealth.TakeDamage(healthDamage);
            }

            bool isFatal = defenderHealth.IsDead || hitQuality == HitQuality.Fatal;

            if (isFatal && !defenderHealth.IsDead)
                defenderHealth.Kill();

            return new HitResult(
                true,
                guardResult,
                hitQuality,
                healthDamage,
                staminaDamage,
                guardDamage,
                isFatal
            );
        }

        private HitQuality GetHitQuality(AttackData attackData, float hitQuality01)
        {
            if (hitQuality01 >= attackData.FatalThreshold)
                return HitQuality.Fatal;

            if (hitQuality01 >= attackData.CriticalThreshold)
                return HitQuality.Critical;

            if (hitQuality01 >= 0.35f)
                return HitQuality.Clean;

            return HitQuality.Glancing;
        }
    }
}