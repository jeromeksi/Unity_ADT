using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Batiment.BatimentVente
{
    public class BatimentVente_InitComponent : MonoBehaviour
    {

        //Stock
        public int StockMax;
        public int MoneyStart;
        public List<ItemStockInit> List_ItemStockInit;
        public float IndexShop;
        //Work
        public int EmpMax;
        public List<NPCController> List_Emp;
    }
    [Serializable]
    public class ItemStockInit
    {
        public ItemRef ItemRef;

        public int PriceBuy;
        public int PriceSell;

        public int Amount;
    }
}
