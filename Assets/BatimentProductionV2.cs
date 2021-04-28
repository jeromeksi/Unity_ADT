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

    public  List<ItemCheckNeed> List_ItemNeedBuy = new List<ItemCheckNeed>();

    public  List<ItemCheckNeed> List_ItemNeedSell = new List<ItemCheckNeed>();

    public bool NeedMoney;
    public AssignementV2 MainAssignBatiment;
    
    public int StockMax;

    public float Money;



    public Shop Magasin;

    public Stock Stock = new Stock();

    public bool IsProduct;
    public int numberWorkingEmp;
    public int numberEmpReadyProduct;

    void Start()
    {
        InitStock();
        IsProduct = true;
        StartCoroutine(CalculatePercent());

        foreach (var itc in List_ItemCreate)
        {

            StartCoroutine(RoutineProdItemCreate(itc));
        }

        MainAssignBatiment = new AssignementV2(this);
        MainAssignBatiment.IsMainAssignement = true;
        MainAssignBatiment.TypeAssignement = TypeAssignement.Work;

        StartCoroutine(AssignWork());
    }

    private IEnumerator AssignWork()
    {

        yield return new WaitForSeconds(1f);
        while (IsProduct)
        {
            var list_EmpWorking = List_Employe.Where(x => x.IsWorking).ToList();


            var list_itemRefBuy = new List<ItemCheckNeed>();
            foreach (var it in List_ItemNeedBuy.Where(x => x.IsCurrentActivate == false))
            {
                var its = Stock.GetStockItems(it.ItemRef);

                if (its.Amount < StockMax * 0.3f)
                {

                    it.Amount = Convert.ToInt32(StockMax - its.Amount);

                    list_itemRefBuy.Add(it);
                    it.IsCurrentActivate = true;
                }
                else if (its.Amount < StockMax * 0.7f)
                {
                    if (numberEmpReadyProduct / NumberPosteMax > 0.7f)
                    {

                        it.Amount = Convert.ToInt32(StockMax - its.Amount);
                        list_itemRefBuy.Add(it);
                        it.IsCurrentActivate = true;
                    }
                }
            }

            if (list_itemRefBuy.Count > 0)
            {
                var v = new AssignementV2(this)
                {
                    IsMainAssignement = false,
                    TypeAssignement = TypeAssignement.Buy,
                    Money = 90,
                    Shop = Magasin
                };
                v.List_Item = ItemCheckNeed.ConvertList_ItemBuy_to_ItemAmount(list_itemRefBuy);
                var emp = list_EmpWorking.First();
                emp.Assign(v); 
                lock (this)
                {
                    NeedMoney = false;
                }
            }

            yield return new WaitForSeconds(1f);
            //Assign to Sell
            var list_itemRefSell = new List<ItemCheckNeed>();
            foreach (var it in List_ItemNeedSell.Where(x => x.IsCurrentActivate == false))
            {
                var its = Stock.GetStockItems(it.ItemRef);
                if (its.Amount > 0.7f * StockMax || NeedMoney && its.Amount > 0)
                {
                    it.Amount =  its.Amount;
                    list_itemRefSell.Add(it);
                    it.IsCurrentActivate = true;
                }
                else if (its.Amount > 0.5f * StockMax)
                {
                    if (numberEmpReadyProduct / NumberPosteMax > 0.7f)
                    {

                        it.Amount = its.Amount;
                        list_itemRefSell.Add(it);
                        it.IsCurrentActivate = true;
                    }
                }
            }

            if (list_itemRefSell.Count > 0)
            {
                var v = new AssignementV2(this)
                {
                    IsMainAssignement = false,
                    TypeAssignement = TypeAssignement.Sell,
                    Money = 0,
                    Shop = Magasin
                };
                v.List_Item = ItemCheckNeed.ConvertList_ItemBuy_to_ItemAmount(list_itemRefSell);
                var emp = list_EmpWorking[1];
                emp.Assign(v);
               
            }
            
            //Assign le reste a Production
            foreach (var emp in List_Employe)
            {
                emp.Set_MainAssign(MainAssignBatiment);
            }
            yield return new WaitForSeconds(1f);
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
                            lock (Stock)
                            {
                                foreach (var ritem in itr.Recipe)
                                {
                                    SuppItemStock(ritem.ItemRef, ritem.Amount);
                                }
                            }
                        }                
                    break;
            }
            if(createItem)
            {
                while (etatWork < itr.baseWorkingNeed)
                {
                    yield return new WaitForSeconds(0.1f);
                    etatWork += (numberEmpReadyProduct / 10.0f) ;
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
        foreach(var ita in list_StockItemWork.Where(x => x.Amount > 0))
        {
            Stock.Add(ita);
            List_ItemNeedBuy.FirstOrDefault(x => x.ItemRef == ita.ItemRef).IsCurrentActivate = false;
        }
    }

    internal void AddMoney(float workMoney)
    {
        Money += workMoney;
    }

    internal ItemAmount GetItemAmountSell(ItemAmount ita)
    {
        var sItr = Stock.GetNumber(ita.ItemRef);
        if(sItr > 0)
        {
            Stock.Remove(ita.ItemRef, sItr);
            return new ItemAmount() { ItemRef = ita.ItemRef, Amount = sItr >= ita.Amount ? ita.Amount : sItr };
        }
        else
        {
            return null;
        }

    }

    internal float GetMoneyForAss(AssignementV2 assT)
    {
        if(Money >= assT.Money)
        {
            Money -= assT.Money;
            lock (this)
            {
                NeedMoney = false;
            }
            return assT.Money;
        }
        else
        {
            var moneyT = Money;
            Money -= Money;
            lock (this)
            {
                NeedMoney = true;
            }
            return moneyT;

        }
    }

    private void SuppItemStock(ItemRef itemRef, int amount)
    {
        Stock.Remove(itemRef, amount);
    }

    private bool CheckItemAmount(ItemRef itemRef, int amount)
    {
        return Stock.GetNumber(itemRef) >= amount;
    }
    private void AddItemStock(ItemRef itemRef, int amount)
    {
        Stock.Add(itemRef, amount);

    }
    internal void AssignEnd(AssignementV2 assignementV2, bool succed)
    {
        switch(assignementV2.TypeAssignement)
        {
            case TypeAssignement.Buy:
                DesactivateAll(assignementV2.List_Item, List_ItemNeedBuy);
                break;
            case TypeAssignement.Sell:
                DesactivateAll(assignementV2.List_Item, List_ItemNeedSell);
                break;
        }
       
    }
    private void DesactivateAll(List<ItemAmount> lista, List<ItemCheckNeed> lisNeed)
    {
        foreach (var it in lista)
        {
            var ite = lisNeed.First(x => x.ItemRef == it.ItemRef);
            if(ite != null)
            {
                ite.IsCurrentActivate = false;
            }
        }
    }
    private void InitStock()
    {
        List<ItemRef> ItemNeed = new List<ItemRef>();
        foreach (var it in List_ItemCreate)
        {
            if (!ItemNeed.Any(x => x == it))
            {             
                ItemNeed.Add(it);

                List_ItemNeedSell.Add(new ItemCheckNeed() { ItemRef = it, Amount = 0,IsCurrentActivate=false});
            }
            foreach (var itr in it.Recipe)
            {
                if (!List_ItemNeedBuy.Any(x => x.ItemRef == itr.ItemRef))
                    List_ItemNeedBuy.Add(new ItemCheckNeed() { ItemRef = itr.ItemRef, Amount = 0, IsCurrentActivate = false });


                if (!ItemNeed.Any(x => x == itr.ItemRef))
                {                
                    ItemNeed.Add(itr.ItemRef);
                }
            }
        }
        foreach (var itr in ItemNeed)
        {
            Stock.Add(new ItemAmount(itr));
        }
    }
    [Serializable]
    public class ItemCheckNeed
    {
        public ItemRef ItemRef;
        public int Amount;
        public bool IsCurrentActivate;
        public static ItemAmount Convert_ItemBuy_to_ItemAmount(ItemCheckNeed itb)
        {
            return new ItemAmount(itb.ItemRef, itb.Amount);
        }

        public static List<ItemAmount> ConvertList_ItemBuy_to_ItemAmount(List<ItemCheckNeed> itbs)
        {
            List<ItemAmount> listIta = new List<ItemAmount>();
            foreach (var itb in itbs)
            {
                listIta.Add(ItemCheckNeed.Convert_ItemBuy_to_ItemAmount(itb));
            }
            return listIta;
        }
    }
}
