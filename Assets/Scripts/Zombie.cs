using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [Header("Personal Variables")]
    [SerializeField] float Health;
    [SerializeField] GameObject Goal;
    [SerializeField] GameObject regularHordeModel;
    [SerializeField] GameObject heavyHordeModel;
    [SerializeField] bool isHeavy;
    private ZombieGroup group;
    private ZombieAgentManager agentManager;

    void Start()
    {
        agentManager = GetComponent<ZombieAgentManager>();
        group = agentManager.FindGroupForZombie(this.transform);
        
        if (isHeavy)
        {
            regularHordeModel.SetActive(false);
            heavyHordeModel.SetActive(true);
            Health = 1000f;
        } 
        else
        {
            heavyHordeModel.SetActive(false);
            regularHordeModel.SetActive(true);
            Health = 100f;
        }
    }

    //Keep the update simple here, to help performance we dont want each zombie
    //to run a complex update on top of a tree
    void Update()
    {
        if (Health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void TakeDamage(float dmg)
    {
        Health -= dmg;
    }

    public bool InsideGroup(Transform zom)
    {
        if (group.InsideGroup(this.transform.position))
        {
            return true;
        }
        else { return false; }
    }

    public Vector3 GetCenter()
    {
        return group.GroupCenter;
    }
}
