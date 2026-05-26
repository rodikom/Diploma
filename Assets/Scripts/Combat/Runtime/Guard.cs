using System;
using Core.Resources;
using UnityEngine;

namespace Combat.Runtime
{
    public sealed class Guard : Resource
    {
        [SerializeField] private float _recoveryPerSecond = 18f;
        [SerializeField] private float _recoveryDelay = 1.2f;
        [SerializeField, Range(0f, 1f)] private float _recoveredFromBreakThreshold = 0.35f;
        [SerializeField, Range(0f, 1f)] private float _blockedHealthDamageMultiplier = 0.15f;
        [SerializeField, Range(0f, 1f)] private float _blockedStaminaDamageMultiplier = 1f;

        private float _recoverAtTime;
        private bool _isRaised;
        private bool _isBroken;

        public bool IsRaised => _isRaised;
        public bool IsBroken => _isBroken;
        public float BlockedHealthDamageMultiplier => _blockedHealthDamageMultiplier;
        public float BlockedStaminaDamageMultiplier => _blockedStaminaDamageMultiplier;

        public event Action Raised;
        public event Action Lowered;
        public event Action Broken;
        public event Action Recovered;

        protected override void Awake()
        {
            base.Awake();
            Emptied += OnEmptied;
        }

        private void Update()
        {
            if (Time.time < _recoverAtTime)
                return;

            if (IsFull)
                return;

            Increase(_recoveryPerSecond * Time.deltaTime);

            if (_isBroken && Normalized >= _recoveredFromBreakThreshold)
            {
                _isBroken = false;
                Recovered?.Invoke();
            }
        }

        private void OnDestroy()
        {
            Emptied -= OnEmptied;
        }

        public void Raise()
        {
            if (_isBroken)
                return;

            if (_isRaised)
                return;

            _isRaised = true;
            Raised?.Invoke();
        }

        public void Lower()
        {
            if (!_isRaised)
                return;

            _isRaised = false;
            Lowered?.Invoke();
        }

        public void DamageGuard(float amount)
        {
            if (amount <= 0f)
                return;

            Decrease(amount);
            _recoverAtTime = Time.time + _recoveryDelay;
        }

        public void ResetGuard()
        {
            _isRaised = false;
            _isBroken = false;
            ResetToMax();
        }

        private void OnEmptied()
        {
            if (_isBroken)
                return;

            _isBroken = true;
            _isRaised = false;
            Broken?.Invoke();
        }
    }
}