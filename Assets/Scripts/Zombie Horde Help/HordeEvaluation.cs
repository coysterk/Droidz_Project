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

    public Transform Goal;// probally needs to be changed out

    public float EndDistance = 10f;


    public void EvaluateLadder()
    {
        attemptLadderAvailable = (groupPercentageSurvival < 50f || averageDistance > 7f);
    }

    public void CalculateFitness()
    {
        EvaluateLadder();
        float survivalScore = Mathf.Clamp01(groupPercentageSurvival / 100f);
        float distanceScore = Mathf.Clamp01(1f - (averageDistance / EndDistance));
        float ladderScore = ladderActive ? 1f : (attemptLadderAvailable ? -1f : 0f);
        fitness = survivalScore * wSurvival + distanceScore * wDistance + ladderScore * wLadder;
    }

public float GroupFailureUtility(ZombieGroup ZG)
{
    // penalty for not reaching the goal
    float distanceFailure = 1f - Mathf.Clamp01(ZG.AverageDistanceToGoal(Goal.position));

    // penalty for many deaths
    float deathPenalty = 1f - Mathf.Clamp01(ZG.ZombiesKilledPercentage());

    //
    float ladderPenalty = 0f;
    if (!ladderActive && attemptLadderAvailable)
        ladderPenalty = 1f;

    float failure =
        (distanceFailure * 0.5f) +
        (deathPenalty * 0.3f) +
        (ladderPenalty * 0.2f);

    return -failure;
}

public float GroupUtility(ZombieGroup ZG)
{

    // reward going far toward the goal
    float progressReward = Mathf.Clamp01(ZG.AverageDistanceToGoal(Goal.position));

    // reward keeping many alive
    float survivalReward = Mathf.Clamp01(ZG.ZombiesKilledPercentage());

    // combine weights
    float utility =
        (progressReward * 0.8f) + 
        (survivalReward * 0.2f);

    return utility;
}

public float GroupUtilityToJoin(ZombieGroup ZG, Transform T)
    {
        float util = GroupUtility(ZG);
        util*=.75f;
    float dist = Vector3.Distance(ZG.GroupCenter, T.position);
    float DistanceReward = 1f - Mathf.Clamp01(dist / 10f);
        DistanceReward*=.25f;
        return util+DistanceReward;

    }
}