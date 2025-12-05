using UnityEngine;

public class Zombie : MonoBehaviour
{
    public int health;
    public int maxHealth = 100;

    public bool isAttacking;

    public GameObject targettedBy;

    //public string lane = "lane0";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
