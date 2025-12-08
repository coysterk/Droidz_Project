using UnityEngine;
using System.Collections.Generic;

public class ZombieAgentManager : MonoBehaviour
{
    public bool LazyTest;
    public List<ZombieGroup> groups = new List<ZombieGroup>();

    public GameObject groupParentPrefab;
    public static ZombieAgentManager Instance;

    public HordeEvaluation HordeEvaluation;

    void Awake() 
    {
         Instance = this;
         //whenever a group is spawned make a new object
         //group middle is made into center of the object
    }
    void Update()
    {
        foreach (var group in groups)
        {
            group.UpdateGroup();
             group.GroupCenter= group.GroupObject.position;
        }
            
    }
    
    public ZombieGroup FindGroupForZombie(Transform zombie)
    {
        float Best = -1;
        ZombieGroup ZG = null;
        foreach (var group in groups)
        {
            if (group.InsideGroup(zombie.position))
            {
            Debug.Log("Adding to "+ group);
                return group;
            }
            float current = HordeEvaluation.GroupUtilityToJoin(group,zombie); 
            if(Best < current)
            {
                ZG = group;
                Best = current;
            }
        }
        Debug.Log("Best" + Best);
        if(ZG != null && Best < .5f)
        {
            return ZG;
        }
        return CreateNewGroup(zombie);
    }

    public ZombieGroup CreateNewGroup(Transform zombie)
    {
        Vector3 zero = Vector3.zero;
        ZombieGroup group = new ZombieGroup(zombie);
        groups.Add(group);
        Debug.Log("Creating new Group");
        GameObject groupParent = Instantiate(groupParentPrefab, zombie.position, Quaternion.identity);
        group.GroupCenter = groupParent.transform.position;
                group.GroupObject = groupParent.transform;
        group.transform.parent = groupParent.transform;

        return group;
    }

void OnDrawGizmos()
{
    if (groups == null) return;

    foreach (var group in groups)
    {
        if (group == null) continue;

        Vector3 center = group.GroupCenter;
        Vector3 radii = group.GroupRadii;

        // Draw ellipsoid
        Gizmos.color = Color.green;
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.identity, radii * 2f);
        Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
        Gizmos.matrix = old;

        // Draw center
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(center, 0.1f);

        // Draw radii lines for clarity
        Gizmos.color = Color.red;
        Gizmos.DrawLine(center, center + new Vector3(radii.x, 0, 0));
        Gizmos.DrawLine(center, center - new Vector3(radii.x, 0, 0));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(center, center + new Vector3(0, radii.y, 0));
        Gizmos.DrawLine(center, center - new Vector3(0, radii.y, 0));

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(center, center + new Vector3(0, 0, radii.z));
        Gizmos.DrawLine(center, center - new Vector3(0, 0, radii.z));
    }
}

//make zombie group child of group parent
    public int FindZombieIndex(ZombieGroup ZG, Transform t)
    {
        for(int i = 0; i < ZG.zombies.Count; i ++)
        {
            if(t = ZG.zombies[i])
            {
                return i;
            }
        }
        return -1;
    }
}