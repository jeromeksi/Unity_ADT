using Assets.Script.Batiment.Assignement_Work;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

namespace Batiment.BatimentProduction
{
    [Serializable]
    public class BatimentProduction_WorkComponent 
    {
        private bool b_SearchInfo = false;
        public bool SecourEnCours = false ;
        private int NumberPosteMax;
        private List<NPCController> List_Employe = new List<NPCController>();

        public TimeSpan TimeMax_Seours = new TimeSpan(0,2,30);
        public TimeSpan TimerMax_CheckInfo = new TimeSpan(0, 1, 0);


        public List<ItemRef> List_ItemCreate = new List<ItemRef>();

        public List<ItemCheckNeed> List_ItemNeedBuy = new List<ItemCheckNeed>();

        public List<ItemCheckNeed> List_ItemNeedSell = new List<ItemCheckNeed>();
        public BatimentProduction_Controller Controller;

        public bool NeedMoney;

        public DateTime DateLastProduction = DateTime.Now; // Check last Prodcution et declencher une routine de secours !! (pas mal) // Donc 2 routines (check + secours)

        public DateTime DateLastTime_CheckPrice = DateTime.Now;
        //public Shop Magasin;

        //private Stock Stock = new Stock();

        public int numberEmpReadyProduct;

        public void AssignMainWork_ByEmp()
        {
            foreach(var emp in List_Employe)
            {

                BatimentProduction_Work MainAssignBatiment = new BatimentProduction_Work();
                MainAssignBatiment.Pos_BatimentProduction = Controller.transform.position;
                MainAssignBatiment.IsMainAssignement = true;
                MainAssignBatiment.TypeAssignement = TypeAssignement.Work;

                emp.Assign(MainAssignBatiment);
            }
        }

        public bool AddEmploye(NPCController nPCController)
        {
            if (List_Employe.Count < NumberPosteMax)
            {
                List_Employe.Add(nPCController);

                BatimentProduction_Work MainAssignBatiment = new BatimentProduction_Work();
                MainAssignBatiment.Pos_BatimentProduction = Controller.transform.position;
                MainAssignBatiment.IsMainAssignement = true;
                MainAssignBatiment.TypeAssignement = TypeAssignement.Work;

                nPCController.Set_MainAssign(MainAssignBatiment);
                return true;
            }
            return false;
        }

        internal void SetNumberPosteMax(int numberPosteMax)
        {
            NumberPosteMax = numberPosteMax;
        }

        public IEnumerator AssignWork()
        {

            yield return new WaitForSeconds(1f);
            while (Controller.BatIsActive())
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
                    }
                    else if (itAmount < StockMax * 0.7f)
                    {
                        if (List_Employe.Count(x => x.IsWorking && x.IsDoMainAssign()) / NumberPosteMax > 0.7f)
                        {

                            it.Amount = Convert.ToInt32(StockMax - itAmount);
                            list_itemRefBuy.Add(it);
                        }
                    }
                }

