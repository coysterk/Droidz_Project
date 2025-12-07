using UnityEngine;

public class AOEProjectile : MonoBehaviour
{
    public int damage = 5;
    float damageTimer = 0f;
    float damageInterval = 0.1f;

    public int radius = 5;
    Collider[] colliders;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(radius, 0.1f, radius);
        GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.5f);
        colliders = Physics.OverlapSphere(transform.position, radius);
        Destroy(gameObject, 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        //every 0.1 seconds, deal damage to all zombies in range
        if(Time.time >= damageTimer + damageInterval)
        {
            foreach (var collider in colliders)
            {
                if(collider == null) continue;
                if (collider.CompareTag("Zombie"))
                    {
                        collider.GetComponent<Zombie>().health -= damage;
                    }
                    Debug.Log("AOE Projectile damaged " + colliders.Length + " enemies.");
            }
        damageTimer = Time.time;
        }   

    }
}
