using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockBatiment : MonoBehaviour
{
    public MoneyComponent MoneyComponent;

    private Stock Stock;

    public StockBatiment()
    {
        MoneyComponent = new MoneyComponent();
        Stock = new Stock();
    }
    

}
