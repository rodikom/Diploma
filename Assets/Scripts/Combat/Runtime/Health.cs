using System;
using Core.Resources;

namespace Combat.Runtime
{
    public sealed class Health : Resource
    {
        public bool IsDead => IsEmpty;

        public event Action Died;

        protected override void Awake()
        {
            base.Awake();
            Emptied += OnEmptied;
        }

        private void OnDestroy()
        {
            Emptied -= OnEmptied;
        }

        public void TakeDamage(float amount)
        {
            if (IsDead)
                return;

            Decrease(amount);
        }

        public void Kill()
        {
            if (IsDead)
                return;

            ResetToZero();
        }

        public void RestoreHealth(float amount)
        {
            if (IsDead)
                return;

            Increase(amount);
        }

        private void OnEmptied()
        {
            Died?.Invoke();
        }
    }
}