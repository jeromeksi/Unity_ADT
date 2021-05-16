using Assets.Script.Batiment.Assignement_Work;
using Batiment.BatimentProduction;
using Batiment.BatimentProduction.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

public class NPC_WorkComponement : MonoBehaviour
{
    public List<Assignement> List_Assignement = new List<Assignement>();
    private Assignement MainAssignement;
    public Assignement CurrentAssignement;
    private NPCController Controller;
    public bool DoMainAssignement;
    public bool DoSecAssignement;
    public int WorkMoney;
    public List<ItemAmount> List_StockItemWork = new List<ItemAmount>();
    private void Awake()
    {
        Controller = GetComponent<NPCController>();
    }
       
    public void Update()
    {
        if (List_Assignement.Count > 0 && !DoSecAssignement)
        {
            DoMainAssignement = false;

            var assT = List_Assignement.First();
            if (assT != null)
            {
                CurrentAssignement = assT;
                StartCoroutine( SecAssignementRoutine(assT));
                
                DoSecAssignement = true;
            }
        }
        else
        {
            if (!DoSecAssignement && !DoMainAssignement && MainAssignement != null)
            {
                DoMainAssignement = true;
                StartCoroutine(MainAssignementRoutine());
            }
        }
    }

    private IEnumerator MainAssignementRoutine()
    {
        //if(MainAssignement.BatimentProduction != null)
        //{
        //    Controller.SetDestination(MainAssignement.BatimentProduction.gameObject.transform.position);
        //    yield return Controller.CheckArrive();
        //}
        yield return MainAssignement.DoWork();
    }

    internal bool HadMainAssing()
    {
        return MainAssignement != null;
    }

    internal void AssignSec(Assignement assignementV2)
    {
        List_Assignement.Add(assignementV2);
    }

    private IEnumerator SecAssignementRoutine(Assignement w)
    {
        DoSecAssignement = true;
        DoMainAssignement = false;
        yield return new WaitForSeconds(0.1f);
        //go to Batiment de prod


        w.SetController(Controller);
        
        yield return w.DoWork();

        List_Assignement.Remove(w);
        
        DoSecAssignement = false;
    }

    public void Set_MainAssign(Assignement ass)
    {
        MainAssignement = ass;
        MainAssignement.SetController(Controller);
    }
    public void Remove_MainAssign()
    {
        MainAssignement = null;
    }
}
