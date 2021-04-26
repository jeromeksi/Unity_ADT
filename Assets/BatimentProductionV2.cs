using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BatimentProductionV2 : MonoBehaviour
{
    public int NumberPosteMax;
    public List<NPCController> List_Employe = new List<NPCController>();


    public List<ItemRef> List_ItemCreate = new List<ItemRef>();

    private List<ItemBuy> List_ItemNeedBuy = new List<ItemBuy>();


    public int StockMax;

    public float Money;

    public Shop Magasin;
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

        foreach (var emp in List_Employe)
        {

            emp.Set_MainAssign(new AssignementV2(this)
            {
                IsMainAssignement = true
            });
        }
        StartCoroutine(AssignWork());
    }

    private IEnumerator AssignWork()
    {
        while (IsProduct)
        {
            //TODO - Bon ba c'est la ...
            var list_EmpWorking = List_Employe.Where(x => x.IsWorking).ToList();

            //Assign to buy
            //if(List_ItemNeedBuy.Count(x=> x.IsCurrentBuying == false) >0)
            //{
            //    List<ItemAmount> list_ita = new List<ItemAmount>();
            //    foreach(var it in List_ItemNeedBuy)
            //    {
            //        list_ita.Add(new ItemAmount(it.ItemRef, it.Amount));
            //        var vv = it.IsCurrentBuying;
            //        it.IsCurrentBuying = true;
            //    }

            //    var v = new AssignementV2(this)
            //    {
            //        IsMainAssignement = false,
            //        TypeAssignement = TypeAssignement.Buy,
            //        Money = 200,
            //        Shop = Magasin
            //    };
            //    v.List_Item = list_ita;
            //    var emp = list_EmpWorking.First();
            //        emp.Assign(v);
            //    list_EmpWorking.Remove(emp);
            //}



            var list_itemRefBuy = new List<ItemBuy>();
            foreach(var it in List_ItemNeedBuy.Where(x=> x.IsCurrentBuying == false))
            {
                var its = Stock.GetStockItems(it.ItemRef);
                
                if(its.Amount<StockMax * 0.3f)
                {
                    it.Amount = Convert.ToInt32(0.7f * StockMax - its.Amount);
                    list_itemRefBuy.Add(it);
                }
                else if (its.Amount < StockMax * 0.7f)
                {
                    if(numberEmpReadyProduct/List_Employe.Count > 0.7f)
                    {

                        it.Amount = Convert.ToInt32(StockMax - its.Amount);
                        list_itemRefBuy.Add(it);
                    }
                }
                it.IsCurrentBuying = true;

            }

            if (list_itemRefBuy.Count > 0)
            {
                var v = new AssignementV2(this)
                {
                    IsMainAssignement = false,
                    TypeAssignement = TypeAssignement.Buy,
                    Money = 200,
                    Shop = Magasin
                };
                v.List_Item = ItemBuy.ConvertList_ItemBuy_to_ItemAmount(list_itemRefBuy);
                var emp = list_EmpWorking.First();
                emp.Assign(v);
            }

            //Assign to Sell



            //Assign le reste a Production

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
            List_ItemNeedBuy.FirstOrDefault(x => x.ItemRef == ita.ItemRef).IsCurrentBuying = false;
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

        //CheckItemNeedBuy();
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
                if (!List_ItemNeedBuy.Any(x => x.ItemRef == itr.ItemRef))
                    List_ItemNeedBuy.Add(new ItemBuy() { ItemRef = itr.ItemRef, Amount = 0, IsCurrentBuying = false });
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
    public class ItemBuy
    {
        public ItemRef ItemRef;
        public int Amount;
        public bool IsCurrentBuying;
        public static ItemAmount Convert_ItemBuy_to_ItemAmount(ItemBuy itb)
        {
            return new ItemAmount(itb.ItemRef, itb.Amount);
        }

        public static List<ItemAmount> ConvertList_ItemBuy_to_ItemAmount(List<ItemBuy> itbs)
        {
            List<ItemAmount> listIta = new List<ItemAmount>();
            foreach (var itb in itbs)
            {
                listIta.Add(ItemBuy.Convert_ItemBuy_to_ItemAmount(itb));
            }
            return listIta;
        }
    }


}
