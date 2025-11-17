using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HelperBot : MonoBehaviour
{
    private Transform t;
    private Transform towerTransform;

    private Turret[] turrets;

    List<int> turretHealths = new List<int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        t = gameObject.transform;
        turrets = FindObjectsByType<Turret>(FindObjectsSortMode.None);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckHealth()
    {
        int currentHealth;
        int lowestHealth = 100;
        int lowestTurret = -1;

        for (int i = 0; i < turrets.Length; i++)
        {
            currentHealth = turrets[i].GetHealth();

            if(currentHealth > 0)
            {
                if (currentHealth <= lowestHealth)
                {
                    lowestHealth = currentHealth;
                    lowestTurret = i;
                }
            }

        }

        if (lowestHealth <= 50 && lowestHealth > 0)
        {
            HealTurret(lowestTurret);
        }
    }

    private void HealTurret(int tNum)
    {
        towerTransform = turrets[tNum].gameObject.transform;
        t.LookAt(towerTransform);

    }
}
