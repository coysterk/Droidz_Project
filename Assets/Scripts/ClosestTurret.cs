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
        
        if (enemiesInRange.Count > 0)
        {
            target = getTarget(enemiesInRange);
            transform.LookAt(target);
            if (Time.time >= shotTime)
            {
                shoot(target);
                shotTime = Time.time + shootTimer;
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
        GameObject closestEnemy = enemies[0];
        foreach(GameObject zombie in enemies)
        {
            if (Vector3.Distance(closestEnemy.transform.position, goal.position) > Vector3.Distance(zombie.transform.position, goal.position))
            {
                closestEnemy = zombie;
            }
        }
        return closestEnemy.transform;
    }
}
