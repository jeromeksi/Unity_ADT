using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Batiment.BatimentVente
{
    [Serializable]
    public class BatimentVente_ShopComponent
    {
        //Ici C'est l'interface BUY/SELL du magasin
        public bool CanShop;
        public BatimentVente_Controller Controller;

        [Range(0.01f, 2f)]
        public float Index; // Index des prix de ce magasin


        public BatimentVente_ShopComponent(BatimentVente_Controller cc)
        {
            Controller = cc;
        }

      
        public int? PriceBuyForItem(ItemRef it)
        {
            try
            {
                int? ret = Convert.ToInt32(Controller.GetMemoryComponent().GetInfoItemRef(it).PriceBuy * Index);

                return ret < 1 ? 1 : ret;
            }
            catch
            {
                return null;
            }
        }
        public int? PriceSellForItem(ItemRef it)
        {
            try
            {
                int? ret = Convert.ToInt32(Controller.GetMemoryComponent().GetInfoItemRef(it).PriceSell * Index);

                return ret < 1 ? 1 : ret;
            }
            catch
            {
                return null;
            }
        }
        public TransactionV2 BuyItem(ItemRef it, int amount) //Shop qui achète ! 
        {
            if(CanShop)
            {
                int? priceItem = PriceBuyForItem(it);

                TransactionV2 ret = null;

                if (priceItem.HasValue)
                {
                    Controller.Stock.GetStock().Add(it, amount);

                    //Verif qu'on a de la money
                    var currentMoney = Controller.Stock.GetMoneyComponement().GetMoney();

                    if (currentMoney < priceItem * amount)
                    {
                        amount = Convert.ToInt32(amount - ((priceItem * amount - currentMoney) / priceItem));
                    }

                    Controller.Stock.GetMoneyComponement().RemoveMoney(priceItem.Value * amount);

                    ret = new TransactionV2()
                    {
                        TypeTransaction = TypeTransaction.Buy,
                        ItemRef = it,
                        Amount = amount,
                        Money = priceItem.Value * amount,
                        PriceItem = priceItem.Value
                    };
                    
                }
                return ret;
            }
            return null;
        }
        public TransactionV2 SellItem(ItemRef it,int amount, float money)
        {
            if (CanShop)
            {
                int nbItem = 0;

                var prixPourItem = PriceSellForItem(it);
                var amountItemInStock = Controller.Stock.GetStock().GetAmount(it);
                if (prixPourItem != null && amountItemInStock > 0)
                {
                    int amountSell;
                    if (amountItemInStock >= amount)
                    {
                        amountSell = amount;
                    }
                    else
                    {
                        amountSell = amountItemInStock;
                    }


                    float valTotalMaxitem = prixPourItem.Value * amountSell;

                    if (valTotalMaxitem <= money)
                    {
                        nbItem = amount;
                    }
                    else
                    {
                        nbItem = Convert.ToInt32(amount - ((valTotalMaxitem - money) / prixPourItem));
                    }

                    Controller.Stock.GetStock().Remove(it, nbItem);


                    var transct = new TransactionV2()
                    {
                        TypeTransaction = TypeTransaction.Sell,
                        ItemRef = it,
                        Money = nbItem * prixPourItem.Value,
                        Amount = nbItem,
                        PriceItem = prixPourItem.Value
                    };
                    return transct;
                }
            }
            return null;

        }
        public List<InfoItemRef> GetAllItemPrice()
        {
            if(CanShop)
            {
                return Controller.GetMemoryComponent().GetAllInfoItemRef();
            }
            return null;
            
        }

    }
    [Serializable]
    public class TransactionV2
    {
        public TypeTransaction TypeTransaction;
        public ItemRef ItemRef;
        public int Money;
        public int Amount;
        public int PriceItem;
    }
    public enum TypeTransaction
    {
        Buy,
        Sell
    }
}
