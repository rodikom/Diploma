using System.Collections;
using Combat.Core;
using Combat.Data;
using Combat.Runtime;
using Game.Animation;
using UnityEngine;

namespace Characters.Enemy
{
    public sealed class EnemyAttackController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CombatActor _actor;
        [SerializeField] private DamageResolver _damageResolver;
        [SerializeField] private MeleeHitDetector _hitDetector;
        [SerializeField] private CombatAnimator _animator;

        [Header("Hit")]
        [SerializeField, Range(0f, 1f)] private float _hitQuality = 0.5f;

        [Header("Debug")]
        [SerializeField] private bool _showDebug;

        private Coroutine _attackRoutine;

        public bool IsAttacking => _actor != null &&
                                   _actor.AttackPhase != null &&
                                   _actor.AttackPhase.IsAttacking;

        private void Awake()
        {
            if (_actor == null)
                _actor = GetComponent<CombatActor>();

            if (_damageResolver == null)
                _damageResolver = GetComponent<DamageResolver>();

            if (_hitDetector == null)
                _hitDetector = GetComponent<MeleeHitDetector>();

            if (_animator == null)
                _animator = GetComponent<CombatAnimator>();

            if (_animator == null)
                _animator = GetComponentInChildren<CombatAnimator>();
        }

        public bool TryStartAttack(AttackData attackData, bool heavyAttack)
        {
            if (_actor == null || !_actor.IsAlive)
                return false;

            if (_actor.State == null || !_actor.State.CanAttack())
                return false;

            if (_actor.AttackPhase != null && _actor.AttackPhase.IsAttacking)
                return false;

            if (attackData == null)
                return false;

            if (_actor.Stamina != null && !_actor.Stamina.TrySpendStamina(attackData.StaminaCost))
                return false;

            if (heavyAttack)
                _animator?.PlayHeavyAttack();
            else
                _animator?.PlayLightAttack();

            _attackRoutine = StartCoroutine(AttackRoutine(attackData));
            return true;
        }

        private IEnumerator AttackRoutine(AttackData attackData)
        {
            _actor.State.SetState(CombatState.Attacking);

            if (_actor.Guard != null)
                _actor.Guard.Lower();

            if (_actor.AttackPhase != null)
                _actor.AttackPhase.BeginAttack(attackData);

            yield return new WaitForSeconds(attackData.WindUpTime);

            if (_actor.AttackPhase != null)
                _actor.AttackPhase.SetActive();

            ApplyAttackHit(attackData);

            yield return new WaitForSeconds(attackData.ActiveTime);

            if (_actor.AttackPhase != null)
                _actor.AttackPhase.SetRecovery();

            if (_actor.State != null)
                _actor.State.SetState(CombatState.Recovering);

            yield return new WaitForSeconds(attackData.RecoveryTime);

            if (_actor.AttackPhase != null)
                _actor.AttackPhase.EndAttack();

            if (_actor.IsAlive && _actor.State != null)
                _actor.State.SetState(CombatState.Idle);

            _attackRoutine = null;
        }

        private void ApplyAttackHit(AttackData attackData)
        {
            if (_damageResolver == null)
                return;

            if (_hitDetector == null)
                return;

            if (!_hitDetector.TryFindTarget(_actor, attackData, out CombatActor target))
            {
                if (_showDebug)
                    Debug.Log($"[EnemyAttackController] Attack={attackData.name}, Miss");

                return;
            }

            HitResult result = _damageResolver.ResolveHit(_actor, target, attackData, _hitQuality);

            if (_showDebug)
            {
                Debug.Log(
                    $"[EnemyAttackController] Attack={attackData.name}, Target={target.name}, Phase={_actor.AttackPhase?.CurrentPhase}, Hit={result.WasHit}, Guard={result.GuardResult}, Quality={result.HitQuality}, Damage={result.HealthDamage}, Fatal={result.IsFatal}"
                );
            }
        }
    }
}