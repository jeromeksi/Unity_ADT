
using UnityEngine;
using UnityEngine.AI;

public static class Util
{

    public static bool CheckIsArrived(Vector3 selfPos, NavMeshAgent agent)
    {
        float dist = agent.remainingDistance;
        
        if (dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance < 1 && Vector3.Distance(selfPos,agent.destination)<2
            && !agent.pathPending)
            return true;
        return false;
    }
}

