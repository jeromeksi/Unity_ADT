using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPC_WorkComponement : MonoBehaviour
{
    public List<Assignement> List_Assignement = new List<Assignement>();
    private Assignement MainAssignement;
    public Assignement CurrentAssignement;
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
        bool b = true;
        if(MainAssignement.BatimentProduction != null)
        {
            NPCController.SetDestination(MainAssignement.BatimentProduction.gameObject.transform.position);
            yield return NPCController.CheckArrive();

        }


    }

    internal bool HadMainAssing()
    {
        return MainAssignement != null;
    }

    internal void AssignSec(Assignement assignementV2)
    {
        List_Assignement.Add(assignementV2);
    }

    private IEnumerator SecAssignementRoutine(Assignement assT)
    {
        DoSecAssignement = true;
        DoMainAssignement = false;
        yield return new WaitForSeconds(0.1f);
        //go to Batiment de prod
        NPCController.SetDestination(assT.BatimentProduction.gameObject.transform.position);


        yield return NPCController.CheckArrive();


        //Take Money ou Item
        switch(assT.TypeAssignement)
        {
            case TypeAssignement.Sell:
                foreach (var ita in assT.List_Item)
                {
                    var itas = assT.BatimentProduction.GetItemAmountSell(ita);
                    if(itas !=null)
                    {
                        List_StockItemWork.Add(itas);
                    }
                }
                break;
            case TypeAssignement.Buy:

                WorkMoney = assT.BatimentProduction.GetMoneyForAss(assT);

                break;
        }
        if(WorkMoney > 0 || List_StockItemWork.Count > 0)
        {

            //GO to shop
            NPCController.SetDestination(assT.Shop.gameObject.transform.position);


            yield return NPCController.CheckArrive();


            //Vendre ou Buy
            switch (assT.TypeAssignement)
            {
                case TypeAssignement.Buy:
                    foreach (var ita in assT.List_Item)
                    {

                        var trasact = assT.Shop.SellItem(ita, WorkMoney);
                        if(trasact != null)
                        {
                            List_StockItemWork.Add(trasact.ItemAmount);
                            WorkMoney -= trasact.Money;
                        }
                    }
                    break;
                case TypeAssignement.Sell:
                    foreach (var ita in List_StockItemWork)
                    {
                        var transact = assT.Shop.BuyItem(ita.ItemRef, ita.Amount);
                        if(transact != null)
                        {
                            WorkMoney += transact.Money;
                            ita.Amount -= ita.Amount;
                        }
                    }
                    break;
            }
            assT.List_UpdateShopInfo = assT.Shop.GetAllItemPrice();
            

            //Goto Bat

            NPCController.SetDestination(assT.BatimentProduction.gameObject.transform.position);


            yield return NPCController.CheckArrive();

            //decharge
            assT.BatimentProduction.AddMoney(WorkMoney);
            WorkMoney = 0;
            assT.BatimentProduction.AddListItem(List_StockItemWork);
            List_StockItemWork.Clear();
            assT.EndAssign(true);
        }
        else
        {
            assT.EndAssign(false);
        }
        List_Assignement.Remove(assT);

        DoSecAssignement = false;
    }

    public void Set_MainAssign(Assignement ass)
    {
        MainAssignement = ass;
    }
    public void Remove_MainAssign()
    {
        MainAssignement = null;
    }
}
