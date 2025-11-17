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
        if (other.CompareTag("Zombie"))
        {
            other.GetComponent<Zombie>().health -= damage;
            Destroy(gameObject);
        }
    }
}
