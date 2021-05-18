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
            StartCoroutine(WorkComponent.RoutineProdItemCreate());
            StartCoroutine(WorkComponent.AssignWork());
        }
        public void DisactiveBatiment()
        {
            IsActive = false;
            StopCoroutine(WorkComponent.RoutineProdItemCreate());
            StopCoroutine(WorkComponent.AssignWork());
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

            mic.Add(InitComponent.Farine_ref, InitComponent.Shop_1, InitComponent.Shop_1.GetItemPrice(InitComponent.Farine_ref));
            mic.Add(InitComponent.Farine_ref, InitComponent.Shop_2, InitComponent.Shop_2.GetItemPrice(InitComponent.Farine_ref));

            mic.Add(InitComponent.Ble_ref, InitComponent.Shop_1, InitComponent.Shop_1.GetItemPrice(InitComponent.Ble_ref));
            mic.Add(InitComponent.Ble_ref, InitComponent.Shop_2, InitComponent.Shop_2.GetItemPrice(InitComponent.Ble_ref));

            mic.Add(InitComponent.Orge_ref, InitComponent.Shop_1, InitComponent.Shop_1.GetItemPrice(InitComponent.Orge_ref));
            mic.Add(InitComponent.Orge_ref, InitComponent.Shop_2, InitComponent.Shop_2.GetItemPrice(InitComponent.Orge_ref));

            //mic.Add(InitComponent.Farine_ref, InitComponent.Shop_1, InitComponent.Shop_1.PriceForItem(InitComponent.Farine_ref), InitComponent.Distance_1);
            //mic.Add(InitComponent.Farine_ref, InitComponent.Shop_2, InitComponent.Shop_2.PriceForItem(InitComponent.Farine_ref), InitComponent.Distance_1);

            //mic.Add(InitComponent.Ble_ref, InitComponent.Shop_1, InitComponent.Shop_1.PriceForItem(InitComponent.Ble_ref), InitComponent.Distance_1);
            //mic.Add(InitComponent.Ble_ref, InitComponent.Shop_2, InitComponent.Shop_2.PriceForItem(InitComponent.Ble_ref), InitComponent.Distance_1);

            //mic.Add(InitComponent.Orge_ref, InitComponent.Shop_1, InitComponent.Shop_1.PriceForItem(InitComponent.Orge_ref), InitComponent.Distance_1);
            //mic.Add(InitComponent.Orge_ref, InitComponent.Shop_2, InitComponent.Shop_2.PriceForItem(InitComponent.Orge_ref), InitComponent.Distance_1);

            ActiveBatiment();
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

    }


}
