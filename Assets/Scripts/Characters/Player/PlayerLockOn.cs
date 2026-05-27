using System;
using Characters.Targeting;
using Core.Services;
using Game.Input;
using UnityEngine;

namespace Characters.Player
{
    public sealed class PlayerLockOn : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _origin;
        [SerializeField] private Transform _cameraTransform;

        [Header("Search")]
        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private float _searchRadius = 18f;
        [SerializeField, Range(0f, 180f)] private float _maxSearchAngle = 80f;
        [SerializeField] private int _maxTargets = 16;

        private IInputService _inputService;
        private Collider[] _hits;
        private LockOnTarget _currentTarget;

        public LockOnTarget CurrentTarget => _currentTarget;
        public bool HasTarget => _currentTarget != null && _currentTarget.IsAvailable;

        public event Action<LockOnTarget> TargetChanged;

        private void Awake()
        {
            if (_origin == null)
                _origin = transform;

            if (_cameraTransform == null && Camera.main != null)
                _cameraTransform = Camera.main.transform;

            _hits = new Collider[_maxTargets];
        }

        private void Start()
        {
            if (!ServiceLocator.TryGet(out _inputService))
            {
                Debug.LogError("[PlayerLockOn] IInputService is not registered. Start the game from Bootstrap scene.");
                enabled = false;
                return;
            }

            _inputService.LockOnPressed += OnLockOnPressed;
        }

        private void Update()
        {
            if (_currentTarget != null && !_currentTarget.IsAvailable)
                ClearTarget();
        }

        private void OnDestroy()
        {
            if (_inputService == null)
                return;

            _inputService.LockOnPressed -= OnLockOnPressed;
        }

        private void OnLockOnPressed()
        {
            if (HasTarget)
            {
                ClearTarget();
                return;
            }

            TryLockOn();
        }

        private void TryLockOn()
        {
            LockOnTarget target = FindBestTarget();

            if (target == null)
                return;

            SetTarget(target);
        }

        private LockOnTarget FindBestTarget()
        {
            int count = Physics.OverlapSphereNonAlloc(
                _origin.position,
                _searchRadius,
                _hits,
                _targetMask,
                QueryTriggerInteraction.Ignore
            );

            LockOnTarget bestTarget = null;
            float bestScore = float.PositiveInfinity;

            Vector3 searchForward = GetSearchForward();

            for (int i = 0; i < count; i++)
            {
                Collider hit = _hits[i];

                if (hit == null)
                    continue;

                LockOnTarget target = hit.GetComponentInParent<LockOnTarget>();

                if (target == null || !target.IsAvailable)
                    continue;

                Vector3 toTarget = target.Position - _origin.position;
                toTarget.y = 0f;

                if (toTarget.sqrMagnitude < 0.0001f)
                    continue;

                float distance = toTarget.magnitude;
                float angle = Vector3.Angle(searchForward, toTarget.normalized);

                if (angle > _maxSearchAngle)
                    continue;

                float score = distance + angle * 0.08f;

                if (score < bestScore)
                {
                    bestScore = score;
                    bestTarget = target;
                }
            }

            return bestTarget;
        }

        private Vector3 GetSearchForward()
        {
            Vector3 forward = _cameraTransform != null ? _cameraTransform.forward : transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.0001f)
                return transform.forward;

            return forward.normalized;
        }

        private void SetTarget(LockOnTarget target)
        {
            _currentTarget = target;
            TargetChanged?.Invoke(_currentTarget);
            Debug.Log($"[PlayerLockOn] Locked on: {_currentTarget.name}");
        }

        private void ClearTarget()
        {
            _currentTarget = null;
            TargetChanged?.Invoke(null);
            Debug.Log("[PlayerLockOn] Lock-on cleared.");
        }
    }
}