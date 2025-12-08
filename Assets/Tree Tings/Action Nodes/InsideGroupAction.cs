using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "InsideGroup", story: "[Self] is inside [group]", category: "Action", id: "6281eca8cafbe049ed4a9b3251272a32")]
public partial class InsideGroupAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Zombie> zom;
    [SerializeReference] public BlackboardVariable<bool> hasGroup;

    private NavMeshAgent m_nav;
    private Vector3 target;
    protected override Status OnStart()
    {
        m_nav = Self.Value.GetComponentInChildren<NavMeshAgent>();
        target = zom.Value.GetCenter();
        m_nav.SetDestination(target);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (zom.Value.InsideGroup(Self.Value.transform))
        {
            hasGroup.Value = true;
            return Status.Success;
        }
        else
        {
            m_nav.SetDestination(target);
            return Status.Running;
        }
    }

    protected override void OnEnd()
    {
        m_nav.ResetPath();
    }
}

