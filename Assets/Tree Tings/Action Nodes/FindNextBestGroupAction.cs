using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindNextBestGroup", story: "Agent finds next group", category: "Action", id: "b1f75584c56037a5e856b37fcb3c14a6")]
public partial class FindNextBestGroupAction : Action
{
    [SerializeReference] public BlackboardVariable<List<float>> Values = new BlackboardVariable<List<float>>();
    [SerializeReference] public BlackboardVariable<Transform> target;
    [SerializeReference] public BlackboardVariable<ZombieAgentManager> zom;

    private List<float> m_Values;
    private float temp;
    protected override Status OnStart()
    {
        m_Values = Values;
        if(m_Values.Count <= 0)
        {
            return Status.Failure; //no utils
        }
        foreach (float x in m_Values)
        {
            if(x > temp)
            {
                temp = x;
            }
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

