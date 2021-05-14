using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class StockBatiment
{
    public MoneyComponent MoneyComponent;

    public Stock Stock;

    public StockBatiment()
    {
        MoneyComponent = new MoneyComponent();
        Stock = new Stock();
    }
    public void SetStockMax( int Max)
    {
        Stock.stockMax = Max;
    }
    public Stock GetStock()
    {
        return Stock;
    }

    internal MoneyComponent GetMoneyComponement()
    {
        return MoneyComponent;
    }
}
