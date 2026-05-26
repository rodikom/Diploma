using Core.Resources;
using UnityEngine;

namespace Combat.Runtime
{
    public sealed class Stamina : Resource
    {
        [SerializeField] private float _recoveryPerSecond = 22f;
        [SerializeField] private float _recoveryDelay = 0.8f;

        private float _recoverAtTime;

        private void Update()
        {
            if (Time.time < _recoverAtTime)
                return;

            if (IsFull)
                return;

            Increase(_recoveryPerSecond * Time.deltaTime);
        }

        public bool TrySpendStamina(float amount)
        {
            if (!TrySpend(amount))
                return false;

            DelayRecovery();
            return true;
        }

        public void DrainStamina(float amount)
        {
            Decrease(amount);
            DelayRecovery();
        }

        private void DelayRecovery()
        {
            _recoverAtTime = Time.time + _recoveryDelay;
        }
    }
}