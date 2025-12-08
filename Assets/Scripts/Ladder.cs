using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ladder : MonoBehaviour
{
    public bool IsStacked { get; private set; } = false;
    public int StackIndex { get; private set; } = -1;

    NavMeshAgent nav;
    Transform parentRoot;
    Vector3 stackOffsetPerIndex = new Vector3(0f, 0.9f, 0f); // height per stacked agent
    float moveToStackSpeed = 3f;
    float stackArrivalThreshold = 0.15f;

    void Reset() => nav = GetComponent<NavMeshAgent>();
    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    public void BeginStack(Transform root, int stackIndex)
    {
        if (IsStacked) return;
        StackIndex = stackIndex;
        parentRoot = root;
        StartCoroutine(DoStackRoutine());
    }

    IEnumerator DoStackRoutine()
    {
        // Move to stack base position (center)
        Vector3 targetPos = parentRoot.position + stackOffsetPerIndex * StackIndex;
        nav.enabled = true;
        nav.isStopped = false;
        nav.SetDestination(targetPos);

        while (Vector3.Distance(transform.position, targetPos) > stackArrivalThreshold)
        {
            yield return null;
        }

        // arrived - snap and parent
        nav.isStopped = true;
        nav.enabled = false;

        // parent to root so they move with group
        transform.SetParent(parentRoot, worldPositionStays: true);

        // set exact stacked position / rotation (relative to root)
        transform.localPosition = stackOffsetPerIndex * StackIndex;
        transform.localRotation = Quaternion.identity;

        IsStacked = true;
        yield break;
    }

    public void LeaveStack(Transform root)
    {
        if (!IsStacked) return;
        IsStacked = false;
        StackIndex = -1;
        transform.SetParent(null, true);
        nav.enabled = true;
    }
}
