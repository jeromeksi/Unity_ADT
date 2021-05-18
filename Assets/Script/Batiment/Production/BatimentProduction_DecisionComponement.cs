using Batiment.BatimentVente;
using System;
using UnityEngine;



namespace Batiment.BatimentProduction
{
    public class BatimentProduction_DecisionComponement
    {
        //bon sérious busines ici
        //Ici, c'est le centre de décision
        //cad  des fonctions qui sont appelées pour tirer les meilleurs info de la mémoire..
        BatimentProduction_Controller Controller;

        public BatimentProduction_DecisionComponement(BatimentProduction_Controller batimentProduction_Controller)
        {
            Controller = batimentProduction_Controller;
        }

        public BatimentVente_Controller GetShopHigherPriceForItem(ItemRef itemRef)
        {
            var mc = Controller.GetMemoryComponement();

            var mi = mc.GetMemoryItemComponent().GetInfo(itemRef);

            if (mi != null)
            {
                float price = -1;
                BatimentVente_Controller shopRet = null;
                foreach (var shopInfo in mi.List_ShopInfo)
                {
                    if (shopInfo.PriceSell.HasValue && shopInfo.PriceSell.Value > price)
                    {
                        shopRet = shopInfo.Shop;
                        price = shopInfo.PriceSell.Value;
                    }
                }
                return shopRet;
            }
            return null;
        }
        public BatimentVente_Controller GetShopLowerPriceForItem(ItemRef itemRef)
        {
            var mc = Controller.GetMemoryComponement();

            var mi = mc.GetMemoryItemComponent().GetInfo(itemRef);

            if (mi != null)
            {
                float price = Mathf.Infinity;
                BatimentVente_Controller shopRet = null;
                foreach (var shopInfo in mi.List_ShopInfo)
                {
                    if (shopInfo.PriceBuy.HasValue && shopInfo.PriceBuy.Value < price)
                    {
                        shopRet = shopInfo.Shop;
                        price = shopInfo.PriceBuy.Value;
                    }
                }
                return shopRet;
            }
            return null;
        }

        internal int? GetLowerPriceForItem(ItemRef itemRef)
        {
            var mc = Controller.GetMemoryComponement();

            var mi = mc.GetMemoryItemComponent().GetInfo(itemRef);

            if (mi != null)
            {
                int price = int.MaxValue;
                foreach (var shopInfo in mi.List_ShopInfo)
                {
                    if (shopInfo.PriceSell.HasValue && shopInfo.PriceSell.Value < price)
                    {
                        price = shopInfo.PriceSell.Value;
                    }
                }
                return price;
            }
            return null;
        }
    }
}