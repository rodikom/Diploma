using Combat.Data;
using UnityEngine;

namespace Combat.Runtime
{
    public sealed class CombatDebugTester : MonoBehaviour
    {
        [SerializeField] private CombatActor _attacker;
        [SerializeField] private CombatActor _defender;
        [SerializeField] private AttackData _attackData;
        [SerializeField] private DamageResolver _damageResolver;
        [SerializeField, Range(0f, 1f)] private float _hitQuality = 0.5f;

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.H))
                TestHit();

            if (UnityEngine.Input.GetKeyDown(KeyCode.G))
                ToggleGuard();
        }

        private void TestHit()
        {
            if (_damageResolver == null)
                return;

            HitResultLog(_damageResolver.ResolveHit(_attacker, _defender, _attackData, _hitQuality));
        }

        private void ToggleGuard()
        {
            if (_defender == null || _defender.Guard == null)
                return;

            if (_defender.Guard.IsRaised)
                _defender.Guard.Lower();
            else
                _defender.Guard.Raise();

            Debug.Log($"Guard raised: {_defender.Guard.IsRaised}");
        }

        private void HitResultLog(Core.HitResult result)
        {
            Debug.Log(
                $"Hit: {result.WasHit}, Guard: {result.GuardResult}, Quality: {result.HitQuality}, HealthDamage: {result.HealthDamage}, StaminaDamage: {result.StaminaDamage}, GuardDamage: {result.GuardDamage}, Fatal: {result.IsFatal}"
            );
        }
    }
}