using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static Sell;

public class NPControlerAssignement : MonoBehaviour
{
    NavMeshAgent agent;
    public float money;
    public Dictionary<ItemRef, int> StockItem;
    public List<ItemAmount> List_itemsSell;

    public List<Assignement> List_Assignement;
    public bool DoAssignement;
    public GameObject Moulin_Test;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        List_Assignement = new List<Assignement>();
        DoAssignement = false;
        StockItem = new Dictionary<ItemRef, int>();
        List_itemsSell = new List<ItemAmount>();
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
    public void AddAssignement(Assignement assign)
    {
        List_Assignement.Add(assign);
    }
    private IEnumerator ExecuteAssignement(Assignement fAssignement)
    {
        DoAssignement = true;


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
                List_itemsSell.Clear();
                
                foreach(var its in fSell.List_ItemSell)
                {
                    fSell.Batiment.SuppItemStock(its.ItemRef, its.Amount);
                    List_itemsSell.Add(its);
                }



                Debug.Log("Je vais au magasin");

                agent.SetDestination(fSell.ShopRef.gameObject.transform.position);
                while (!Util.CheckIsArrived(this.transform.position, agent))
                {
                    yield return new WaitForSeconds(0.1f);

                }
                yield return new WaitForSeconds(1);


                foreach(var its in List_itemsSell)
                {
                    money += fSell.ShopRef.BuyItem(its.ItemRef, its.Amount);

                }


                //int prix;
                //switch(fSell.ItemSell.name)
                //{
                //    case "Huile":
                //        prix = 20;
                //        break;
                //    case "Farine":
                //        prix = 4;
                //        break;
                //    default:
                //        prix = 1;
                //        break;
                //}
                //money = fSell.AmountItem * prix;

                yield return new WaitForSeconds(1);
                agent.SetDestination(fSell.Batiment.transform.position);
                Debug.Log("Je vais au moulin");
                while (!Util.CheckIsArrived(this.transform.position, agent))
                {
                    yield return new WaitForSeconds(0.1f);

                }
                yield return new WaitForSeconds(1);
                fSell.Batiment.AddMoney(money);
                money = 0 ;
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
                switch(fWork.ItemCreate.TypeCreate)
                {
                    case TypeCreate.Craft:
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
                                Debug.Log("Pas de ressoures nécessaire");
                                break;
                            }
                        }
                        break;
                    case TypeCreate.Recolt:
                        yield return new WaitForSeconds(fWork.ItemCreate.baseTimeProduct);
                        fWork.Batiment.AddItemStock(fWork.ItemCreate, 1);
                        break;
                }
                
                fWork.NumberAssignation -= 1;

                break;
        }

        fAssignement.IsAssign = false;
        RemoveAssignement(fAssignement);
        agent.SetDestination(this.transform.position);
        DoAssignement = false;
    }
    public void RemoveAssignement(Assignement fAssignement)
    {
        List_Assignement.Remove(fAssignement);
        fAssignement.Batiment.RemoveAssignement(fAssignement);
    }
}
