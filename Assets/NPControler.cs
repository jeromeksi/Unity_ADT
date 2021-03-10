using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NPControler : MonoBehaviour
{
    NavMeshAgent agent;
    public int money;
    public Dictionary<ItemRef, int> StockItem;

    public List<AssignementPrio> List_Assignement;
    public bool DoAssignement;
    public GameObject Moulin_Test;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        List_Assignement = new List<AssignementPrio>();
        DoAssignement = false;
        StockItem = new Dictionary<ItemRef, int>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!DoAssignement && List_Assignement.Count > 0)
        {
            //var fAssignement = List_Assignement.First();
            var fAssignement = List_Assignement.OrderByDescending(x => x.LevelPrio).First();
            StartCoroutine(ExecuteAssignement(fAssignement));
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //List_Assignement.Add(new Assignement() { NomAssignement = "test", PosAssignement = Moulin_Test.transform.position });
        }
    }
    public void AddAssignement(AssignementPrio assign)
    {
        List_Assignement.Add(assign);
    }
    private IEnumerator ExecuteAssignement(AssignementPrio fAssignementPrio)
    {
        DoAssignement = true;
        var fAssignement = fAssignementPrio.Assignement;


        switch (fAssignement.TypeAssignement)
        {
            case TypeAssignement.Buy:
                Debug.Log("Je vais au moulin");
                var fBuy = (Buy)fAssignement;
                agent.SetDestination(fBuy.Batiment.transform.position);
                while (!Util.CheckIsArrived(this.transform.position, agent))
                {
                    yield return new WaitForSeconds(0.1f);

                }
                if (fBuy.Batiment.SuppMoney(fBuy.AmountMoney))
                    money = fBuy.AmountMoney;
                else
                    break;
                yield return new WaitForSeconds(0.5f);
                Debug.Log("Je vais a la ferme");
                agent.SetDestination(fBuy.PosAssignement);
                while (!Util.CheckIsArrived(this.transform.position, agent))
                {
                    yield return new WaitForSeconds(0.1f);

                }
                money -= fBuy.AmountMoney;
                if(StockItem.ContainsKey(fBuy.ItemBuy))
                {
                    StockItem[fBuy.ItemBuy] += fBuy.AmountItem;
                }
                else
                {
                    StockItem.Add(fBuy.ItemBuy, fBuy.AmountItem);
                }

                yield return new WaitForSeconds(1);
                Debug.Log("Je vais au moulin");
                agent.SetDestination(fBuy.Batiment.transform.position);
                while (!Util.CheckIsArrived(this.transform.position, agent))
                {
                    yield return new WaitForSeconds(0.1f);

                }
                yield return new WaitForSeconds(1);
                fBuy.Batiment.AddItemStock(fBuy.ItemBuy, fBuy.AmountItem);
                break;
            case TypeAssignement.Goto:
                break;
            case TypeAssignement.Sell:
                Debug.Log("Je vais au moulin");
                var fSell = (Sell)fAssignement;
                agent.SetDestination(fSell.Batiment.transform.position);
                while (!Util.CheckIsArrived(this.transform.position, agent))
                {
                    yield return new WaitForSeconds(0.1f);

                }
                fSell.Batiment.SuppItemStock(fSell.ItemSell, fSell.AmountItem);

                if (StockItem.ContainsKey(fSell.ItemSell))
                {
                    StockItem[fSell.ItemSell] += fSell.AmountItem;
                }
                else
                {
                    StockItem.Add(fSell.ItemSell, fSell.AmountItem);
                }

                Debug.Log("Je vais au magasin");

                agent.SetDestination(fSell.PosAssignement);
                while (!Util.CheckIsArrived(this.transform.position, agent))
                {
                    yield return new WaitForSeconds(0.1f);

                }
                yield return new WaitForSeconds(1);

                StockItem[fSell.ItemSell] -= fSell.AmountItem;
                money += 100;

                yield return new WaitForSeconds(1);
                agent.SetDestination(fSell.Batiment.transform.position);
                Debug.Log("Je vais au moulin");
                while (!Util.CheckIsArrived(this.transform.position, agent))
                {
                    yield return new WaitForSeconds(0.1f);

                }
                yield return new WaitForSeconds(1);
                fSell.Batiment.AddMoney(100);
                money -= 100;
                break;
            case TypeAssignement.Work:
                var fWork = (Work)fAssignement;

                agent.SetDestination(fWork.PosAssignement);
                while (!Util.CheckIsArrived(this.transform.position, agent))
                {
                    yield return new WaitForSeconds(0.1f);

                }
                //yield return new WaitForSeconds(1);


                var r = fWork.ItemCreate.Recipe;
                bool createItem = true;
                for (int i = 0; i < fWork.AmountItem; i++)
                {
                    foreach (var ritem in r)
                    {
                        if (!fWork.Batiment.CheckItemAmount(ritem.ItemRef, ritem.Amount))
                        {
                            createItem = false;
                        }
                    }
                    if (createItem)
                    {
                        lock (fWork.Batiment)
                        {
                            foreach (var ritem in r)
                            {
                                fWork.Batiment.SuppItemStock(ritem.ItemRef, ritem.Amount);
                            }
                        }
                        yield return new WaitForSeconds(fWork.ItemCreate.baseTimeProduct);
                        fWork.Batiment.AddItemStock(fWork.ItemCreate, 1);
                        //Debug.Log(DateTime.Now + " : "+ this.name +"J'ai créer 1 de farine");

                    }
                    else
                    {
                        break;
                    }
                }
                fWork.NumberAssignation -= 1;

                break;
        }

        fAssignement.IsAssign = false;
        List_Assignement.Remove(fAssignementPrio);
        agent.SetDestination(this.transform.position);
        DoAssignement = false;
    }
}
