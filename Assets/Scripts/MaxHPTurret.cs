using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MaxHPTurret : MonoBehaviour
{
    public int health;

    public int maxHealth = 100;

    List<GameObject> enemies;
    List<GameObject> enemiesInRange;

    public GameObject projectile;
    Transform target;

    GameObject goal;
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(enemiesInRange.Count > 0)
        {
            target = getTarget(enemiesInRange);
            shoot(target);
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

    }
    
    Transform getTarget(List<GameObject> enemies)
    {
        GameObject highestHpEnemy = enemies[0];
        foreach(GameObject zombie in enemies)
        {
            if (highestHpEnemy.GetComponent<Zombie>().health < zombie.GetComponent<Zombie>().health)
            {
                highestHpEnemy = zombie;
            }
        }
        return highestHpEnemy.transform;
    }
}
