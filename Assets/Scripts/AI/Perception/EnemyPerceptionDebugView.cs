using UnityEngine;

namespace AI.Perception
{
    public sealed class EnemyPerceptionDebugView : MonoBehaviour
    {
        [SerializeField] private EnemyPerception _perception;
        [SerializeField] private float _logInterval = 0.25f;

        private float _nextLogTime;

        private void Awake()
        {
            if (_perception == null)
                _perception = GetComponent<EnemyPerception>();
        }

        private void Update()
        {
            if (_perception == null)
                return;

            if (Time.time < _nextLogTime)
                return;

            _nextLogTime = Time.time + _logInterval;

            if (!_perception.HasTarget)
                return;

            if (!_perception.TargetIsAttacking && !_perception.TargetIsRecovering && !_perception.IsThreatened)
                return;

            Debug.Log(
                $"[EnemyPerception] Distance={_perception.PerceivedDistanceToTarget:F2}, TargetAttacking={_perception.TargetIsAttacking}, TargetRecovering={_perception.TargetIsRecovering}, Threatened={_perception.IsThreatened}, Danger={_perception.PerceivedDanger:F2}, TimeUntilImpact={_perception.PerceivedTimeUntilImpact:F2}, Angle={_perception.PerceivedAngle:F1}"
            );
        }
    }
}