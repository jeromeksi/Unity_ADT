using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Batiment
{
    [Serializable]
    public class StockBatiment
    {
        private MoneyComponent MoneyComponent;
        private Stock Stock;

        public StockBatiment()
        {
            MoneyComponent = new MoneyComponent();
            Stock = new Stock();
        }
        public void SetStockMax(int Max)
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

        internal int GetStockMax()
        {
            return Stock.stockMax;
        }
    }

}
