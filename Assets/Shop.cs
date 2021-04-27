using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public List<ItemRef> List_ItemBuy = new List<ItemRef>();
    List<ItemPrice> List_ItemPrice = new List<ItemPrice>();
    void Start()
    {
        InitValue();
    }

    public float PriceForItem(ItemRef it)
    {
        return List_ItemPrice.First(x => x.ItemRef == it).GetPriceIndex();
    }
    public float BuyItem(ItemRef it, float amount)
    {
        var price = it.GetBasePrice() * amount;


        return price;
    }
    public Transaction SellItem(ItemAmount ita, float money)
    {
        int nbItem = 0;

        var prixPourItem = PriceForItem(ita.ItemRef);
        float valTotalMaxitem = prixPourItem * ita.Amount;

        if(valTotalMaxitem <= money)
        {
            nbItem = ita.Amount;
        }
        else
        {
            nbItem = Convert.ToInt32(ita.Amount - ((valTotalMaxitem - money) / prixPourItem));
        }
        var transct = new Transaction()
        { ItemAmount = new ItemAmount(ita.ItemRef, nbItem),
            Money = nbItem * prixPourItem
        };

        return transct;

    }

    private void InitValue()
    {
        foreach(var it in List_ItemBuy)
        {
            List_ItemPrice.Add(new ItemPrice()
            { 
                ItemRef = it,
                IndiceNeed = 1,
                CurrentPrice = it.BasePrice
            });
        }
    }

    public class ItemPrice
    {
        public ItemRef ItemRef { get; set; }
        public float CurrentPrice { get; set; }
        public float IndiceNeed { get; set; }

        public float GetPriceIndex()
        {
            return IndiceNeed * ItemRef.GetBasePrice();
        }
    }
}
public struct Transaction
{
    public ItemAmount ItemAmount;
    public float Money;
}