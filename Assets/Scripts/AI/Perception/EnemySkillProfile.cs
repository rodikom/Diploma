using UnityEngine;

namespace AI.Perception
{
    [CreateAssetMenu(menuName = "AI/Enemy Skill Profile")]
    public sealed class EnemySkillProfile : ScriptableObject
    {
        [Header("Perception")]
        [SerializeField, Min(0f)] private float _reactionDelay = 0.25f;
        [SerializeField, Min(0.01f)] private float _senseInterval = 0.08f;

        [Header("Error")]
        [SerializeField, Range(0f, 1f)] private float _dangerNoise = 0.15f;
        [SerializeField, Min(0f)] private float _timingError = 0.12f;
        [SerializeField, Min(0f)] private float _distanceError = 0.15f;
        [SerializeField, Min(0f)] private float _angleError = 4f;

        [Header("Decision")]
        [SerializeField, Range(0f, 1f)] private float _decisionNoise = 0.1f;

        public float ReactionDelay => _reactionDelay;
        public float SenseInterval => _senseInterval;
        public float DangerNoise => _dangerNoise;
        public float TimingError => _timingError;
        public float DistanceError => _distanceError;
        public float AngleError => _angleError;
        public float DecisionNoise => _decisionNoise;
    }
}