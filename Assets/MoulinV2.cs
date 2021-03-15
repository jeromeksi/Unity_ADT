using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoulinV2 : MonoBehaviour
{
    public List<ItemRef> List_ItemCreate = new List<ItemRef>();
    public List<NPControler> List_Emp = new List<NPControler>();

    private List<ItemRef> List_ItemNeedBuy = new List<ItemRef>();

    public List<StockItem> Stock = new List<StockItem>();

    public List<Assignement> List_CurrentAssignement = new List<Assignement>();
    public int NumberMaxWork;
    public int MaxStock;
    public float Money;

    public GameObject Ferme;
    public GameObject Magasin;

    public bool IsLogg;

    void Start()
    {
        InitStock();
        StartCoroutine(Routine());
        StartCoroutine(LogRoutine());
    }

    private IEnumerator Routine()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            // create work 
            List<Assignement> list_temp = new List<Assignement>();

            foreach(var itw  in List_ItemCreate) //pour chaque iteam a create
            {
                // TODO - AJouter Plusieur task work d'un coup ...
                // On pourra réduire le temps de boucle 
                if(List_CurrentAssignement.Where(x => x.ID == "Work_" + itw.name).ToList().Count<= NumberMaxWork)
                {
                    list_temp.Add(
                        new Work(itw)
                        {
                            ID = "Work_" + itw.name,
                            AmountItem = 1,
                            Batiment = this,
                            PosAssignement = this.transform.position,
                            LevelPrio = 1
                        });
                }               

            }
            bool NeedMoney = false;
            // check buy, create buy



            foreach (var itb in List_ItemNeedBuy) //pour chaque iteam a create
            {

                var existingItb = List_CurrentAssignement.Any(x => x.ID == "Buy_" + itb.name);
                if(!existingItb)
                {


                    var stockItem = Stock.First(x => x.ItemRef == itb);
                    float percentStock = 1 - ((MaxStock - stockItem.Amount) / (1f * MaxStock));

                    //Debug.Log(percentStock);
                    if (percentStock <= 0.20f*(1+stockItem.AmountScaleNeedMin))
                    {
                        float prio = 1 - stockItem.Amount / (stockItem.AmountScaleNeedMin * MaxStock);

                        int amout = Convert.ToInt32(MaxStock*(0.75f + 0.25f * stockItem.AmountScaleNeedMin) - stockItem.Amount);
                        if (amout > Money)
                        {
                            amout = (int)Money;
                            NeedMoney = true;
                        }
                        Debug.Log("WOWOWO");
                        if (amout > MaxStock)
                            amout = MaxStock;

                        list_temp.Add(new Buy(itb)
                        {
                            ID = "Buy_" + itb.name,
                            AmountItem = amout,
                            AmountMoney = amout,
                            Batiment = this,
                            PosAssignement = Ferme.transform.position,
                            LevelPrio = 2
                        });

                    }
                }
            }

            //chekc sell, create sell
            var listItemStockCreate = Stock.Where(s => List_ItemCreate.Contains(s.ItemRef));
            foreach(var its in listItemStockCreate)
            {

                if(its.Amount > MaxStock*0.75f || NeedMoney)
                {
                    list_temp.Add(new Sell(its.ItemRef)
                    {
                        ID = "Sell_" + its.ItemRef.name,
                        AmountItem = its.Amount,
                        Batiment = this,
                        PosAssignement = Magasin.transform.position,
                        LevelPrio = 3
                    });
                }
            }


            //Assign emp et reevaluate prio existing;

            foreach(var assTemp in list_temp)
            {
                var existingAEP = List_CurrentAssignement.Where(x => x.ID == assTemp.ID).ToList();
                
                if(existingAEP.Count>0)
                {
                    switch(existingAEP[0].TypeAssignement)
                    {
                        case TypeAssignement.Buy:

                            existingAEP[0].LevelPrio = assTemp.LevelPrio;
                            ((Buy)existingAEP[0]).AmountMoney = ((Buy)assTemp).AmountMoney;
                            break;
                        case TypeAssignement.Work:
                            if(existingAEP.Count<= NumberMaxWork)
                            {
                                List_CurrentAssignement.Add(assTemp);
                                assTemp.emp = NPControlerFree();
                                assTemp.emp.AddAssignement(assTemp);
                            }
                            break;
                        case TypeAssignement.Sell:
                            existingAEP[0].LevelPrio = assTemp.LevelPrio;
                            ((Sell)existingAEP[0]).AmountItem = ((Sell)assTemp).AmountItem;
                            break;
                    }                    
                }
                else
                {
                    List_CurrentAssignement.Add(assTemp);
                    assTemp.emp = NPControlerFree();
                    assTemp.emp.AddAssignement(assTemp);
                }
            }

            yield return new WaitForSeconds(0.3f);
        }
    }
    private IEnumerator LogRoutine()
    {
        while(true)
        {
            if (IsLogg)
                SomeLog();
            yield return new WaitForSeconds(1f);
        }
    }
    private void InitStock()
    {
        Dictionary<ItemRef, float> ItemCountNeed = new Dictionary<ItemRef, float>();
        foreach (var it in List_ItemCreate)
        {
            if(ItemCountNeed.Any(x=> x.Key == it))
            {
                ItemCountNeed[it]++;
            }
            else
            {
                ItemCountNeed.Add(it, 1);
            }          
            foreach(var itr in it.Recipe)
            {
                if (!List_ItemNeedBuy.Any(x => x == itr.ItemRef))
                    List_ItemNeedBuy.Add(itr.ItemRef);
                if (ItemCountNeed.Any(x => x.Key == itr.ItemRef))
                {
                    ItemCountNeed[itr.ItemRef] += itr.Amount;
                }
                else
                {
                    ItemCountNeed.Add(itr.ItemRef, itr.Amount/ it.baseTimeProduct);
                }
            }
        }
        foreach(var kv in ItemCountNeed)
        {
            Stock.Add(new StockItem(kv.Key, 0)
            {
                Amount = 0,
                AmountScaleNeedMin = Mathf.Clamp(kv.Value,0.001f,1)
            });
            //Debug.Log(kv.Key.Name+" : "+kv.Value);
        }
    }


    public void SomeLog()
    {
        foreach (var it in Stock)
        {
            Debug.Log(it.ItemRef.Name + " : " + it.Amount);
        }
        Debug.Log("Money : " + Money);

    }
    public NPControler NPControlerFree()
    {
        NPControler npc = List_Emp[0];

        for (int i = 1; i < List_Emp.Count; i++)
        {
            if (npc.List_Assignement.Count > List_Emp[i].List_Assignement.Count)
            {
                npc = List_Emp[i];
            }
        }
        return npc;
    }
    public bool SuppMoney(int amount)
    {
        bool isPossible = false;
        if (Money >= amount)
        {
            isPossible = true;
            Money -= amount;
        }
        return isPossible;

    }
    public void AddMoney(int amount)
    {
        Money += amount;
    }
    public void AddItemStock(ItemRef item, int amount)
    {
        Stock.First(x => x.ItemRef == item).Amount += amount;
    }
    public void SuppItemStock(ItemRef item, int amount)
    {
        Stock.First(x => x.ItemRef == item).Amount -= amount;
    }
    public bool CheckItemAmount(ItemRef item, int amount)
    {
        return Stock.First(x => x.ItemRef == item).Amount >= amount;
    }

    internal void RemoveAssignement(Assignement fAssignement)
    {
        List_CurrentAssignement.Remove(fAssignement);
    }
}