using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPC_WorkComponement : MonoBehaviour
{
    public List<AssignementV2> List_Assignement = new List<AssignementV2>();
    private AssignementV2 MainAssignement;

    private NPCController NPCController;
    public bool DoMainAssignement;
    public bool DoSecAssignement;
    public float WorkMoney;
    public List<ItemAmount> List_StockItemWork = new List<ItemAmount>();
    private void Start()
    {
        NPCController = GetComponent<NPCController>();
    }
       
    public void Update()
    {
        if (List_Assignement.Count > 0 && !DoSecAssignement)
        {
            DoMainAssignement = false;

            var assT = List_Assignement.First();
            if (assT != null)
            {
                List_Assignement.Remove(assT);
                StartCoroutine( SecAssignementRoutine(assT));
                DoSecAssignement = true;
            }
        }
        else
        {
            if (!DoSecAssignement)
                DoMainAssignement = true;
        }
    }
    internal bool HadMainAssing()
    {
        return MainAssignement != null;
    }

    internal void AssignSec(AssignementV2 assignementV2)
    {
        List_Assignement.Add(assignementV2);
    }

    private IEnumerator SecAssignementRoutine(AssignementV2 assT)
    {
        DoSecAssignement = true;
        DoMainAssignement = false;
        yield return new WaitForSeconds(0.1f);
        //go to Batiment de prod
        NPCController.SetDestination(assT.BatimentProduction.gameObject.transform.position);


        yield return NPCController.CheckArrive();


        //Take Money ou Item
        WorkMoney = assT.BatimentProduction.GetMoneyForAss(assT);

        if(assT.TypeAssignement == TypeAssignement.Sell)
        {
            foreach(var ita in assT.List_Item)
            {
                List_StockItemWork.Add(assT.BatimentProduction.GetItemAmountSell(ita));
            }
        }

        //GO to shop
        NPCController.SetDestination(assT.Shop.gameObject.transform.position);


        yield return NPCController.CheckArrive();
        

        //Vendre ou Buy
        switch (assT.TypeAssignement)
        {
            case TypeAssignement.Buy:
                foreach(var ita in assT.List_Item)
                {

                    var trasact = assT.Shop.SellItem(ita, WorkMoney);

                    List_StockItemWork.Add(trasact.ItemAmount);
                    WorkMoney -= trasact.Money;
                }
                break;
            case TypeAssignement.Sell:
                foreach(var ita in List_StockItemWork)
                {
                    WorkMoney += assT.Shop.BuyItem(ita.ItemRef, ita.Amount);
                }
                break;
        }
        //Goto Bat

        NPCController.SetDestination(assT.BatimentProduction.gameObject.transform.position);


        yield return NPCController.CheckArrive();

        //decharge
        assT.BatimentProduction.AddMoney(WorkMoney);
        WorkMoney = 0;
        assT.BatimentProduction.AddListItem(List_StockItemWork);
        List_StockItemWork.Clear();
        DoSecAssignement = false;
    }

    public void Set_MainAssign(AssignementV2 ass)
    {
        if(MainAssignement != null && ass.IsMainAssignement)
            MainAssignement = ass;
    }
    public void Remove_MainAssign(AssignementV2 ass)
    {
        MainAssignement = ass;
    }
}
