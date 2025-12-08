using System.Collections.Generic;
using UnityEngine;

public class ZombieGroup
{
    public List<Transform> zombies = new List<Transform>();

    public Vector3 GroupCenter;
    public Vector3 GroupRadii;
    public float AverageDistance;
    public float ExpansionFactor = 1.1f;

    public int CurrentGroupCount;

    public int MaxZombieCount;
    public Transform transform;

    public Transform GroupObject;
    

    // Added for optimization
    private float lastAverageDistance = -1f;
    public float SpreadChangeThreshold = 0.05f; // 5% change

    bool zombieAdded;

    public ZombieGroup(Transform firstZombie)
    {
        zombies = new List<Transform> { firstZombie };
        UpdateGroup();
    }

    public void AddToZombieGroup( Transform T)
    {
        MaxZombieCount++;
        zombies.Add(T);
        zombieAdded = true;
    }

    public void KillZombie(Transform T)
    {
    
    }

    public float ZombiesKilledPercentage()
    {
        return (MaxZombieCount - CurrentGroupCount) / (float)MaxZombieCount;
    }

    public void UpdateGroup()// Should try and call it every frame
    {
        RecalculateCenter();
        RecalculateAverageDistance();
        CurrentGroupCount = zombies.Count;
        // Only recalculates radii when average distance changes enough
        if (ShouldRecalculateRadii()|| zombieAdded)
            RecalculateRadii();
    }

    public void RecalculateCenter()// Pretty obvious as well
    {
        if (zombies.Count == 0) return;

        Vector3 sum = Vector3.zero;
        foreach (Transform z in zombies)
            sum += z.position;

        GroupCenter = sum / zombies.Count;
    }

public void RecalculateRadii()
{
    GroupRadii = GetGroupRadii(zombies, GroupCenter);
    GroupRadii += new Vector3(10,10,10);

    float min = 0.5f; 
    if (GroupRadii.x < min) GroupRadii.x = min;
    if (GroupRadii.y < min) GroupRadii.y = min;
    if (GroupRadii.z < min) GroupRadii.z = min;
}

    public void RecalculateAverageDistance()// Pretty obvious
    {
        if (zombies.Count == 0)
        {
            AverageDistance = 0f;
            return;
        }

        float total = 0f;
        foreach (Transform z in zombies)
            total += Vector3.Distance(z.position, GroupCenter);

        AverageDistance = total / zombies.Count;
    }

    private bool ShouldRecalculateRadii()//used for optimization could allow for some intresting behaviours once we get the single AI's going
    {
        if (lastAverageDistance < 0f)
        {
            lastAverageDistance = AverageDistance;
            return true;
        }

        float change = Mathf.Abs(AverageDistance - lastAverageDistance);

        if (lastAverageDistance > 0f &&
            (change / lastAverageDistance) >= SpreadChangeThreshold)
        {
            lastAverageDistance = AverageDistance;
            return true;
        }

        return false;
    }

Vector3 GetGroupRadii(List<Transform> list, Vector3 center)
{
    zombieAdded = false;
    if (list.Count == 0) 
    {
        Debug.Log("List too small");
        return Vector3.one * 0.5f;
    }

    float rx = 0f, ry = 0f, rz = 0f;

    foreach (var t in list)
    {
        Vector3 offset = t.position - center;
                Debug.Log(offset);
        rx = Mathf.Max(rx, Mathf.Abs(offset.x));
        ry = Mathf.Max(ry, Mathf.Abs(offset.y));
        rz = Mathf.Max(rz, Mathf.Abs(offset.z));
    }
    
    return new Vector3(rx, ry, rz);
}

    public bool InsideGroup(Vector3 pos) // Should be ran on Agents outside the group each frame. Inside the group is technically handlded by the 
    {
        return InsideEllipsoid(pos, GroupCenter, GroupRadii);
    }

    bool InsideEllipsoid(Vector3 point, Vector3 center, Vector3 radii)// Figures out if they are within the bounds of the circle.
    {
        if (radii.x <= 0.01f) radii.x = 0.01f;
        if (radii.y <= 0.01f) radii.y = 0.01f;
        if (radii.z <= 0.01f) radii.z = 0.01f;

        Vector3 p = point - center;

        float value =
            (p.x * p.x) / (radii.x * radii.x) +
            (p.y * p.y) / (radii.y * radii.y) +
            (p.z * p.z) / (radii.z * radii.z);

        return value <= 1f;
    }

    public float AverageDistanceToGoal(Vector3 Goal)
    {
        return Vector3.Distance(GroupCenter, Goal);
    }
}