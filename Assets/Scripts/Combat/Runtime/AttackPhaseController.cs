using System;
using Combat.Core;
using Combat.Data;
using UnityEngine;

namespace Combat.Runtime
{
    public sealed class AttackPhaseController : MonoBehaviour
    {
        [SerializeField] private AttackPhase _currentPhase = AttackPhase.None;
        [SerializeField] private AttackData _currentAttack;

        private float _phaseStartedAt;
        private float _estimatedImpactTime;

        public AttackPhase CurrentPhase => _currentPhase;
        public AttackData CurrentAttack => _currentAttack;
        public float PhaseElapsedTime => Time.time - _phaseStartedAt;
        public float EstimatedImpactTime => _estimatedImpactTime;
        public float TimeUntilImpact => Mathf.Max(0f, _estimatedImpactTime - Time.time);

        public bool IsAttacking => _currentPhase != AttackPhase.None;
        public bool IsInWindUp => _currentPhase == AttackPhase.WindUp;
        public bool IsActive => _currentPhase == AttackPhase.Active;
        public bool IsRecovering => _currentPhase == AttackPhase.Recovery;
        public bool IsVulnerable => _currentPhase == AttackPhase.Recovery;

        public event Action<AttackPhase, AttackPhase> Changed;
        public event Action<AttackData> AttackStarted;
        public event Action<AttackData> AttackBecameActive;
        public event Action<AttackData> AttackEnteredRecovery;
        public event Action<AttackData> AttackEnded;

        public void BeginAttack(AttackData attackData)
        {
            if (attackData == null)
                return;

            _currentAttack = attackData;
            _estimatedImpactTime = Time.time + attackData.WindUpTime;

            SetPhase(AttackPhase.WindUp);
            AttackStarted?.Invoke(_currentAttack);
        }

        public void SetActive()
        {
            if (_currentAttack == null)
                return;

            SetPhase(AttackPhase.Active);
            AttackBecameActive?.Invoke(_currentAttack);
        }

        public void SetRecovery()
        {
            if (_currentAttack == null)
                return;

            SetPhase(AttackPhase.Recovery);
            AttackEnteredRecovery?.Invoke(_currentAttack);
        }

        public void EndAttack()
        {
            AttackData finishedAttack = _currentAttack;

            _currentAttack = null;
            _estimatedImpactTime = 0f;

            SetPhase(AttackPhase.None);

            if (finishedAttack != null)
                AttackEnded?.Invoke(finishedAttack);
        }

        public void CancelAttack()
        {
            EndAttack();
        }

        private void SetPhase(AttackPhase phase)
        {
            if (_currentPhase == phase)
                return;

            AttackPhase previousPhase = _currentPhase;
            _currentPhase = phase;
            _phaseStartedAt = Time.time;

            Changed?.Invoke(previousPhase, _currentPhase);
        }
    }
}