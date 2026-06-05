using AI.Perception;
using AI.Utility;
using Combat.Runtime;
using UnityEngine;

namespace Characters.Enemy.AI
{
    public sealed class EnemyUtilityBrain : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CombatActor _actor;
        [SerializeField] private EnemyPerception _perception;
        [SerializeField] private EnemyMotor _motor;
        [SerializeField] private EnemyAttackController _attack;
        
        [Header("Decision")]
        [SerializeField] private UtilityAction[] _actions;
        [SerializeField, Min(0.01f)] private float _thinkInterval = 0.12f;
        [SerializeField, Range(0f, 1f)] private float _switchThreshold = 0.05f;

        [Header("Debug")]
        [SerializeField] private bool _showDebug;

        private UtilityAIContext _context;
        private UtilityAction _currentAction;
        private float _currentScore;
        private float _nextThinkTime;

        private void Awake()
        {
            if (_actor == null)
                _actor = GetComponent<CombatActor>();

            if (_perception == null)
                _perception = GetComponent<EnemyPerception>();
            
            if (_motor == null)
                _motor = GetComponent<EnemyMotor>();

            if (_attack == null)
                _attack = GetComponent<EnemyAttackController>();

            _context = new UtilityAIContext(_actor, _perception, _motor, _attack);
        }

        private void Update()
        {
            if (_actor == null || !_actor.IsAlive)
                return;

            if (Time.time >= _nextThinkTime)
            {
                _nextThinkTime = Time.time + _thinkInterval;
                Think();
            }

            _currentAction?.Execute(_context);
        }

        private void Think()
        {
            if (_currentAction != null &&
                !_currentAction.CanBeInterrupted &&
                !_currentAction.IsFinished(_context))
                return;

            UtilityAction bestAction = null;
            float bestScore = float.MinValue;

            for (int i = 0; i < _actions.Length; i++)
            {
                UtilityAction action = _actions[i];

                if (action == null)
                    continue;

                if (!action.CanExecute(_context))
                    continue;

                float score = Mathf.Clamp01(action.Evaluate(_context));
                score = ApplyDecisionNoise(score);

                if (score < action.MinimumScoreToRun)
                    continue;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestAction = action;
                }
            }

            if (bestAction == null)
                return;

            bool shouldSwitch = _currentAction == null ||
                                _currentAction.IsFinished(_context) ||
                                bestScore > _currentScore + _switchThreshold ||
                                bestAction == _currentAction;

            if (!shouldSwitch)
                return;

            if (bestAction != _currentAction)
            {
                _currentAction?.OnExit(_context);
                _currentAction = bestAction;
                _currentAction.OnEnter(_context);
            }

            _currentScore = bestScore;

            if (_showDebug)
                Debug.Log($"[EnemyUtilityBrain] Action={_currentAction.name}, Score={_currentScore:F2}");
        }

        private float ApplyDecisionNoise(float score)
        {
            if (_perception == null || _perception.SkillProfile == null)
                return score;

            return UtilityScoreMath.ApplyNoise(score, _perception.SkillProfile.DecisionNoise);
        }
    }
}