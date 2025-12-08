using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveOffEachOther", story: "[Agent] moves off other [Agents]", category: "Action", id: "e1333e818a42cff504ecdcaf79c41eb0")]
public partial class MoveOffEachOtherAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<List<Vector3>> Agents;
    [SerializeReference] public BlackboardVariable<Zombie> group;
    [SerializeReference] public BlackboardVariable<float> repulsionRadius;
    [SerializeReference] public BlackboardVariable<float> repulsionStrength;

    private NavMeshAgent nav;
    private Transform agentTrans;
    private ZombieGroup thisGroup;
    private Vector3 desiredPosition;
    private float slotArrivalThreshold = 0.5f;
    protected override Status OnStart()
    {
        nav = Agent.Value.GetComponentInChildren<NavMeshAgent>();
        thisGroup = group.Value.GetGroup();
        agentTrans = Agent.Value.transform;

        repulsionRadius.Value = 0.6f;
        repulsionStrength.Value = 0.5f;

        desiredPosition = group.Value.GetCenter();

        Vector3 repulsion = Vector3.zero;
        Collider[] hits = Physics.OverlapSphere(agentTrans.position, repulsionRadius);
        foreach (var c in hits)
        {
            if (c.transform == agentTrans) continue;
            Zombie other = c.GetComponent<Zombie>();
            if (other == null || other.GetGroup() != thisGroup) continue;
            Vector3 diff = agentTrans.position - other.transform.position;
            float d = diff.magnitude;
            if (d < 0.001f) continue;
            float strength = Mathf.Clamp01((repulsionRadius - d) / repulsionRadius) * repulsionStrength;
            repulsion += diff.normalized * strength;
        }

        Vector3 target = desiredPosition + repulsion;
        nav.isStopped = false;
        nav.SetDestination(target);

        // optional: if close to destination, slow down to stay grouped
        if ((agentTrans.position - desiredPosition).sqrMagnitude <= slotArrivalThreshold * slotArrivalThreshold)
        {
            nav.speed = Mathf.Lerp(nav.speed, 0.6f * nav.speed, 0.5f);
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if ((agentTrans.position - desiredPosition).sqrMagnitude <= slotArrivalThreshold * slotArrivalThreshold)
        {
            nav.speed = Mathf.Lerp(nav.speed, 0.6f * nav.speed, 0.5f);
            return Status.Running;
        }
        if ((agentTrans.position - desiredPosition).sqrMagnitude <= slotArrivalThreshold){
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

