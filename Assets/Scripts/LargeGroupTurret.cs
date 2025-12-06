using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LargeGroupTurret : MonoBehaviour
{
    public int health;

    public int maxHealth = 100;

    List<GameObject> enemies;
    List<GameObject> enemiesInRange = new List<GameObject>();

    List<GameObject> stack = new List<GameObject>();


    public float shootTimer = 0.8f;
    float shotTime = 0;

    public int radius;
    string targettingTechnique;

    public GameObject projectile;
    public Transform target, priorityTarget;

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

        enemiesInRange.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        Debug.DrawRay(transform.position, Vector3.up * radius, Color.red);
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Zombie"))
            {
                enemiesInRange.Add(hitCollider.gameObject);
            }
        }

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



    void shoot(Transform target)
    {
        GameObject firingProjectile = Instantiate(projectile, shotSpawn.position, shotSpawn.rotation);
        Rigidbody rb = firingProjectile.GetComponent<Rigidbody>();
        rb.AddForce((target.position - shotSpawn.position).normalized * 20,ForceMode.Impulse);
    }
    
    Transform getTarget(List<GameObject> enemies)
    {      
        if(priorityTarget != null && Vector3.Distance(transform.position, priorityTarget.position) <= radius)
           {
            targettingTechnique = "priorityTarget";
            return priorityTarget;
           }
        else if(getDangerousTarget(enemies)!= null && getDangerousTarget(enemies).GetComponent<Zombie>().isAttacking)
           {
            targettingTechnique = "dangerousTarget";
            sendTarget(getDangerousTarget(enemies));
            return getDangerousTarget(enemies);
           }
        else if(getStackPriority(stack) >= 5)
            {
            targettingTechnique = "stackPriority";
            return targetStack(enemies);
            }
        else
           {
            targettingTechnique = "normalTarget";
            return getNormalTarget(enemies);
           }
    }

   
    //when not targetting priority, dangerous, or stack, get most valuable target
    Transform getNormalTarget(List<GameObject> enemies)
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
            zombieScore += 100;
            }
            if (zombie.GetComponent<Zombie>().isAttacking)
            {
                zombieScore+= 30;
            }
            Collider[] colliders = Physics.OverlapSphere(zombie.transform.position, 5);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Zombie"))
                {
                    zombieScore += 30;
                zombieScore+=collider.GameObject().GetComponent<Zombie>().health/2;
                }
            }
                zombieScore -= (int)Vector3.Distance(zombie.transform.position, goal.position)/5;

            if(zombie.GetComponent<Zombie>().targettedBy != gameObject && zombie.GetComponent<Zombie>().targettedBy != null)
                    {
                        zombieScore -= 1000;
                    }
            if (zombieScore > hiscore)
            {
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

//get most dangerous target (closest to and attacking the turret)
    Transform getDangerousTarget(List<GameObject> enemies)
    {
        GameObject targetEnemy;

        //get enemy closest to this game object 
        targetEnemy = enemies[0];
        float closestDistance = Vector3.Distance(transform.position, targetEnemy.transform.position);
        foreach (GameObject zombie in enemies)
        {   if(zombie == null) continue;
            float distance = Vector3.Distance(transform.position, zombie.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetEnemy = zombie;
            }
        }
        if(targetEnemy != null)
        {
            if (Vector3.Distance(transform.position, targetEnemy.transform.position) < radius * 0.5f)
            {
                return targetEnemy.transform;
            }
        }
        return null;
    }

    //send target to other turrets
    void sendTarget(Transform target)
    {
        MaxHPTurret[] turrets = Object.FindObjectsByType<MaxHPTurret>(FindObjectsSortMode.None);
        ClosestTurret[] closeTurrets = Object.FindObjectsByType<ClosestTurret>(FindObjectsSortMode.None);
        foreach(ClosestTurret turret in closeTurrets)
        {
            if(turret != this)
            {
                turret.priorityTarget = target;
            }
        }
        foreach(MaxHPTurret turret in turrets)
        {
            if(turret != this)
            {
                turret.priorityTarget = target;
            }
        }
    }

//calculates stack priority
    float getStackPriority(List<GameObject> enemies)
    {
        int priority = 0;
        if (enemies.Count < 2)
        {
            return 0;
        }
        foreach(GameObject z in enemies)
        {
            priority++;
        }
        if(Vector3.Distance(enemies[enemies.Count-1].transform.position, transform.position) > radius * 0.7f)
        {
            float distance = Vector3.Distance(enemies[enemies.Count-1].transform.position, transform.position);
            float percentage = distance / radius;
            priority += (int)(percentage * 5);
        }
        return priority;
    }

//sets target to highest zombie in stack
    Transform targetStack(List<GameObject> enemies)
    {
        GameObject targetEnemy = enemies[0];
        foreach(GameObject zombie in enemies)
        {
            if(zombie == null) continue;
            if(zombie.transform.position.y > targetEnemy.transform.position.y)
            {
                targetEnemy = zombie;
            }
        }
        return targetEnemy.transform;
    }


}
