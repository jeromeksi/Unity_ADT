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

        internal bool BatIsActive()
        {
            return IsActive;
        }

        private BatimentVente_ShopComponent ShopComponent;

        private BatimentVente_DecisionComponent DecisionComponent;

        public StockBatiment Stock;



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

            WorkComponent = new BatimentVente_WorkerComponent();
            WorkComponent.Controller = this;

            TryGetComponent(out DecisionComponent);
            TryGetComponent(out InitComponent);


            MemoryComponent = new BatimentVente_MemoryComponent();
            ShopComponent = new BatimentVente_ShopComponent();
        }
        public void Start()
        {
            
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

            //init work
            WorkComponent.SetNumberPosteMax(InitComponent.EmpMax);

            foreach(var emp in InitComponent.List_Emp)
            {
                WorkComponent.AddEmploye(emp);
            }
        }

        private void AddItemInStock(ItemStockInit itemStockInit)
        {
            //Stock
            Stock.GetStock().Add(itemStockInit.ItemRef, itemStockInit.Amount);

            //Mémoire
            MemoryComponent.AddItem(itemStockInit);
        }
    }
}
