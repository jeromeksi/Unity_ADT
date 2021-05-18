using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Batiment.BatimentVente
{
    [RequireComponent(typeof(BatimentVente_DecisionComponent))]
    public class BatimentVente_Controller : MonoBehaviour
    {
        private BatimentVente_InitComponent InitComponent;
        private BatimentVente_MemoryComponent MemoryComponent;
        private BatimentVente_WorkerComponent WorkComponent;

        private bool IsActive;

        private BatimentVente_ShopComponent ShopComponent;

        private BatimentVente_DecisionComponent DecisionComponent;

        public StockBatiment Stock;


        internal bool BatIsActive()
        {
            return IsActive;
        }

        internal BatimentVente_MemoryComponent GetMemoryComponent()
        {
            return MemoryComponent;
        }


        public void ActiveBatiment()
        {
            IsActive = true;
            StartCoroutine(WorkComponent.RoutineSell());
        }
        public void DisactiveBatiment()
        {
            IsActive = false;
            SetCanShop(false);
            StopCoroutine(WorkComponent.RoutineSell());
        }


        public void Awake()
        {
            Stock = new StockBatiment();

            WorkComponent = new BatimentVente_WorkerComponent(this);

            TryGetComponent(out DecisionComponent);
            TryGetComponent(out InitComponent);


            MemoryComponent = new BatimentVente_MemoryComponent();
            ShopComponent = new BatimentVente_ShopComponent(this);
            if (InitComponent != null)
            {
                InitBat();
                ActiveBatiment();
            }
        }


        public void SetCanShop(bool canShop)
        {
            ShopComponent.CanShop = canShop;
        }
        private void InitBat()
        {
            //Init Stock
            Stock.SetStockMax(InitComponent.StockMax);
            Stock.GetMoneyComponement().AddMoney(InitComponent.MoneyStart);

            //init stock et mémoire 
            foreach(var ItemStockInit in InitComponent.List_ItemStockInit)
            {
                AddItemInStock(ItemStockInit);
            }
            //shop 
            ShopComponent.Index = InitComponent.IndexShop;
            //init work
            WorkComponent.SetNumberPosteMax(InitComponent.EmpMax);

            foreach(var emp in InitComponent.List_Emp)
            {
                WorkComponent.AddEmploye(emp);
            }
        }

        internal bool GetCanShop()
        {
            return ShopComponent.CanShop;
        }

        private void AddItemInStock(ItemStockInit itemStockInit)
        {
            //Stock
            Stock.GetStock().Add(itemStockInit.ItemRef, itemStockInit.Amount);

            //Mémoire
            MemoryComponent.AddItem(itemStockInit);
        }

        internal InfoItemRef GetItemPrice(ItemRef farine_ref)
        {
            return MemoryComponent.GetInfoItemRef(farine_ref);
        }

        public bool CheckNPCControllerCanInteract(NPCController nPCController)
        {
            return Mathf.Abs(Vector3.Distance(this.transform.position, nPCController.transform.position)) < 3;
        }



        public int? PriceBuyForItem(NPCController nPCController,ItemRef it)
        {
            if(CheckNPCControllerCanInteract(nPCController))
                return ShopComponent.PriceBuyForItem(it);
            return null;
        }
        public int? PriceSellForItem(NPCController nPCController,ItemRef it)
        {

            if (CheckNPCControllerCanInteract(nPCController))
                return ShopComponent.PriceSellForItem(it);
            return null;
        }

        public TransactionV2 BuyItem(NPCController nPCController,ItemRef it, int amount)
        {
            if (CheckNPCControllerCanInteract(nPCController))
                return ShopComponent.BuyItem(it, amount);
            return null;
        }
        public TransactionV2 SellItem(NPCController nPCController,ItemRef it, int amount, float money)
        {

            if (CheckNPCControllerCanInteract(nPCController))
                return ShopComponent.SellItem(it, amount, money);
            return null;
        }
        public List<InfoItemRef> GetAllItemPrice(NPCController nPCController)
        {
            if (CheckNPCControllerCanInteract(nPCController))
                return ShopComponent.GetAllItemPrice();
            return null;
        }


    }
}
