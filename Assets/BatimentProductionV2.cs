using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BatimentProductionV2 : MonoBehaviour
{
    public int NumberPosteMax;
    public List<EmployeController> List_Employe = new List<EmployeController>();


    private List<WorkV2> listWork = new List<WorkV2>();
    public List<ItemRef> List_ItemCreate = new List<ItemRef>();
    private List<ItemRef> List_ItemNeedBuy = new List<ItemRef>();

    public Stock Stock = new Stock();

    public bool IsProduct;
    private int numberWorkingEmp;
    void Start()
    {
        InitStock();
        IsProduct = true;
        StartCoroutine(CalculatePercent());
        foreach(var itc in List_ItemCreate)
        {

            StartCoroutine(RoutineProdItemCreate(itc));
        }
    }

    private IEnumerator CalculatePercent()
    {
        while (IsProduct)
        {
            try
            {
                numberWorkingEmp = List_Employe.Count(x => x.IsProduct);

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
        while(IsProduct)
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
                            }
                        }
                        if (createItem)
                        {
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
                            Debug.Log("Pas de ressoures nécessaire");
                        }                    
                    break;
            }
            if(createItem)
            {
                while (etatWork < itr.baseWorkingNeed)
                {
                    yield return new WaitForSeconds(0.1f);
                    etatWork += (numberWorkingEmp/10.0f) ;
                    Debug.Log(etatWork);
                }
                AddItemStock(itr, 1);

            }            
            etatWork = 0;
            Debug.Log("ici");
            yield return new WaitForSeconds(0.1f);
        }
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
                if (!List_ItemNeedBuy.Any(x => x == itr.ItemRef))
                    List_ItemNeedBuy.Add(itr.ItemRef);
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
