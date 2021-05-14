using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC_WorkComponement))]
[RequireComponent(typeof(NPC_MoveComponement))]
public class NPCController : MonoBehaviour
{
    public bool IsWorking;
    private NPC_WorkComponement WorkComponement;
    private NPC_MoveComponement MoveComponement;
    // Start is called before the first frame update
    void Start()
    {
        MoveComponement = GetComponent<NPC_MoveComponement>();
        WorkComponement = GetComponent<NPC_WorkComponement>();
    }
    public void SetDestination(Vector3 dest)
    {
        MoveComponement.SetDestination(dest);
    }
    public bool IsDoMainAssign()
    {
        return WorkComponement.DoMainAssignement;
    }


    internal IEnumerator CheckArrive()
    {
        return MoveComponement.CheckArrive();       

    }

    internal void Assign(Assignement assignementV2)
    {
        WorkComponement.AssignSec(assignementV2);
    }

    internal void Set_MainAssign(Assignement MainAssignement)
    {
        WorkComponement.Set_MainAssign(MainAssignement);
    }

    internal bool Work_HadMainAssing()
    {
        return WorkComponement.HadMainAssing();
    }

    internal int CountSecAssign()
    {
        return WorkComponement.List_Assignement.Count;
    }
}
