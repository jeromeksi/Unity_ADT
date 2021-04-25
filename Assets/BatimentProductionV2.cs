using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BatimentProductionV2 : MonoBehaviour
{
    public int NumberPosteMax;
    public List<NPCController> List_Employe = new List<NPCController>();


    private List<WorkV2> listWork = new List<WorkV2>();
    public List<ItemRef> List_ItemCreate = new List<ItemRef>();
    private List<ItemRef> List_ItemNeedBuy = new List<ItemRef>();


    public  float Money;

    public Shop Magasin;
    public ItemRef ble;
    //private List<ItemRef>
    public Stock Stock = new Stock();

    public bool IsProduct;
    private int numberWorkingEmp;
    private int numberEmpReadyProduct;

    void Start()
    {
        InitStock();
        IsProduct = true;
        StartCoroutine(CalculatePercent());
        //var v = new AssignementV2(this)
        //{
        //    IsMainAssignement = false,
        //    TypeAssignement = TypeAssignement.Buy,
        //    Money = 200,
        //    Shop = Magasin
        //};

        //v.List_Item.Add(new ItemAmount(ble, 10));
        //List_Employe[0].Assign(v);
        foreach (var itc in List_ItemCreate)
        {

            StartCoroutine(RoutineProdItemCreate(itc));
        }
        StartCoroutine(AssignWork());
    }

    private IEnumerator AssignWork()
    {
        while (IsProduct)
        {


            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator CalculatePercent()
    {
        while (IsProduct)
        {
            try
            {
                numberWorkingEmp = List_Employe.Count(x => x.IsWorking);
                numberEmpReadyProduct = List_Employe.Count(x => x.IsWorking && x.IsDoMainAssign());
            }
            catch(Exception ex)
            {
                Debug.Log("Erreur : "+ ex);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator RoutineProdItemCreate(ItemRef itr)
    {
        float etatWork = 0;
        bool AddNeedItem = false;
        while (IsProduct)
        {
            bool createItem = true;
            switch (itr.TypeCreate)
            {
                case TypeCreate.Craft:

                        foreach (var ritem in itr.Recipe)
                        {
                            if (!CheckItemAmount(ritem.ItemRef, ritem.Amount))
                            {
                                createItem = false;
                                if(!AddNeedItem)
                                    NeedItemRef(ritem.ItemRef);
                            }
                        }
                        if (createItem)
                        {
                            AddNeedItem = false;
                            lock (Stock)
                            {
                                foreach (var ritem in itr.Recipe)
                                {
                                    SuppItemStock(ritem.ItemRef, ritem.Amount);
                                }
                            }
                        }
                        else
                        {
                            AddNeedItem = true;
                        }                    
                    break;
            }
            if(createItem)
            {
                while (etatWork < itr.baseWorkingNeed)
                {
                    yield return new WaitForSeconds(0.1f);
                    etatWork += (numberEmpReadyProduct / 10.0f) ;
                    Debug.Log(etatWork);
                }
                AddItemStock(itr, itr.AmountByWorking);

            }            
            etatWork = 0;
            Debug.Log("ici");
            yield return new WaitForSeconds(0.1f);
        }
    }

    internal void AddListItem(List<ItemAmount> list_StockItemWork)
    {
        foreach(var ita in list_StockItemWork)
        {
            Stock.Add(ita);
        }
    }

    internal void AddMoney(float workMoney)
    {
        Money += workMoney;
    }

    internal ItemAmount GetItemAmountSell(ItemAmount ita)
    {
        var sItr = Stock.GetNumber(ita.ItemRef);
        return new ItemAmount() { ItemRef = ita.ItemRef, Amount = sItr >= ita.Amount ? ita.Amount : sItr };

    }

    internal float GetMoneyForAss(AssignementV2 assT)
    {
        if(Money >= assT.Money)
        {
            Money -= assT.Money;
            return assT.Money;
        }
        throw new Exception("Pas assez d'argent");
    }

    private void NeedItemRef(ItemRef itemRef)
    {
        Debug.Log("Pas la ressoures : " + itemRef);
        if (!List_ItemNeedBuy.Exists(x => x == itemRef))
            List_ItemNeedBuy.Add(itemRef);
    }

    private void SuppItemStock(ItemRef itemRef, int amount)
    {
        Stock.Remove(itemRef, amount);
    }

    private bool CheckItemAmount(ItemRef itemRef, int amount)
    {
        return Stock.GetNumber(itemRef) >= amount;
        //return true;
    }
    private void AddItemStock(ItemRef itemRef, int amount)
    {
        Stock.Add(itemRef, amount);
    }
    private IEnumerator RoutineBuy()
    {
        while (IsProduct)
        {
            //vendre des 

            yield return new WaitForSeconds(1f);
        }
    }
    private IEnumerator RoutineSell()
    {
        while (true)
        {


            yield return new WaitForSeconds(1f);
        }
    }

    private void InitStock()
    {
        Dictionary<ItemRef, float> ItemCountNeed = new Dictionary<ItemRef, float>();
        foreach (var it in List_ItemCreate)
        {
            if (ItemCountNeed.Any(x => x.Key == it))
            {
                ItemCountNeed[it]++;
            }
            else
            {
                ItemCountNeed.Add(it, 1);
            }
            foreach (var itr in it.Recipe)
            {
                if (ItemCountNeed.Any(x => x.Key == itr.ItemRef))
                {
                    ItemCountNeed[itr.ItemRef] += itr.Amount / it.baseTimeProduct;
                }
                else
                {
                    ItemCountNeed.Add(itr.ItemRef, itr.Amount / it.baseTimeProduct);
                }
            }
        }
        foreach (var kv in ItemCountNeed)
        {
            Stock.Add(new StockItem(kv.Key, 0)
            {
                Amount = 0,
                AmountScaleNeedMin = Mathf.Clamp(kv.Value, 0.001f, 1)
            });
            //Debug.Log(kv.Key.Name+" : "+kv.Value);
        }
    }




}
