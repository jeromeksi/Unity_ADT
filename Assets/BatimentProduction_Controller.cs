using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Shop))]
[RequireComponent(typeof(BatimentProductionV2))]
public class BatimentProduction_Controller : MonoBehaviour
{

    private BatimentProductionV2 BatimentProductionV2;

    private Shop Shop;
    public StockBatiment StockBatiment;
    
    public Batiment_MemoryComponement MemoryComponement;
    public Batiment_DecisionComponement DecisionComponement;

    public BatimentProduction_InitComponent InitComponent;

    internal Batiment_MemoryComponement GetMemoryComponement()
    {
        return MemoryComponement;
    }

    public void Awake()
    {
        StockBatiment = new StockBatiment();
        MemoryComponement = new Batiment_MemoryComponement();
        DecisionComponement = new Batiment_DecisionComponement(this);
        TryGetComponent<Shop>(out Shop);
        TryGetComponent<BatimentProduction_InitComponent>(out InitComponent);
        TryGetComponent<BatimentProductionV2>(out BatimentProductionV2);

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
        mic.Add(InitComponent.Farine_ref, InitComponent.Shop_1, InitComponent.f_Price_1, InitComponent.Distance_1);
        mic.Add(InitComponent.Farine_ref, InitComponent.Shop_2, InitComponent.f_Price_2, InitComponent.Distance_2);
        mic.Add(InitComponent.Ble_ref, InitComponent.Shop_1, InitComponent.b_Price_1, InitComponent.Distance_1);
        mic.Add(InitComponent.Ble_ref, InitComponent.Shop_2, InitComponent.b_Price_2, InitComponent.Distance_2);

    }
    public Shop GetShopComptonent()
    {
        if (Shop != null)
            return Shop;
        throw new System.Exception("Pas de shop dans ce magasin");
    }
    public BatimentProductionV2 GetBatimentProductionV2Comptonent()
    {
        if (BatimentProductionV2 != null)
            return BatimentProductionV2;
        throw new System.Exception("Pas de shop dans ce BatimentProductionV2");
    }

    internal StockV2 GetStock()
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
}

