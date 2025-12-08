using UnityEngine;
using System.Collections.Generic;

public class ZombieAgentManager : MonoBehaviour
{
    public bool LazyTest;
    public List<ZombieGroup> groups = new List<ZombieGroup>();

    public List<GameObject> AllZombies= new List<GameObject>();

    public GameObject ZombiePrefab;
    public static ZombieAgentManager Instance;

    public HordeEvaluation HordeEvaluation;

    public float joinDistance;
    void Awake() 
    {
         Instance = this;
    }
    void Update()
    {
        if(LazyTest)
        {
            LazyTest = false;
            LAZY();
        }
        foreach (var group in groups)
            group.UpdateGroup();
            
    }

    [ContextMenu("DO IT")]
    public void LAZY()
    {
    GameObject temp = Instantiate(ZombiePrefab, new Vector3(0,0,0), Quaternion.identity);
            AllZombies.Add(temp);
        FindGroupForZombie(temp.transform).AddToZombieGroup(temp.transform);
        Debug.Log(groups.Count);
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
            float current =HordeEvaluation.GroupUtilityToJoin(group,zombie); 
            if(Best < current)
            {
                ZG = group;
                Best = current;
            }
        }

        if(ZG != null)
        {
            return ZG;
        }
        return CreateNewGroup(zombie);
    }

    public ZombieGroup CreateNewGroup(Transform zombie)
    {
        ZombieGroup group = new ZombieGroup(zombie);
        groups.Add(group);
        Debug.Log("Creating new Group");
        return group;
    }


    void TryToReturnToGroup()
    {
        
        //Add a route to try and join group Maybe give them extra speed
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

}
