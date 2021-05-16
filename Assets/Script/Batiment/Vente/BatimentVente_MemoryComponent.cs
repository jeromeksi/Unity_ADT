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

        public BatimentVente_MemoryComponent()
        {
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
        }

    }

    public class InfoItemRef
    {
        public ItemRef ItemRef;
        public int PriceBuy;
        public int PriceSell;

        public float? PercentMarge;

        public InfoItemRef(ItemRef ir,int pb,int ps)
        {
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
