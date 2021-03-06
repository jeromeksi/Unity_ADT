using Batiment.BatimentProduction;
using Batiment.BatimentVente;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Util;

namespace Assets.Script.Batiment.Assignement_Work
{
    [Serializable]
    public abstract class Assignement
    {
        public bool IsMainAssignement;
        public TypeAssignement TypeAssignement;
        public NPC_WorkComponement WorkComponement;
        public NPCController NPCController;
        public abstract IEnumerator DoWork();
        public abstract void EndAssign(bool Succed);

        public void SetController(NPCController _controller)
        {
            NPCController = _controller;
            WorkComponement = NPCController.GetWorkComponent();
        }
    }

    [Serializable]
    public class Buy : Assignement
    {
        public TypeBuy TypeBuy;
        public BatimentProduction_WorkComponent BatimentProduction;

        public BatimentVente_Controller Shop;
        public Vector3 Pos_Batiment;
        public List<InfoItemRef> List_UpdateShopInfo = new List<InfoItemRef>();

        public int Money;
        public List<ItemAmount> List_Item = new List<ItemAmount>();

        public override IEnumerator DoWork()
        {
            NPCController.SetDestination(Pos_Batiment);


            yield return NPCController.CheckArrive();

            //Take Money ou Item
          

            WorkComponement.WorkMoney = BatimentProduction.GetMoneyForAss(this);


            if (WorkComponement.WorkMoney > 0 || WorkComponement.List_StockItemWork.Count > 0)
            {

                //GO to shop
                NPCController.SetDestination(Shop.gameObject.transform.position);


                yield return NPCController.CheckArrive();



                if(Shop.GetCanShop())
                {
                    switch(TypeBuy)
                    {
                        case TypeBuy.Order:
                            foreach (var ita in List_Item)
                            {
                                var trasact = Shop.SellItem(NPCController, ita.ItemRef, ita.Amount, WorkComponement.WorkMoney);
                                if (trasact != null)
                                {
                                    WorkComponement.List_StockItemWork.Add(new ItemAmount(trasact.ItemRef, trasact.Amount));
                                    WorkComponement.WorkMoney -= trasact.Money;
                                }
                            }
                            break;
                        case TypeBuy.Share:
                            var moneyByBuy = WorkComponement.WorkMoney / List_Item.Count;
                            foreach (var ita in List_Item)
                            {
                                var trasact = Shop.SellItem(NPCController, ita.ItemRef, ita.Amount, moneyByBuy);
                                if (trasact != null)
                                {
                                    WorkComponement.List_StockItemWork.Add(new ItemAmount(trasact.ItemRef, trasact.Amount));
                                    WorkComponement.WorkMoney -= trasact.Money;
                                }
                            }
                            break;
                    }
                   
                    List_UpdateShopInfo = Shop.GetAllItemPrice(NPCController);
                }




                //Goto Bat

                NPCController.SetDestination(Pos_Batiment);


                yield return NPCController.CheckArrive();

                //decharge
                BatimentProduction.AddMoney(WorkComponement.WorkMoney);
                WorkComponement.WorkMoney = 0;
                BatimentProduction.AddListItem(WorkComponement.List_StockItemWork);
                WorkComponement.List_StockItemWork.Clear();
                EndAssign(true);
            }
            else
            {
                EndAssign(false);
            }
        }

        public override void EndAssign(bool Succed)
        {
            BatimentProduction.AssignEnd(this, Succed);
        }
    }



    [Serializable]
    public class Sell : Assignement
    {

        public BatimentProduction_WorkComponent BatimentProduction;
        public Vector3 Pos_Batiment;

        public BatimentVente_Controller Shop;
        public Vector3 Destination;
        public List<InfoItemRef> List_UpdateShopInfo = new List<InfoItemRef>();

        public int Money;
        public List<ItemAmount> List_Item = new List<ItemAmount>();

