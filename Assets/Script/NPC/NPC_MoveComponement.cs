using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPC_MoveComponement : MonoBehaviour
{
    NavMeshAgent agent;
    Vector3 CurrentDestination;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    public Vector3 GetCurrentDestination()
    {
        return CurrentDestination;
    }
    public void SetDestination(Vector3 dest)
    {
        CurrentDestination = dest;
        agent.isStopped = false;
        agent.SetDestination(dest);
    }

    internal IEnumerator CheckArrive()
    {
        while (!Util_tool.CheckIsArrived(this.transform.position, agent))
        {
            yield return new WaitForSeconds(0.1f);
        }
        agent.isStopped = true;
    }
}
