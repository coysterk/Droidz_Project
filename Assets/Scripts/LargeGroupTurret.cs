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


    public float shootTimer = 0.8f;
    float shotTime = 0;

    public int radius;

    public GameObject projectile;
    Transform target;

    public Transform shotSpawn;

    GameObject goal;
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
        GameObject largestGroupEnemy = enemies[0];

        int largestGroup = 0;

        foreach (GameObject zombie in enemies)
        {
            Collider[] colliders = Physics.OverlapSphere(zombie.transform.position, radius);
            int surroundCount = 0;
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Zombie"))
                    surroundCount++;
            }
            if (surroundCount > largestGroup /*&& zombie.GetComponent<Zombie>().lane == priorityLane*/)
                largestGroupEnemy = zombie; Debug.Log(surroundCount);
        }
        return largestGroupEnemy.transform;
    }
}
