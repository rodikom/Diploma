using UnityEngine;

namespace AI.Utility
{
    public static class UtilityScoreMath
    {
        public static float MultiplyConsiderations(UtilityAIContext context, UtilityConsideration[] considerations)
        {
            if (considerations == null || considerations.Length == 0)
                return 0f;

            float score = 1f;
            bool hasValidConsideration = false;

            for (int i = 0; i < considerations.Length; i++)
            {
                UtilityConsideration consideration = considerations[i];

                if (consideration == null)
                    continue;

                score *= consideration.EvaluateWeighted(context);
                hasValidConsideration = true;
            }

            return hasValidConsideration ? Mathf.Clamp01(score) : 0f;
        }

        public static float AverageConsiderations(UtilityAIContext context, UtilityConsideration[] considerations)
        {
            if (considerations == null || considerations.Length == 0)
                return 0f;

            float sum = 0f;
            int count = 0;

            for (int i = 0; i < considerations.Length; i++)
            {
                UtilityConsideration consideration = considerations[i];

                if (consideration == null)
                    continue;

                sum += consideration.EvaluateWeighted(context);
                count++;
            }

            return count > 0 ? Mathf.Clamp01(sum / count) : 0f;
        }

        public static float ApplyNoise(float score, float noise)
        {
            if (noise <= 0f)
                return Mathf.Clamp01(score);

            return Mathf.Clamp01(score + Random.Range(-noise, noise));
        }
    }
}