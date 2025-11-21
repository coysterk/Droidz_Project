using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ClosestTurret : MonoBehaviour
{
        public int health;

    public int maxHealth = 100;

    List<GameObject> enemies;
    List<GameObject> enemiesInRange = new List<GameObject>();

    public float shootTimer = 0.3f;

     float shotTime = 0;

    public GameObject projectile;
    Transform target;

    public Transform shotSpawn;

    public Transform goal;
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Zombie").ToList();
        
        if (Time.frameCount % 3 == 0 || Time.frameCount<3)
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
            enemiesInRange.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Zombie")
        {
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
        /*GameObject closestEnemy = enemies[0];
        foreach(GameObject zombie in enemies)
        {
            if (Vector3.Distance(closestEnemy.transform.position, goal.position) > Vector3.Distance(zombie.transform.position, goal.position))
            {
                closestEnemy = zombie;
            }
        }
        return closestEnemy.transform;*/

         GameObject targetEnemy;
        if(target == null)
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
            Collider[] colliders = Physics.OverlapSphere(zombie.transform.position, 5);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Zombie"))
                zombieScore-=30;
            }
                zombieScore -= (int)Vector3.Distance(zombie.transform.position, goal.position)*3;
            if (zombieScore > hiscore)
            {
                 Debug.Log(zombieScore + " is higher than " + hiscore);
                hiscore = zombieScore;
                targetEnemy = zombie;
            }
        }
        return targetEnemy.transform;
    }
}
