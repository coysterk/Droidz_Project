using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    public int damage;

    void Start()
    {
        Destroy(gameObject, 3.0f);       
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Zombie>() != null)
        {
            other.GetComponent<Zombie>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
