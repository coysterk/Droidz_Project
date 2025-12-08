using UnityEngine;
using System.Collections.Generic;

public class HelperBot : MonoBehaviour
{
    Rigidbody rb;
    bool healing;

    GameObject healingTarget = null;

    List<GameObject> turrets;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        turrets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Turret"));
        healingTarget = null;
    }

    // Update is called once per frame
    void Update()
    {
        turrets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Turret"));
        //print turrets
        foreach(GameObject turret in turrets)
        {
            if(turret == null) continue;
            Turret turretScript = turret.GetComponent<Turret>();
        }
        List<GameObject> dangerOne = new List<GameObject>();
        List<GameObject> dangerTwo = new List<GameObject>();
        List<GameObject> dangerThree = new List<GameObject>();
        List<GameObject> dangerFour = new List<GameObject>();
        if(healingTarget==null)
        {
            foreach(GameObject turret in turrets)
            {
                if(turret == null) continue;
                Turret turretScript = turret.GetComponent<Turret>();
                if(turretScript.health <= turretScript.maxHealth * 0.25f)
                {
                    turretScript.danger = 4;
                    dangerFour.Add(turret);
                }
                else if(turretScript.health <= turretScript.maxHealth * 0.5f)
                {
                    turretScript.danger = 3;
                    dangerThree.Add(turret);
                }
                else if(turretScript.health <= turretScript.maxHealth * 0.75f)
                {
                    turretScript.danger = 2;
                    dangerTwo.Add(turret);
                }
                else if(turretScript.health < turretScript.maxHealth)
                {
                    turretScript.danger = 1;
                    dangerOne.Add(turret);
                }
                //visit turrets based off of danger level and distance
                if(dangerFour.Count > 0)
                {
                    dangerFour.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));
                    healingTarget = dangerFour[0];
                    break;
                }
                else if(dangerThree.Count > 0)
                {
                    dangerThree.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));
                    healingTarget = dangerThree[0];
                    break;
                }
                else if(dangerTwo.Count > 0)
                {
                    dangerTwo.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));
                    healingTarget = dangerTwo[0];
                    break;
                }
                else if(dangerOne.Count > 0)
                {
                    dangerOne.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));
                    healingTarget = dangerOne[0];
                    break;
                }
            }
        }
        if(healingTarget != null && !healing)
        {
            Vector3 direction = (healingTarget.transform.position - transform.position).normalized;
            rb.MovePosition(transform.position + direction * Time.deltaTime * 5);
            if(Vector3.Distance(transform.position, healingTarget.transform.position) < 2f && !healing)
            {
                healing = true;
                StartCoroutine(HealTurret(healingTarget));
                healingTarget = null;
            }
        }
    }

    System.Collections.IEnumerator HealTurret(GameObject turret)
    {
        Turret turretScript = turret.GetComponent<Turret>();
        if(turretScript != null)
        {
            while(turretScript.health < turretScript.maxHealth)
            {
                turretScript.health += 5;
                if(turretScript.health > turretScript.maxHealth)
                {
                    turretScript.health = turretScript.maxHealth;
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
        healingTarget = null;
        healing = false;
    }
    
}
