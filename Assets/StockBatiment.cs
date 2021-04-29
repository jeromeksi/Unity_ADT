using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockBatiment : MonoBehaviour
{
    public MoneyComponent MoneyComponent;

    private StockV2 Stock;

    public StockBatiment()
    {
        MoneyComponent = new MoneyComponent();
        Stock = new StockV2();
    }

    //private void SuppItemStock(ItemRef itemRef, int amount)
    //{
    //    Stock.Remove(itemRef, amount);
    //}

    //private bool CheckItemAmount(ItemRef itemRef, int amount)
    //{
    //    return Stock.GetNumber(itemRef) >= amount;
    //}
    //private void AddItemStock(ItemRef itemRef, int amount)
    //{
    //    Stock.Add(itemRef, amount);

    //}
}
