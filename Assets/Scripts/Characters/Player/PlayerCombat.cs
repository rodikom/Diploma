using System.Collections;
using Combat.Core;
using Combat.Data;
using Combat.Runtime;
using Core.Services;
using Game.Input;
using UnityEngine;

namespace Characters.Player
{
    public sealed class PlayerCombat : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CombatActor _actor;
        [SerializeField] private DamageResolver _damageResolver;
        [SerializeField] private MeleeHitDetector _hitDetector;

        [Header("Attacks")]
        [SerializeField] private AttackData _lightAttack;
        [SerializeField] private AttackData _heavyAttack;

        [Header("Debug")]
        [SerializeField, Range(0f, 1f)] private float _hitQuality = 0.5f;

        private IInputService _inputService;
        private Coroutine _attackRoutine;

        public bool IsAttacking => _actor != null &&
                                   _actor.State != null &&
                                   _actor.State.IsAttacking;

        private void Awake()
        {
            if (_actor == null)
                _actor = GetComponent<CombatActor>();

            if (_hitDetector == null)
                _hitDetector = GetComponent<MeleeHitDetector>();
        }

        private void Start()
        {
            if (!ServiceLocator.TryGet(out _inputService))
            {
                Debug.LogError("[PlayerCombat] IInputService is not registered. Start the game from Bootstrap scene.");
                enabled = false;
                return;
            }

            _inputService.LightAttackPressed += OnLightAttackPressed;
            _inputService.HeavyAttackPressed += OnHeavyAttackPressed;
        }

        private void OnDestroy()
        {
            if (_inputService == null)
                return;

            _inputService.LightAttackPressed -= OnLightAttackPressed;
            _inputService.HeavyAttackPressed -= OnHeavyAttackPressed;
        }

        private void OnLightAttackPressed()
        {
            TryStartAttack(_lightAttack);
        }

        private void OnHeavyAttackPressed()
        {
            TryStartAttack(_heavyAttack);
        }

        private void TryStartAttack(AttackData attackData)
        {
            if (_actor == null || !_actor.IsAlive)
                return;

            if (_actor.State == null || !_actor.State.CanAttack())
                return;

            if (_actor.AttackPhase != null && _actor.AttackPhase.IsAttacking)
                return;

            if (attackData == null)
                return;

            if (_actor.Stamina != null && !_actor.Stamina.TrySpendStamina(attackData.StaminaCost))
                return;

            if (_attackRoutine != null)
                StopCoroutine(_attackRoutine);

            _attackRoutine = StartCoroutine(AttackRoutine(attackData));
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
            {
                Debug.LogError("[PlayerCombat] DamageResolver is missing.");
                return;
            }

            if (_hitDetector == null)
            {
                Debug.LogError("[PlayerCombat] MeleeHitDetector is missing.");
                return;
            }

            if (!_hitDetector.TryFindTarget(_actor, attackData, out CombatActor target))
            {
                Debug.Log($"[PlayerCombat] Attack={attackData.name}, Miss");
                return;
            }

            HitResult result = _damageResolver.ResolveHit(_actor, target, attackData, _hitQuality);

            Debug.Log(
                $"[PlayerCombat] Attack={attackData.name}, Target={target.name}, Phase={_actor.AttackPhase?.CurrentPhase}, Hit={result.WasHit}, Guard={result.GuardResult}, Quality={result.HitQuality}, Damage={result.HealthDamage}, Fatal={result.IsFatal}"
            );
        }
    }
}