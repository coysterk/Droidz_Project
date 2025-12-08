using UnityEngine;

public class Turret : MonoBehaviour
{

    public int health;

    public int maxHealth = 100;

    public int danger=0;
    public int priorityHp;

    public int radius;

    public Transform target, priorityTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void sendTarget(Transform target, int health)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Turret"))
            {
                Turret turret = hitCollider.GetComponent<Turret>();
                if (turret != this && Vector3.Distance(turret.transform.position, target.position) <= turret.radius && health < turret.priorityHp)
                {
                    turret.priorityHp = health;
                    turret.priorityTarget = target;
                    turret.sendTarget(target, health);
                }
            }
        }
    }
}
