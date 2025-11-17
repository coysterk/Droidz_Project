using UnityEngine;

public class Turret : MonoBehaviour
{
    private Transform t;

    public int health = 100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        t = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetHealth()
    {
        return health;
    }

    public void RestoreHealth()
    {
        health += 5;
    }

}