                if (list_itemRefBuy.Count > 0)
                {


                    List<Assignement> list_Ass = new List<Assignement>();
                    int TotalMoneyNeed = 0;
                    foreach (var irb in list_itemRefBuy)
                    {
                        var Shop = Controller.GetShopLowerPriceForItem(irb.ItemRef); // a amélioré en une fonction
                        var PriceForItem = Controller.GetLowerPriceForItem(irb.ItemRef);
                        int TotalMoneyAss = 0;
                        if (PriceForItem.HasValue && Shop != null)
                        {
                            var ite = List_ItemNeedBuy.FirstOrDefault(x => x.ItemRef == irb.ItemRef);
                            if (ite != null)
                                ite.IsCurrentActivate = true;
                            TotalMoneyAss = irb.Amount * PriceForItem.Value;

                            TotalMoneyNeed += TotalMoneyAss;
                            Buy ass = (Buy)list_Ass.Find(x => ((Buy)x).Shop == Shop);
                            if (ass != null)
                            {
                                ass.List_Item.Add(ItemCheckNeed.Convert_ItemBuy_to_ItemAmount(irb));
                                ass.Money += TotalMoneyAss;
                            }
                            else
                            {

                                var v = new Buy()
                                {
                                    IsMainAssignement = false,
                                    TypeAssignement = TypeAssignement.Buy,
                                    Money = TotalMoneyAss,
                                    Shop = Shop,
                                    TypeBuy = TypeBuy.Share,
                                    Pos_Batiment = Controller.transform.position,
                                    BatimentProduction = this
                                };
                                v.List_Item.Add(ItemCheckNeed.Convert_ItemBuy_to_ItemAmount(irb));
                                list_Ass.Add(v);
                            }
                        }
                    }

                    int currentMoney = Controller.GetMoneyComponement().GetMoney();
                    int moneyByAssign = -1;
                    bool DoAssignEmp = true;
                    if (!NeedMoney && TotalMoneyNeed > currentMoney)
                    {
                        NeedMoney = true;

                        moneyByAssign = currentMoney / list_Ass.Count;
                        if (moneyByAssign < 10)
                        {
                            DoAssignEmp = false;
                        }

                    }
                    if (DoAssignEmp)
                    {
                        foreach (var assT in list_Ass)
                        {
                            if(moneyByAssign > -1)
                                ((Buy)assT).Money = moneyByAssign;
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
                    else
                    {
                        foreach (var assT in list_Ass)
                        {
                            AssignEnd(assT, false);
                        }
                    }

                }

                yield return new WaitForSeconds(1f);



                //Assign to Sell
                var list_itemRefSell = new List<ItemCheckNeed>();
                var list_SellAssign = new List<Sell>();

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
                        if (List_Employe.Count(x => x.IsWorking && x.IsDoMainAssign()) / NumberPosteMax > 0.7f)
                        {

                            it.Amount = itAmount;
                            list_itemRefSell.Add(it);
                            it.IsCurrentActivate = true;
                        }
                    }
                }

                if (list_itemRefSell.Count > 0)
                {
                    var v = new Sell()
                    {
                        IsMainAssignement = false,
                        BatimentProduction = this,
                        TypeAssignement = TypeAssignement.Sell,
                        Pos_Batiment = Controller.transform.position,
                        Money = 0,
                        Shop = Controller.GetShopHigherPriceForItem(list_itemRefSell[0].ItemRef) //ATTENTION ICI A REFAIRE  !! ! oui oui on veux que le premier au plus chère
                    };
                    v.List_Item = ItemCheckNeed.ConvertList_ItemBuy_to_ItemAmount(list_itemRefSell);
                    var emp = GetFreeNPCController();
                    if (emp != null)
                    {
                        emp.Assign(v);
                    }
                    else
                    {
                        AssignEnd(v, false);
                    }

                }
                //Assing CheckPrice

                if(DateTime.Now - DateLastTime_CheckPrice > TimerMax_CheckInfo && !b_SearchInfo)
                {
                    b_SearchInfo = true;
                    var searchInfo = new SearchInfo()
                    {
                        IsMainAssignement = false,
                        BatimentProduction = this,
                        TypeAssignement = TypeAssignement.SearchInfo,
                        Pos_Batiment = Controller.transform.position,
                        List_Shop = Controller.GetAllShop()
                    };
                    var emp = GetFreeNPCController();
                    if (emp != null)
                    {
                        emp.Assign(searchInfo);
                    }
                    else
                    {
                        AssignEnd(searchInfo, false);
                    }
                    DateLastTime_CheckPrice = DateTime.Now;
                }



