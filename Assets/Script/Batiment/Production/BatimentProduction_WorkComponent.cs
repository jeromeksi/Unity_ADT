using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BatimentProduction_WorkComponent : MonoBehaviour
{
    private int NumberPosteMax;
    private List<NPCController> List_Employe = new List<NPCController>();


    public List<ItemRef> List_ItemCreate = new List<ItemRef>();

    public  List<ItemCheckNeed> List_ItemNeedBuy = new List<ItemCheckNeed>();

    public  List<ItemCheckNeed> List_ItemNeedSell = new List<ItemCheckNeed>();
    public BatimentProduction_Controller Controller;

    public bool NeedMoney;
    public Assignement MainAssignBatiment;
    


    //public Shop Magasin;

    //private Stock Stock = new Stock();

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

        MainAssignBatiment = new Assignement(this);
        MainAssignBatiment.IsMainAssignement = true;
        MainAssignBatiment.TypeAssignement = TypeAssignement.Work;

        StartCoroutine(AssignWork());
    }
    public bool AddEmploye(NPCController nPCController)
    {
        if (List_Employe.Count < NumberPosteMax)
        {
            List_Employe.Add(nPCController);
            return true;
        }
        return false;
    }

    internal void SetNumberPosteMax(int numberPosteMax)
    {
        NumberPosteMax = numberPosteMax;
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
                var itAmount = Controller.GetStock().GetAmount(it.ItemRef);
                int StockMax = Controller.GetStockMAx();
                if (itAmount < StockMax * 0.3f)
                {

                    it.Amount = Convert.ToInt32(StockMax - itAmount);

                    list_itemRefBuy.Add(it);
                    it.IsCurrentActivate = true;
                }
                else if (itAmount < StockMax * 0.7f)
                {
                    if (numberEmpReadyProduct / NumberPosteMax > 0.7f)
                    {

                        it.Amount = Convert.ToInt32(StockMax - itAmount);
                        list_itemRefBuy.Add(it);
                        it.IsCurrentActivate = true;///Problème
                    }
                }
            }

            if (list_itemRefBuy.Count > 0)
            {
               

                List<Assignement> list_Ass = new List<Assignement>();
                int TotalMoneyNeed = 0;
                foreach (var irb in list_itemRefBuy)
                {
                    var shop = Controller.GetShopLowerPriceForItem(irb.ItemRef);
                    var TotalMoneyAss = irb.Amount * shop.PriceForItem(irb.ItemRef);
                    if (TotalMoneyAss.HasValue)
                    {
                        TotalMoneyNeed += TotalMoneyAss.Value;
                        var ass = list_Ass.Find(x => x.Shop == shop);
                        if(ass != null)
                        {
                            ass.List_Item.Add(ItemCheckNeed.Convert_ItemBuy_to_ItemAmount(irb));
                            ass.Money += TotalMoneyAss.Value;
                        }
                        else
                        {

                            var v = new Assignement(this)
                            {
                                IsMainAssignement = false,
                                TypeAssignement = TypeAssignement.Buy,
                                Money = TotalMoneyAss.Value,
                                Shop = shop 
                            };
                            v.List_Item.Add(ItemCheckNeed.Convert_ItemBuy_to_ItemAmount(irb));
                            list_Ass.Add(v);
                        }
                    }
                }

                if(!NeedMoney && TotalMoneyNeed > Controller.GetMoneyComponement().GetMoney())/// bon faut faire un truc ici ... si on a pas de money on fait quoi ?   
                {
                    NeedMoney = true;
                }
                else
                {
                    foreach (var assT in list_Ass)
                    {
                        var emp = GetFreeNPCController();
                        if (emp != null)
                        {
                            emp.Assign(assT);
                        }
                        else
                        {
                            AssignEnd(assT, false);
                        }
                    }
                }
                
            }

            yield return new WaitForSeconds(1f);



            //Assign to Sell
            var list_itemRefSell = new List<ItemCheckNeed>();
            foreach (var it in List_ItemNeedSell.Where(x => x.IsCurrentActivate == false))
            {
                int itAmount = Controller.GetStock().GetAmount(it.ItemRef);
                int StockMax = Controller.GetStockMAx();
                if (itAmount > 0.7f * StockMax || NeedMoney && itAmount > 0)
                {
                    it.Amount = itAmount;
                    list_itemRefSell.Add(it);
                    it.IsCurrentActivate = true;
                }
                else if (itAmount > 0.5f * StockMax)
                {
                    if (numberEmpReadyProduct / NumberPosteMax > 0.7f)
                    {

                        it.Amount = itAmount;
                        list_itemRefSell.Add(it);
                        it.IsCurrentActivate = true;
                    }
                }
            }

            if (list_itemRefSell.Count > 0)
            {
                var v = new Assignement(this)
                {
                    IsMainAssignement = false,
                    TypeAssignement = TypeAssignement.Sell,
                    Money = 0,
                    Shop = Controller.GetShopHigherPriceForItem(list_itemRefSell[0].ItemRef) //ATTENTION ICI A REFAIRE  !! !
                };
                v.List_Item = ItemCheckNeed.ConvertList_ItemBuy_to_ItemAmount(list_itemRefSell);
                var emp = GetFreeNPCController();
                if (emp != null)
                {
                    emp.Assign(v);
                }
                else
                {
                    AssignEnd(v,false);
                }

            }
            
            //Assign le reste a Production
            foreach (var emp in List_Employe)
            {
                emp.Set_MainAssign(MainAssignBatiment);
            }
            yield return new WaitForSeconds(1f);
        }
    }
    public NPCController GetFreeNPCController()
    {
        NPCController ret = null;
        if (List_Employe.Count>0)
        {
            ret = List_Employe[0];
            foreach (var npc in List_Employe)
            {
                ret = ret.CountSecAssign() > npc.CountSecAssign() ? npc : ret;
            }
        }
        return ret; 


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
                //Debug.Log("Erreur : "+ ex);
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
                            lock (Controller.GetStock())
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
            Controller.GetStock().Add(ita);
            List_ItemNeedBuy.FirstOrDefault(x => x.ItemRef == ita.ItemRef).IsCurrentActivate = false;
        }
    }

    internal void AddMoney(float workMoney)
    {
        Controller.GetMoneyComponement().AddMoney(workMoney);
        lock (this)
        {
            NeedMoney = false;
        }
    }

    internal ItemAmount GetItemAmountSell(ItemAmount ita)
    {
        var sItr = Controller.GetStock().GetAmount(ita.ItemRef);
        if(sItr > 0)
        {
            Controller.GetStock().Remove(ita.ItemRef, sItr);
            return new ItemAmount() { ItemRef = ita.ItemRef, Amount = sItr >= ita.Amount ? ita.Amount : sItr };
        }
        else
        {
            return null;
        }

    }

    internal float GetMoneyForAss(Assignement assT)
    {
        if(Controller.GetMoneyComponement().GetMoney() >= assT.Money)
        {
            Controller.GetMoneyComponement().RemoveMoney(assT.Money);
            lock (this)
            {
                NeedMoney = false;
            }
            return assT.Money;
        }
        else
        {
            var moneyT = Controller.GetMoneyComponement().GetMoney();
            Controller.GetMoneyComponement().RemoveMoney(moneyT);
            lock (this)
            {
                NeedMoney = true;
            }
            return moneyT;

        }
    }

    private void SuppItemStock(ItemRef itemRef, int amount)
    {
        Controller.GetStock().Remove(itemRef, amount);
    }

    private bool CheckItemAmount(ItemRef itemRef, int amount)
    {
        return Controller.GetStock().GetAmount(itemRef) >= amount;
    }
    private void AddItemStock(ItemRef itemRef, int amount)
    {
        Controller.GetStock().Add(itemRef, amount);

    }
    internal void AssignEnd(Assignement assignementV2, bool succed)
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
        foreach(var usi in assignementV2.List_UpdateShopInfo)
        {
            Controller.AddMemoryInfo(usi);
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
            Controller.GetStock().Add(new ItemAmount(itr));
        }
    }   
}
[Serializable]
public class ItemAmount
{
    public ItemRef ItemRef;
    public int Amount;
    public ItemAmount(ItemRef _ItemRef, int _Amount = 0)
    {
        ItemRef = _ItemRef;
        Amount = _Amount;
    }
    public ItemAmount()
    {
        ItemRef = null;
        Amount = 0;
    }
}
public enum TypeAssignement
{
    Buy,
    Sell,
    Goto,
    Work
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