        public override IEnumerator DoWork()
        {
            NPCController.SetDestination(Pos_Batiment);


            yield return NPCController.CheckArrive();

            //Take Money ou Item

            foreach (var ita in List_Item)
            {
                var itas = BatimentProduction.GetItemAmountSell(ita);
                if (itas != null)
                {
                    WorkComponement.List_StockItemWork.Add(itas);
                }
            }
                

            if (WorkComponement.List_StockItemWork.Count > 0)
            {

                //GO to shop
                NPCController.SetDestination(Shop.gameObject.transform.position);


                yield return NPCController.CheckArrive();


                //Vendre ou Buy
               
                foreach (var ita in WorkComponement.List_StockItemWork)
                {
                    var transact = Shop.BuyItem(NPCController, ita.ItemRef, ita.Amount);
                    if (transact != null)
                    {
                        WorkComponement.WorkMoney += transact.Money;
                        ita.Amount -= ita.Amount;
                    }
                }
                   
                List_UpdateShopInfo = Shop.GetAllItemPrice(NPCController);


                //Goto Bat

                NPCController.SetDestination(Pos_Batiment);


                yield return NPCController.CheckArrive();

                //decharge
                BatimentProduction.AddMoney(WorkComponement.WorkMoney);
                WorkComponement.WorkMoney = 0;
                BatimentProduction.AddListItem(WorkComponement.List_StockItemWork);
                WorkComponement.List_StockItemWork.Clear();
                EndAssign(true);
            }
            else
            {
                EndAssign(false);
            }
        }

        public override void EndAssign(bool Succed)
        {
            BatimentProduction.AssignEnd(this, Succed);
        }
    }

    [Serializable]
    public class BatimentProduction_Work : Assignement
    {
        public bool IsMainAssignement;
        public Vector3 Pos_BatimentProduction;


        public override IEnumerator DoWork()
        {
            if (Pos_BatimentProduction != null)
            {
                NPCController.SetDestination(Pos_BatimentProduction);
                yield return NPCController.CheckArrive();
            }

        }

        public override void EndAssign(bool Succed)
        {

        }
    }

    [Serializable]
    public class BatimentVente_Work : Assignement
    {
        public bool IsMainAssignement;
        public Vector3 BatimentVente;
        public BatimentVente_WorkerComponent BatimentVente_cmp;

        public override IEnumerator DoWork()
        {
            if (BatimentVente != null)
            {
                NPCController.SetDestination(BatimentVente);
                yield return NPCController.CheckArrive();
            }
            EndAssign(true);
        }

        public override void EndAssign(bool Succed)
        {
            BatimentVente_cmp.AssignEnd(this, Succed);
        }
      
    }

    public class SearchInfo : Assignement
    {
        public Vector3 Pos_Batiment;
        public BatimentProduction_WorkComponent BatimentProduction;
        public List<BatimentVente_Controller> List_Shop = new List<BatimentVente_Controller>();
        public Dictionary<BatimentVente_Controller, List<InfoItemRef>> Dict_Shop_ListInfoItem = new Dictionary<BatimentVente_Controller, List<InfoItemRef>>();
        public override IEnumerator DoWork()
        {
            foreach(var shp in List_Shop)
            {
                //go to shop
                NPCController.SetDestination(shp.gameObject.transform.position);
                yield return NPCController.CheckArrive();

                Dict_Shop_ListInfoItem.Add(shp, shp.GetAllItemPrice(NPCController));
            }
            //go to Bat
            NPCController.SetDestination(Pos_Batiment);


            yield return NPCController.CheckArrive();
            EndAssign(true);
        }

        public override void EndAssign(bool Succed)
        {
            BatimentProduction.AssignEnd(this, true);
        }
    }
    public enum TypeBuy
    {
        Share,
        Order
    }
    public enum TypeAssignement
    {
        Buy,
        Sell,
        SearchInfo,
        Goto,
        Work
    }

}
