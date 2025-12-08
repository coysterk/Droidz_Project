using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class ClosestTurret : Turret
{

    List<GameObject> enemies;
    List<GameObject> enemiesInRange = new List<GameObject>();
    List<GameObject> stackParent = new List<GameObject>();
    List<GameObject> stack = new List<GameObject>();

    public float shootTimer = 0.3f;
    float edgeMultiplier = 1f;
    float decreaseTimer = 0f;
    public float damage;

     float shotTime = 0;
    public string targettingTechnique;

    public GameObject projectile;
    public Transform gun;
    

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
        List<GameObject> oldEnemiesInRange = new List<GameObject>(enemiesInRange);
        oldEnemiesInRange.RemoveAll(item => item == null);

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
        if(enemiesInRange.Count < oldEnemiesInRange.Count)
        {
            edgeMultiplier += (oldEnemiesInRange.Count - enemiesInRange.Count) * 0.5f;
            decreaseTimer = Time.time;
        }
        else if(Time.time >= decreaseTimer + 2f && edgeMultiplier > 1f)
        {
            edgeMultiplier -= 0.5f;
            if(edgeMultiplier < 1f)
            {
                edgeMultiplier = 1f;
            }
            decreaseTimer = Time.time;
        }

        
        stackParent.Clear();
        foreach(GameObject enemy in enemiesInRange)
        {
            if(enemy == null) continue;
            if(enemy.transform.childCount > 0)
            {
                foreach(Transform child in enemy.transform)
                {
                    if(child.GetComponent<Zombie>() != null)
                    {
                        if(!stackParent.Contains(enemy) && enemy.transform.parent == null)
                        {
                            stackParent.Add(enemy);
                        }
                    }
                }
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
                gun.LookAt(target);
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
        if(priorityTarget != null && getDangerousTarget(enemies) == null)
           {
            targettingTechnique = "priorityTarget";
            if(!priorityTarget.GetComponent<Zombie>().isAttacking || Vector3.Distance(transform.position, priorityTarget.position) > radius)
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
            sendTarget(getDangerousTarget(enemies), health);
            return getDangerousTarget(enemies);
           }
        else
        {
            float highestPriority = 0;
            GameObject highestPriorityParent = null;
            foreach(GameObject parent in stackParent)
            {
                float stackPriority = getStackPriority(getStack(parent));
                if(stackPriority > highestPriority)
                {
                    highestPriority = stackPriority;
                    highestPriorityParent = parent;
                }
            }
            if(highestPriorityParent != null && highestPriority >= 3)
            {
                targettingTechnique = "stackTarget";
                return targetStack(getStack(highestPriorityParent));
            }
            else
            {
                targettingTechnique = "normalTarget";
                return getNormalTarget(enemies);
            }
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
                zombieScore += (int)(percentage * (25 * edgeMultiplier));
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


void getStackRecursive(Transform parent, List<GameObject> stack)
    {
        foreach(Transform child in parent)
        {
            if(child.GetComponent<Zombie>() != null)
            {
                stack.Add(child.gameObject);
            }
            if(child.childCount > 0)
            {
                getStackRecursive(child, stack);
            }
        }
    }

    List<GameObject> getStack(GameObject parent)
    {
        List<GameObject> stack = new List<GameObject>();
        stack.Add(parent);
        getStackRecursive(parent.transform, stack);
        Debug.Log("Stack size: " + stack.Count);
        return stack;
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
        //target random enemy in stack
        GameObject targetEnemy = enemies[Random.Range(0, enemies.Count)];
        return targetEnemy.transform;
    }
}
