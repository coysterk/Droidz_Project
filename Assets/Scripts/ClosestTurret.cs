using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ClosestTurret : Turret
{

    List<GameObject> enemies;
    List<GameObject> enemiesInRange = new List<GameObject>();
    List<GameObject> stack = new List<GameObject>();

    public float shootTimer = 0.3f;
    public float damage;

     float shotTime = 0;
    public string targettingTechnique;

    public GameObject projectile;
    

    public Transform shotSpawn;

    public Transform goal;
    void Start()
    {
        health = maxHealth;
        priorityHp = maxHealth;
        
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
        //every 20 frames, update target
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
        if(priorityTarget != null  && Vector3.Distance(transform.position, priorityTarget.position) <= radius)
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

        int hiscore = -100;
        foreach(GameObject zombie in enemies)
        {
            if(zombie == null) continue;
            int zombieScore = 0;
            if(target!=null)
            {
            if (zombie == target.GameObject())
            zombieScore -= 10;
            }
            if (zombie.GetComponent<Zombie>().isAttacking)
            {
                zombieScore=-5;
            }
                zombieScore -= (int)Vector3.Distance(zombie.transform.position, goal.position)*5;
                if(Vector3.Distance(zombie.transform.position, transform.position) > radius * 0.9f)
            {
                float distance = Vector3.Distance(zombie.transform.position, transform.position);
                float percentage = distance / radius;
                zombieScore += (int)(percentage * 25);
            }
            if (zombieScore > hiscore)
            {
                hiscore = zombieScore;
                targetEnemy = zombie;
            }
        }
        if(targetEnemy != null)
        {
            //print target distance from goal
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

//sets target to highest zombie in stack
    Transform targetStack(List<GameObject> enemies)
    {
        GameObject targetEnemy = enemies[0];
        foreach(GameObject zombie in enemies)
        {
            //get lowest zomgies
            if(zombie == null) continue;
            if(zombie.transform.position.y < targetEnemy.transform.position.y)
            {
                targetEnemy = zombie;
            }
            //if heights are equal, choose closer one
            else if(zombie.transform.position.y == targetEnemy.transform.position.y)
            {
                if(Vector3.Distance(zombie.transform.position, transform.position) < Vector3.Distance(targetEnemy.transform.position, transform.position))
                {
                    targetEnemy = zombie;
                }
            }
        }
        return targetEnemy.transform;
    }
}
