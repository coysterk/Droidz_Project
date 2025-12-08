using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MaxHPTurret : Turret
{

    List<GameObject> enemies;
    List<GameObject> enemiesInRange = new List<GameObject>();
    List<GameObject> stack = new List<GameObject>();

    public float shootTimer = 1f;
    private float shotTime = 0;

    public GameObject projectile;


    public Transform shotSpawn;

    public Transform goal;

    public string targettingTechnique = " ";
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
        //target.gameObject.GetComponent<Zombie>().takeDamage(damage);
        GameObject firingProjectile = Instantiate(projectile, shotSpawn.position, shotSpawn.rotation);
        Rigidbody rb = firingProjectile.GetComponent<Rigidbody>();
        rb.AddForce((target.position - shotSpawn.position).normalized * 20,ForceMode.Impulse);
    }
    
    Transform getTarget(List<GameObject> enemies)
    {
        if(priorityTarget != null && Vector3.Distance(transform.position, priorityTarget.position) <= radius)
           {
            targettingTechnique = "priorityTarget";
            if(!priorityTarget.GetComponent<Zombie>().isAttacking)
            {
                priorityTarget = null;
                priorityHp = maxHealth;
                return getTarget(enemies);
            }
            return priorityTarget;
           }
        else if(getDangerousTarget(enemies) != null && getDangerousTarget(enemies).GetComponent<Zombie>().isAttacking)
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

    Transform getNormalTarget(List<GameObject> enemies)
    {
         GameObject targetEnemy;
        if(target == null || Vector3.Distance(target.position, transform.position) > radius)
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

            if(Vector3.Distance(zombie.transform.position, transform.position) > radius * 0.9f)
            {
                float distance = Vector3.Distance(zombie.transform.position, transform.position);
                float percentage = distance / radius;
                zombieScore *= (int)(1+percentage);
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
            if (Vector3.Distance(transform.position, targetEnemy.transform.position) < radius * 0.3f)
            {
                return targetEnemy.transform;
            }
        }
        
        return null;
    }

    //send target to other turrets
   void sendTarget(Transform target)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Turret"))
            {
                Turret turret = hitCollider.GetComponent<LargeGroupTurret>();
                if (turret != this && Vector3.Distance(turret.transform.position, target.position) <= turret.radius && health < turret.priorityHp)
                {
                    turret.priorityHp = health;
                    turret.priorityTarget = target;
                    turret.SendMessage("sendTarget", target);
                }
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

//sets target to random zombie in stack
    Transform targetStack(List<GameObject> enemies)
    {
        GameObject targetEnemy = null;
        while(targetEnemy == null)
        {
        targetEnemy = enemies[Random.Range(0, enemies.Count)];
        }
        return targetEnemy.transform;
    }
}
