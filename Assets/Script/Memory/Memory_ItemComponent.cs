using Batiment.BatimentVente;
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
    public void Add(ItemRef itemRef, BatimentVente_Controller shop, int? priceS, int? priceB, float? distance)
    {
        var mi = MemoryItems.Find(x => x.ItemRef == itemRef);
        if (mi != null)
        {
            mi.AddInfo(shop, priceS, priceB, distance);
        }
        else
        {
            var miT = new MemoryItem();
            miT.ItemRef = itemRef;
            miT.AddInfo(shop, priceS, priceB, distance);
            MemoryItems.Add(miT);
        }
    }
    //public void Add(UpdateShopInfo updateShopInfo)
    //{
    //    Add(updateShopInfo.ItemRef, updateShopInfo.ShopInfo.Shop, updateShopInfo.ShopInfo.Price, updateShopInfo.ShopInfo.Distance);
    //}

    internal void Add(ItemRef farine_ref, BatimentVente_Controller shop_1, InfoItemRef infoItemRef)
    {
        Add(farine_ref, shop_1, infoItemRef.PriceSell,infoItemRef.PriceBuy,0);
    }


    public void Remove(ItemRef itemRef, BatimentVente_Controller shop)
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

    public void RemoveShop(BatimentVente_Controller shop)
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
    public void AddInfo(BatimentVente_Controller _shop, int? _priceS, int? _priceB, float? _distance)
    {
        var si = List_ShopInfo.Find(x => x.Shop == _shop);
        if(si != null)
        {

            si.PriceBuy = _priceB;
            si.PriceSell = _priceS;
            si.Distance= _distance;
        }
        else
        {
            List_ShopInfo.Add(new ShopInfo()
            {
                Shop = _shop,
                Distance = _distance,
                PriceBuy = _priceB,
                PriceSell = _priceS,
            });
        }
    }

    internal void Remove(BatimentVente_Controller _shop)
    {
        var si = List_ShopInfo.Find(x => x.Shop == _shop);
        if (si != null)
            List_ShopInfo.Remove(si);
    }

    public bool ShopExist(BatimentVente_Controller _shop)
    {
        return List_ShopInfo.Exists(x => x.Shop == _shop);
    }
}

[Serializable]
public class ShopInfo
{
    public BatimentVente_Controller Shop;
    public int? PriceSell;
    public int? PriceBuy;
    public float? Distance;
}