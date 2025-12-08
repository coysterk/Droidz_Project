using UnityEngine;

public class AOEProjectile : MonoBehaviour
{
    public int damage = 5;
    float damageTimer = 0f;
    float damageInterval = 0.1f;

    public int radius = 30;
    Collider[] colliders;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(radius, 0.1f, radius);
        GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.3f);
        GetComponent<Renderer>().material.renderQueue = 3000;
        colliders = Physics.OverlapSphere(transform.position, radius);
        Destroy(gameObject, 0.4f);
    }

    // Update is called once per frame
    void Update()
    {

        if(Time.time >= damageTimer + damageInterval)
        {
            foreach (var collider in colliders)
            {
                if(collider == null) continue;
                if (collider.CompareTag("Zombie"))
                    {
                        collider.GetComponent<Zombie>().TakeDamage(damage);
                    }
                    Debug.Log("AOE Projectile damaged " + colliders.Length + " enemies.");
            }
        damageTimer = Time.time;
        }
    }
}
