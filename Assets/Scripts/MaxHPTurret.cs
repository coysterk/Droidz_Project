using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MaxHPTurret : MonoBehaviour
{
    public int health;

    public int maxHealth = 100;

    List<GameObject> enemies;
    List<GameObject> enemiesInRange = new List<GameObject>();

    public float shootTimer = 1f;
    private float shotTime = 0;

    public GameObject projectile;
    Transform target;

    public Transform shotSpawn;

    public Transform goal;
    string priorityLane;
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Zombie").ToList();

        //find the lane with the most zombies and make it priority
        /*int lane1count = 0, lane2count = 0, lane3count = 0;
        foreach (GameObject zombie in enemies)
        {
            if (zombie.GetComponent<Zombie>().lane == "lane1")
                lane1count++;
            else if (zombie.GetComponent<Zombie>().lane == "lane2")
                lane2count++;
            else if (zombie.GetComponent<Zombie>().lane == "lane3")
                lane3count++;
        }
        if (lane1count >= lane2count && lane1count >= lane3count)
            priorityLane = "lane1";
        else if (lane2count >= lane1count && lane2count >= lane3count)
            priorityLane = "lane2";
        else
            priorityLane = "lane3";
        */
        if (Time.frameCount % 20 == 0 || Time.frameCount<20)
        {
            if (enemiesInRange.Count > 0)
            {
                target = getTarget(enemiesInRange);
            }
        }
        if(target!= null){
        if (enemiesInRange.Count > 0)
            {
                transform.LookAt(target);
                if (Time.time >= shotTime)
                {
                    shoot(target);
                    shotTime = Time.time + shootTimer;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Zombie")
        {
            Debug.Log("zombie entered");
            enemiesInRange.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Zombie")
        {
            Debug.Log("zombie left");
            enemiesInRange.Remove(other.gameObject);
        }
    }

    void shoot(Transform target)
    {
        GameObject firingProjectile = Instantiate(projectile, shotSpawn.position, shotSpawn.rotation);
        Rigidbody rb = firingProjectile.GetComponent<Rigidbody>();
        rb.AddForce((target.position - shotSpawn.position).normalized * 20,ForceMode.Impulse);
    }
    
    Transform getTarget(List<GameObject> enemies)
    {

        GameObject targetEnemy;
        if(target == null)
       { 
        targetEnemy = enemies[0];
       }
        else
        {
            targetEnemy = target.GameObject();
        }

        int hiscore = 0;
        foreach(GameObject zombie in enemies)
        {
            if(zombie == null) continue;
            int zombieScore = 0;
            if(target!=null)
            {
            if (zombie == target.GameObject())
            zombieScore += 50;
            }
            if (zombie.GetComponent<Zombie>().health > targetEnemy.GetComponent<Zombie>().health)
            zombieScore += zombie.GetComponent<Zombie>().health - targetEnemy.GetComponent<Zombie>().health;
            if (zombie.GetComponent<Zombie>().isAttacking)
            {
                zombieScore+= 25;
            }
            Collider[] colliders = Physics.OverlapSphere(zombie.transform.position, 5);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Zombie"))
                zombieScore+=10;
            }
                zombieScore -= (int)Vector3.Distance(zombie.transform.position, goal.position);

            if(zombie.GetComponent<Zombie>().targettedBy != gameObject && zombie.GetComponent<Zombie>().targettedBy != null)
                    {
                        zombieScore -= 100;
                    }
            if (zombieScore > hiscore)
            {
                 Debug.Log(zombieScore + " is higher than " + hiscore);
                hiscore = zombieScore;
                targetEnemy = zombie;
            }
        }
        if(targetEnemy != null)
        {
            targetEnemy.GameObject().GetComponent<Zombie>().targettedBy = gameObject;
                foreach(GameObject enemy in enemiesInRange)
                {
                    if(enemy == null) continue;
                    if(enemy.GetComponent<Zombie>().targettedBy == gameObject && enemy != targetEnemy)
                    {
                        enemy.GetComponent<Zombie>().targettedBy = null;
                    }
                }
        }
        if(targetEnemy != null)
        {
            return targetEnemy.transform;
        }
        return null;
    }
}
