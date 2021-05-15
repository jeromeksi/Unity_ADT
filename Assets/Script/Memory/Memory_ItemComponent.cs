using Batiment.BatimentProduction.Util;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Memory_ItemComponent
{
    public List<MemoryItem> MemoryItems;

    public Memory_ItemComponent()
    {
        MemoryItems = new List<MemoryItem>();
    }
    public void Add(ItemRef itemRef, Shop shop, int? price, float? distance)
    {
        var mi = MemoryItems.Find(x => x.ItemRef == itemRef);
        if (mi != null)
        {
            mi.AddInfo(shop,price,distance);
        }
        else
        {
            var miT = new MemoryItem();
            miT.ItemRef = itemRef;
            miT.AddInfo(shop, price, distance);
            MemoryItems.Add(miT);
        }
    }
    public void Add(UpdateShopInfo updateShopInfo)
    {
        Add(updateShopInfo.ItemRef, updateShopInfo.ShopInfo.Shop, updateShopInfo.ShopInfo.Price, updateShopInfo.ShopInfo.Distance);
    }


    public void Remove(ItemRef itemRef, Shop shop)
    {
        var mi = MemoryItems.Find(x => x.ItemRef == itemRef);
        if (mi != null )
        {
            mi.Remove(shop);
        }
    }

    internal MemoryItem GetInfo(ItemRef itemRef)
    {
        return MemoryItems.Find(x => x.ItemRef == itemRef);
    }

    public void RemoveShop(Shop shop)
    {
        var lmi = MemoryItems.Where(x => x.ShopExist(shop));
        foreach(var mi in lmi)
        {
            mi.Remove(shop);
        }
    }
}

[Serializable]
public class MemoryItem
{
    public ItemRef ItemRef;
    public List<ShopInfo> List_ShopInfo = new List<ShopInfo>();
    public MemoryItem()
    {
        ItemRef = null;
    }
    public MemoryItem(ItemRef _ItemRef)
    {
        ItemRef = _ItemRef;
    }
    public void AddInfo(Shop _shop, int? _price, float? _distance)
    {
        var si = List_ShopInfo.Find(x => x.Shop == _shop);
        if(si != null)
        {

            si.Price = _price; 
            si.Distance= _distance;
        }
        else
        {
            List_ShopInfo.Add(new ShopInfo()
            {
                Shop = _shop,
                Distance = _distance,
                Price = _price
            });
        }
    }

    internal void Remove(Shop _shop)
    {
        var si = List_ShopInfo.Find(x => x.Shop == _shop);
        if (si != null)
            List_ShopInfo.Remove(si);
    }

    public bool ShopExist(Shop _shop)
    {
        return List_ShopInfo.Exists(x => x.Shop == _shop);
    }
}

[Serializable]
public class ShopInfo
{
    public Shop Shop;
    public int? Price;
    public float? Distance;
}