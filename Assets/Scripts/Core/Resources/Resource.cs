using System;
using UnityEngine;

namespace Core.Resources
{
    public abstract class Resource : MonoBehaviour
    {
        [SerializeField] private float _maxValue = 100f;
        [SerializeField] private float _initialValue = 100f;

        private float _currentValue;

        public float MaxValue => _maxValue;
        public float CurrentValue => _currentValue;
        public float Normalized => _maxValue <= 0f ? 0f : _currentValue / _maxValue;
        public bool IsEmpty => _currentValue <= 0f;
        public bool IsFull => _currentValue >= _maxValue;

        public event Action<float, float> Changed;
        public event Action Emptied;
        public event Action Filled;

        protected virtual void Awake()
        {
            _currentValue = Mathf.Clamp(_initialValue, 0f, _maxValue);
        }

        public bool Has(float amount)
        {
            return amount <= 0f || _currentValue >= amount;
        }

        public bool TrySpend(float amount)
        {
            if (amount <= 0f)
                return true;

            if (!Has(amount))
                return false;

            Decrease(amount);
            return true;
        }

        public void Decrease(float amount)
        {
            if (amount <= 0f)
                return;

            SetValue(_currentValue - amount);
        }

        public void Increase(float amount)
        {
            if (amount <= 0f)
                return;

            SetValue(_currentValue + amount);
        }

        public void ResetToMax()
        {
            SetValue(_maxValue);
        }

        public void ResetToZero()
        {
            SetValue(0f);
        }

        protected void SetValue(float value)
        {
            float previousValue = _currentValue;
            _currentValue = Mathf.Clamp(value, 0f, _maxValue);

            if (Mathf.Approximately(previousValue, _currentValue))
                return;

            Changed?.Invoke(_currentValue, _maxValue);

            if (_currentValue <= 0f && previousValue > 0f)
                Emptied?.Invoke();

            if (_currentValue >= _maxValue && previousValue < _maxValue)
                Filled?.Invoke();
        }
    }
}