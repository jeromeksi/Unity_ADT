using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BatimentProduction_WorkComponent))]
public class BatimentProduction_Controller : MonoBehaviour
{

    private BatimentProduction_WorkComponent BatimentProductionV2;

    private Shop Shop;
    public StockBatiment StockBatiment;
    
    public BatimentProduction_MemoryComponement MemoryComponement;
    public BatimentProduction_DecisionComponement DecisionComponement;

    public BatimentProduction_InitComponent InitComponent;



    internal BatimentProduction_MemoryComponement GetMemoryComponement()
    {
        return MemoryComponement;
    }

    public void Awake()
    {
        StockBatiment = new StockBatiment();
        MemoryComponement = new BatimentProduction_MemoryComponement();
        DecisionComponement = new BatimentProduction_DecisionComponement(this);
        TryGetComponent<Shop>(out Shop);
        TryGetComponent<BatimentProduction_InitComponent>(out InitComponent);
        TryGetComponent<BatimentProduction_WorkComponent>(out BatimentProductionV2);

        BatimentProductionV2.Controller = this;
        if(InitComponent != null)
            InitForTest();
    }

    public void InitForTest()
    {
        //init stock
        StockBatiment.SetStockMax(InitComponent.StockMaxStart);
        StockBatiment.MoneyComponent.AddMoney(InitComponent.MoneyStart);

        //BatimentProductionV2
        BatimentProductionV2.SetNumberPosteMax(InitComponent.NumberPosteMax);
        foreach (var emp in InitComponent.List_Employe)
        {
            BatimentProductionV2.AddEmploye(emp);
        }



        //init Memory
        var mic = MemoryComponement.GetMemoryItemComponent();
        mic.Add(InitComponent.Farine_ref, InitComponent.Shop_1, InitComponent.Shop_1.PriceForItem(InitComponent.Farine_ref), InitComponent.Distance_1);
        mic.Add(InitComponent.Farine_ref, InitComponent.Shop_2, InitComponent.Shop_2.PriceForItem(InitComponent.Farine_ref), InitComponent.Distance_1);

        mic.Add(InitComponent.Ble_ref, InitComponent.Shop_1, InitComponent.Shop_1.PriceForItem(InitComponent.Ble_ref), InitComponent.Distance_1);
        mic.Add(InitComponent.Ble_ref, InitComponent.Shop_2, InitComponent.Shop_2.PriceForItem(InitComponent.Ble_ref), InitComponent.Distance_1);

        mic.Add(InitComponent.Orge_ref, InitComponent.Shop_1, InitComponent.Shop_1.PriceForItem(InitComponent.Orge_ref), InitComponent.Distance_1);
        mic.Add(InitComponent.Orge_ref, InitComponent.Shop_2, InitComponent.Shop_2.PriceForItem(InitComponent.Orge_ref), InitComponent.Distance_1);


    }

    internal int GetStockMAx()
    {
        return StockBatiment.Stock.stockMax;
    }

    public Shop GetShopComptonent()
    {
        if (Shop != null)
            return Shop;
        throw new System.Exception("Pas de shop dans ce magasin");
    }
    public BatimentProduction_WorkComponent GetBatimentProductionV2Comptonent()
    {
        if (BatimentProductionV2 != null)
            return BatimentProductionV2;
        throw new System.Exception("Pas de shop dans ce BatimentProductionV2");
    }

    internal Stock GetStock()
    {
        return StockBatiment.GetStock();
    }
    internal MoneyComponent GetMoneyComponement()
    {
        return StockBatiment.GetMoneyComponement();
    }
    public Shop GetShopHigherPriceForItem(ItemRef itemRef)
    {
        return DecisionComponement.GetShopHigherPriceForItem(itemRef);
    }
    public Shop GetShopLowerPriceForItem(ItemRef itemRef)
    {
        return DecisionComponement.GetShopLowerPriceForItem(itemRef);
    }

    internal void AddMemoryInfo(UpdateShopInfo updateShopInfo)
    {
        MemoryComponement.memoryItem.Add(updateShopInfo);
    }
}

