using Combat.Core;
using Game.Animation;
using UnityEngine;

namespace Combat.Runtime
{
    public sealed class CombatHitReaction : MonoBehaviour
    {
        [SerializeField] private CombatActor _actor;
        [SerializeField] private CombatAnimator _animator;

        private void Awake()
        {
            if (_actor == null)
                _actor = GetComponent<CombatActor>();

            if (_animator == null)
                _animator = GetComponent<CombatAnimator>();

            if (_animator == null)
                _animator = GetComponentInChildren<CombatAnimator>();
        }

        private void OnEnable()
        {
            if (_actor != null)
                _actor.HitReceived += OnHitReceived;
        }

        private void OnDisable()
        {
            if (_actor != null)
                _actor.HitReceived -= OnHitReceived;
        }

        private void OnHitReceived(HitResult result)
        {
            if (!result.WasHit)
                return;

            if (result.IsFatal)
            {
                _animator?.PlayDeath();
                return;
            }

            _animator?.PlayHit();
        }
    }
}