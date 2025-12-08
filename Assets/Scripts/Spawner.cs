using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform StartSpawnPoint;
    public Transform EndSpawnPoint;

    public GameObject EndGoal;

    public GameObject Zombo;

    public float difficulty;

    public int HordeSize;

    public float heavyChance = 0f;           // starts at 0%
    public float timer = 0f;                // tracks elapsed time
    public float timer2 = 10f;

    public float howLongToSpawn = 5f;

    public float intervalSeconds = 120f;     // 2 minutes
    public float increasePerInterval = 5f;   // +5% each interval

    public float Threshold;

    void Update()
    {
        // Accumulate time
        timer += Time.deltaTime;
        timer2+= Time.deltaTime;

        // Check if 2 minutes (or more) have passed
        if (timer >= intervalSeconds)
        {
            int intervals = Mathf.FloorToInt(timer / intervalSeconds);

            heavyChance += intervals * increasePerInterval;

            heavyChance = Mathf.Min(heavyChance, 100f);

            timer -= intervals * intervalSeconds;

        }

        if(timer2 > howLongToSpawn)
        {
            SpawnGroup();
            timer2 -= howLongToSpawn;
        }
    }

    public void SpawnGroup()
    {
        for(int i = 0; i < HordeSize; i++)
        {
            float t = Random.value;
            Vector3 Point = Vector3.Lerp(StartSpawnPoint.position,EndSpawnPoint.position,t);
            GameObject obj = Instantiate(Zombo, Point, Quaternion.identity);
            obj.GetComponent<Zombie>().GoalGetSet = EndGoal;
            obj.GetComponent<Zombie>().HeavyGetSet = TimedChance(heavyChance);
        }
    }


public bool TimedChance(float chance)
{
    return Threshold < chance +Random.value * 0.25f;
}
}
