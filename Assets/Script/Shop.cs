using Batiment.BatimentProduction;
using Batiment.BatimentProduction.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

public class Shop : MonoBehaviour
{
    //public List<ItemRef> List_ItemBuy = new List<ItemRef>();
    [SerializeField] public List<ItemPrice> List_ItemPrice = new List<ItemPrice>();

    [Range(0.01f, 2f)]
    public float Index;

    void Awake()
    {
        //InitValue();
    }

    public int? PriceForItem(ItemRef it)
    {
        try
        {
            int? ret = Convert.ToInt32(List_ItemPrice.First(x => x.ItemRef == it).GetPriceIndex() * Index);

            return ret <1 ? 1:ret;
        }
        catch
        {
            return null;
        }
    }
    public Transaction BuyItem(ItemRef it, float amount)
    {
        int? priceItem = PriceForItem(it);
        Transaction ret = null;

        if(priceItem.HasValue)
        {
            ret = new Transaction()
            {
                ItemAmount = new ItemAmount(it),
                Money = Convert.ToInt32(priceItem.Value * amount)
            };
        }
        return ret;

    }
    public Transaction SellItem(ItemAmount ita, float money)
    {
        int nbItem = 0;

        var prixPourItem = PriceForItem(ita.ItemRef);
        if(prixPourItem != null)
        {
            float valTotalMaxitem = prixPourItem.Value * ita.Amount;

            if (valTotalMaxitem <= money)
            {
                nbItem = ita.Amount;
            }
            else
            {
                nbItem = Convert.ToInt32(ita.Amount - ((valTotalMaxitem - money) / prixPourItem));
            }
            var transct = new Transaction()
            {
                ItemAmount = new ItemAmount(ita.ItemRef, nbItem),
                Money = nbItem * prixPourItem.Value               
            };
            return transct;
        }
        else
        {
            return null;
        }

    }

    //private void InitValue()
    //{
    //    foreach(var it in List_ItemBuy)
    //    {
    //        List_ItemPrice.Add(new ItemPrice()
    //        { 
    //            ItemRef = it,
    //            BasePrice = it.BasePrice
    //        });
    //    }
    //}

    public List<UpdateShopInfo> GetAllItemPrice()
    {
        List<UpdateShopInfo> lisT = new List<UpdateShopInfo>();
        foreach(var ib in List_ItemPrice)
        {
            lisT.Add(new UpdateShopInfo()
            {
                ItemRef = ib.ItemRef,
                ShopInfo = new ShopInfo()
                {
                    Shop = this,
                    Price = PriceForItem(ib.ItemRef)
                }
            });
        }

        return lisT;
    }
}
public class Transaction
{
    public ItemAmount ItemAmount;
    public int Money;
}
[Serializable]
public class ItemPrice
{
    [SerializeField] public ItemRef ItemRef;
    [Range(0.01f, 10f)]
    public float ModifPrice;

    public int GetPriceIndex()
    {
        return Convert.ToInt32(ModifPrice * ItemRef.GetBasePrice());
    }
}