                yield return new WaitForSeconds(1f);
            }
        }



        public NPCController GetFreeNPCController()
        {
            NPCController ret = null;
            if (List_Employe.Count > 0)
            {
                ret = List_Employe[0];
                foreach (var npc in List_Employe)
                {
                    ret = ret.CountSecAssign() > npc.CountSecAssign() ? npc : ret;
                }
            }
            return ret;


        }
    
        public IEnumerator RoutineProdItemCreate()
        {
            float etatWork = 0;
            while (Controller.BatIsActive())
            {
                foreach(var itr in List_ItemCreate)
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
                    if (createItem)
                    {
                        while (etatWork < itr.baseWorkingNeed)
                        {
                            yield return new WaitForSeconds(0.1f);
                            etatWork += (List_Employe.Count(x => x.IsWorking && x.IsDoMainAssign()) / 10.0f);
                        }
                        AddItemStock(itr, itr.AmountByWorking);
                        DateLastProduction = DateTime.Now;
                    }
                    etatWork = 0;
                    //Debug.Log("ici");
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        public IEnumerator CheckDateLastProduction()
        {
            while(Controller.BatIsActive())
            {
                if((DateTime.Now - DateLastProduction) > TimeMax_Seours && !SecourEnCours)
                {
                    SecourEnCours = true;
                    yield return ResetBatiment();
                }
                yield return new WaitForSeconds(1f);
            }
        }

        public IEnumerator ResetBatiment()
        {


            Controller.StopCoroutineWork(); //Stop coroutine => Sell All => Then restart Coroutine

            Debug.Log($"Ok ResetBatiment call ");

            var AllItemInStock = Controller.GetStock().GetAllItemInStock();
            var list_itemRefSell = new List<Sell>();

            foreach (var it in AllItemInStock)
            {
                var BestShop = Controller.GetShopHigherPriceForItem(it.ItemRef);
                var SellExist = list_itemRefSell.Find(x => x.Shop == BestShop);
                if(SellExist != null)
                {
                    SellExist.List_Item.Add(new ItemAmount(it.ItemRef, it.Amount));
                }
                else
                {
                    var v = new Sell()
                    {
                        IsMainAssignement = false,
                        BatimentProduction = this,
                        TypeAssignement = TypeAssignement.Sell,
                        Pos_Batiment = Controller.transform.position,
                        Money = 0,
                        Shop = BestShop
                    };
                    v.List_Item.Add(new ItemAmount(it.ItemRef, it.Amount));
                }

            }
            //Assign

            foreach( var sell in list_itemRefSell)
            {
                var emp = GetFreeNPCController();
                if (emp != null)
                {
                    emp.Assign(sell);
                }
                else
                {
                    AssignEnd(sell, false);
                }
            }
            yield return new WaitForSeconds(15f);
            Controller.StartCoroutineWork();
            DateLastProduction = DateTime.Now;
            SecourEnCours = false;

        }
        internal void AddListItem(List<ItemAmount> list_StockItemWork)
        {
            foreach (var ita in list_StockItemWork.Where(x => x.Amount > 0))
            {
                Controller.GetStock().Add(ita);
                List_ItemNeedBuy.FirstOrDefault(x => x.ItemRef == ita.ItemRef).IsCurrentActivate = false;
            }
        }

        internal void AddMoney(int workMoney)
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
            if (sItr > 0)
            {
                Controller.GetStock().Remove(ita.ItemRef, sItr);
                return new ItemAmount() { ItemRef = ita.ItemRef, Amount = sItr >= ita.Amount ? ita.Amount : sItr };
            }
            else
            {
                return null;
            }

        }


        internal int GetMoneyForAss(Buy buy)
        {
            if (Controller.GetMoneyComponement().GetMoney() >= buy.Money)
            {
                Controller.GetMoneyComponement().RemoveMoney(buy.Money);
                lock (this)
                {
                    NeedMoney = false;
                }
                return buy.Money;
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
       
        internal void AssignEnd(Assignement work, bool succed)
        {
            switch (work.TypeAssignement)
            {
                case TypeAssignement.Buy:
                    Buy buy = ((Buy)work);
                    DesactivateAll(buy.List_Item, List_ItemNeedBuy);

                    Controller.UpdateMemory(buy.List_UpdateShopInfo, buy.Shop);
                    break;
                case TypeAssignement.Sell:
                    Sell sell = ((Sell)work);
                    DesactivateAll(sell.List_Item, List_ItemNeedSell);
                    Controller.UpdateMemory(sell.List_UpdateShopInfo, sell.Shop);
                    break;
                case TypeAssignement.SearchInfo:
                    SearchInfo search = (SearchInfo)work;
                    foreach(var infoShop in search.Dict_Shop_ListInfoItem)
                    {
                        Controller.UpdateMemory(infoShop.Value, infoShop.Key);                        
                    }
                    b_SearchInfo = false;
                    break;
            }
        }

        private void DesactivateAll(List<ItemAmount> lista, List<ItemCheckNeed> lisNeed)
        {
            foreach (var it in lista)
            {
                var ite = lisNeed.First(x => x.ItemRef == it.ItemRef);
                if (ite != null)
                {
                    ite.IsCurrentActivate = false;
                }
            }
        }
        public void InitStock()
        {
            List<ItemRef> ItemNeed = new List<ItemRef>();
            foreach (var it in List_ItemCreate)
            {
                if (!ItemNeed.Any(x => x == it))
                {
                    ItemNeed.Add(it);

                    List_ItemNeedSell.Add(new ItemCheckNeed() { ItemRef = it, Amount = 0, IsCurrentActivate = false });
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
