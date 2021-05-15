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

        public Shop GetShopHigherPriceForItem(ItemRef itemRef)
        {
            var mc = Controller.GetMemoryComponement();

            var mi = mc.GetMemoryItemComponent().GetInfo(itemRef);

            if (mi != null)
            {
                float price = -1;
                Shop shopRet = null;
                foreach (var shopInfo in mi.List_ShopInfo)
                {
                    if (shopInfo.Price.HasValue && shopInfo.Price.Value > price)
                    {
                        shopRet = shopInfo.Shop;
                        price = shopInfo.Price.Value;
                    }
                }
                return shopRet;
            }
            return null;
        }
        public Shop GetShopLowerPriceForItem(ItemRef itemRef)
        {
            var mc = Controller.GetMemoryComponement();

            var mi = mc.GetMemoryItemComponent().GetInfo(itemRef);

            if (mi != null)
            {
                float price = Mathf.Infinity;
                Shop shopRet = null;
                foreach (var shopInfo in mi.List_ShopInfo)
                {
                    if (shopInfo.Price.HasValue && shopInfo.Price.Value < price)
                    {
                        shopRet = shopInfo.Shop;
                        price = shopInfo.Price.Value;
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
                    if (shopInfo.Price.HasValue && shopInfo.Price.Value < price)
                    {
                        price = shopInfo.Price.Value;
                    }
                }
                return price;
            }
            return null;
        }
    }
}