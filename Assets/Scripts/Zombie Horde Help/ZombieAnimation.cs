using UnityEngine;

public class ZombieAnimation : MonoBehaviour
{
    Animator anim;
    HordeEvaluation hordeEval;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        hordeEval = GetComponent<HordeEvaluation>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isClimbing", hordeEval.attemptLadderAvailable);
    }
}
