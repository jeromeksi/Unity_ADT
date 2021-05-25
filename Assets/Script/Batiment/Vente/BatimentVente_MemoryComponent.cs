using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Batiment.BatimentVente
{
    public class BatimentVente_MemoryComponent
    {
        //Sotckage des prix des objets en stocks
        //Prix Achat =/= Prix vente

        List<InfoItemRef> list_InfoItemRef;

        public BatimentVente_Controller Controller;
        public BatimentVente_MemoryComponent(BatimentVente_Controller _Controller)
        {
            Controller = _Controller;
            list_InfoItemRef = new List<InfoItemRef>();
        }
        public void AddItem(ItemStockInit ItemStockInit)
        {
            AddItem(ItemStockInit.ItemRef, ItemStockInit.PriceBuy, ItemStockInit.PriceSell, null);
        }
        public void AddItem(ItemRef itemRef, int PriceBuy, int PriceSell, float? PercentMarge)
        {
            var ii = list_InfoItemRef.Find(x => x.ItemRef == itemRef);
            if(ii != null)
            {
                ii.PriceBuy = PriceBuy;
                ii.PriceSell = PriceSell;
                ii.PercentMarge = PercentMarge;
            }
            else
            {
                list_InfoItemRef.Add(new InfoItemRef(itemRef,PriceBuy,PriceSell));
            }
            UpdateMemory();
        }

        public List<InfoItemRef> GetAllInfoItemRef()
        {
            UpdateMemory();
            return list_InfoItemRef;
        }
        internal InfoItemRef GetInfoItemRef(ItemRef it)
        {
            UpdateMemory();
            return list_InfoItemRef.Find(x => x.ItemRef == it);
        }
        private void UpdateMemory()
        {
            foreach(var iir in list_InfoItemRef)
            {
                iir.HadStock = Controller.Stock.GetStock().GetAmount(iir.ItemRef) > 0;
            }
        }
    }
    public class InfoItemRef
    {
        public ItemRef ItemRef;
        public int PriceBuy;
        public int PriceSell;
        public bool HadStock;
        public float? PercentMarge;

        public InfoItemRef(ItemRef ir,int pb,int ps)
        {
            HadStock = true;
            ItemRef = ir;
            PriceBuy = pb;
            PriceSell = ps;

            PercentMarge = null;

        }

        public void SetPriceBuy(int pb)
        {
            PriceBuy = pb;
            CalculateMarge();
        }
        public void SetPriceSell(int ps)
        {
            PriceSell = ps;
            CalculateMarge();
        }
        public void CalculateMarge()
        {
            PercentMarge = (PriceSell - PriceBuy) / PriceSell;
        }
    }

}
