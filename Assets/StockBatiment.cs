using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class StockBatiment
{
    public MoneyComponent MoneyComponent;

    public StockV2 Stock;

    public StockBatiment()
    {
        MoneyComponent = new MoneyComponent();
        Stock = new StockV2();
    }
    public void SetStockMax( int Max)
    {
        Stock.stockMax = Max;
    }
    public StockV2 GetStock()
    {
        return Stock;
    }

    internal MoneyComponent GetMoneyComponement()
    {
        return MoneyComponent;
    }
}
