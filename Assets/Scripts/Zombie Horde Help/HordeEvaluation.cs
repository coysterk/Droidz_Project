using UnityEngine;

public class HordeEvaluation : MonoBehaviour
{
    public float groupPercentageSurvival = 0f;
    public float averageDistance = 0f;
    public bool ladderActive = false;
    public bool attemptLadderAvailable = false;
    public float wSurvival = 0.5f;
    public float wDistance = 0.3f;
    public float wLadder = 0.2f;
    public float fitness = 0f;

    public void EvaluateLadder()
    {
        attemptLadderAvailable = (groupPercentageSurvival < 50f || averageDistance > 7f);
    }

    public void CalculateFitness()
    {
        EvaluateLadder();
        float survivalScore = Mathf.Clamp01(groupPercentageSurvival / 100f);
        float maxDistance = 10f;
        float distanceScore = Mathf.Clamp01(1f - (averageDistance / maxDistance));
        float ladderScore = ladderActive ? 1f : (attemptLadderAvailable ? -1f : 0f);
        fitness = survivalScore * wSurvival + distanceScore * wDistance + ladderScore * wLadder;
    }
}