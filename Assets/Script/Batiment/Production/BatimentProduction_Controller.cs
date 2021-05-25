using Batiment;
using Batiment.BatimentVente;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace Batiment.BatimentProduction
{
    public class BatimentProduction_Controller : MonoBehaviour
    {

        public BatimentProduction_WorkComponent WorkComponent;

        private BatimentVente_Controller Shop;
        public StockBatiment StockBatiment;

        public BatimentProduction_MemoryComponement MemoryComponement;
        public BatimentProduction_DecisionComponement DecisionComponement;

        public BatimentProduction_InitComponent InitComponent;

        private bool IsActive;

        internal bool BatIsActive()
        {
            return IsActive;
        }


        public void ActiveBatiment()
        {
            IsActive = true;
            StartCoroutineWork();
        }
        public void DisactiveBatiment()
        {
            IsActive = false;
            StopCoroutineWork();
        }


        internal BatimentProduction_MemoryComponement GetMemoryComponement()
        {
            return MemoryComponement;
        }

        public void Awake()
        {
            StockBatiment = new StockBatiment();
            MemoryComponement = new BatimentProduction_MemoryComponement();
            DecisionComponement = new BatimentProduction_DecisionComponement(this);
            TryGetComponent<BatimentVente_Controller>(out Shop);
            TryGetComponent<BatimentProduction_InitComponent>(out InitComponent);
            WorkComponent = new BatimentProduction_WorkComponent();
            WorkComponent.Controller = this;

        }

        public void Start()
        {

        }
        public bool StartInit;
        public void Update()
        {
            if(StartInit)
            {
                if (InitComponent != null)
                    InitForTest();
                StartInit = false;
            }
        }
        public void InitForTest()
        {
            //init stock
            StockBatiment.SetStockMax(InitComponent.StockMaxStart);
            StockBatiment.GetMoneyComponement().AddMoney(InitComponent.MoneyStart);

            //BatimentProductionV2
            WorkComponent.SetNumberPosteMax(InitComponent.NumberPosteMax);
            foreach (var emp in InitComponent.List_Employe)
            {
                WorkComponent.AddEmploye(emp);
            }

            foreach(ItemRef it in InitComponent.List_ItemCreate)
            {
                WorkComponent.List_ItemCreate.Add(it);
            }
            WorkComponent.InitStock();

            //init Memory
            var mic = MemoryComponement.GetMemoryItemComponent();

            
            foreach(var it in InitComponent.List_ItemCreate)
            {
                foreach(var shop  in InitComponent.List_Shop)
                {
                    mic.Add(it, shop, shop.GetItemPrice(it));
                    foreach (var itr in it.Recipe)
                    {

                        mic.Add(itr.ItemRef, shop, shop.GetItemPrice(itr.ItemRef));
                    }
                }
            }
            ActiveBatiment();
        }
        public void UpdateMemory(List<InfoItemRef> list_update,BatimentVente_Controller shop)
        {
            var mic = MemoryComponement.GetMemoryItemComponent();
            foreach (var iir in list_update)
            {
                mic.Add(iir.ItemRef, shop, iir);
            }
        }


        internal int GetStockMAx()
        {
            return StockBatiment.GetStockMax();
        }

        public BatimentVente_Controller GetShopComptonent()
        {
            if (Shop != null)
                return Shop;
            throw new System.Exception("Pas de shop dans ce magasin");
        }
        public BatimentProduction_WorkComponent GetBatimentProductionV2Comptonent()
        {
            if (WorkComponent != null)
                return WorkComponent;
            throw new System.Exception("Pas de shop dans ce BatimentProductionV2");
        }

        internal Stock GetStock()
        {
            return StockBatiment.GetStock();
        }

        internal int? GetLowerPriceForItem(ItemRef itemRef)
        {

            return DecisionComponement.GetLowerPriceForItem(itemRef);
        }

        internal MoneyComponent GetMoneyComponement()
        {
            return StockBatiment.GetMoneyComponement();
        }
        public BatimentVente_Controller GetShopHigherPriceForItem(ItemRef itemRef)
        {
            return DecisionComponement.GetShopHigherPriceForItem(itemRef);
        }
        public BatimentVente_Controller GetShopLowerPriceForItem(ItemRef itemRef)
        {
            return DecisionComponement.GetShopLowerPriceForItem(itemRef);
        }

        internal void StopCoroutineWork()
        {
            StopCoroutine(WorkComponent.RoutineProdItemCreate());
            StopCoroutine(WorkComponent.AssignWork());
            //StopCoroutine(WorkComponent.CheckDateLastProduction());
        }

        internal void StartCoroutineWork()
        {
            StartCoroutine(WorkComponent.RoutineProdItemCreate());
            StartCoroutine(WorkComponent.AssignWork());
            //StartCoroutine(WorkComponent.CheckDateLastProduction());
        }

        internal List<BatimentVente_Controller> GetAllShop()
        {
            return MemoryComponement.GetMemoryItemComponent().GetAllShop();
        }
    }


}
