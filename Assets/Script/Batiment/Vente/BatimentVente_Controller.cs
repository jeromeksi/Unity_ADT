using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Batiment.BatimentVente
{
    [RequireComponent(typeof(BatimentVente_DecisionComponent))]
    [RequireComponent(typeof(BatimentVente_WorkComponent))]
    [RequireComponent(typeof(BatimentVente_MemoryComponent))]
    public class BatimentVente_Controller : MonoBehaviour
    {
        public BatimentVente_InitComponent InitComponent;

        public BatimentVente_MemoryComponent MemoryComponent;
        public BatimentVente_WorkComponent WorkComponent;
        public BatimentVente_DecisionComponent DecisionComponent;


        public StockBatiment Stock;
        public void Start()
        {
            Stock = new StockBatiment();

            TryGetComponent(out MemoryComponent);
            TryGetComponent(out WorkComponent);
            TryGetComponent(out DecisionComponent);
            TryGetComponent(out InitComponent);

            if (InitComponent != null)
            {
                InitBat();
            }
        }

        private void InitBat()
        {
            //Init Stock
            Stock.SetStockMax(InitComponent.StockMax);
            Stock.GetMoneyComponement().AddMoney(InitComponent.MoneyStart);

            //Init Mémoire
    

        }
    }
}
