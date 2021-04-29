
using System;
using System.Collections.Generic;

public class StockV2
{
    public int stockMax;
    private List<StockItem> List_StockItem = new List<StockItem>();

    //TODO - ici
    //public bool Add(ItemRef itemRef, int Amoount)
    //{
    //    var its = List_StockItem.Find(x=> x.ItemRef == itemRef);
    //    if (its != null)
    //    {
    //        var SpaceFree = GetSpaceFree();

    //        if(SpaceFree > Amoount)
    //        {
    //            its.Amount += Amoount;
    //        }
    //        else
    //        {

    //        }
    //    }
    //    else
    //    {
    //        //init
    //    }
        
    //}

    public int GetSpaceFree()
    {
        return stockMax - GetSpaceUse();
    }

    public int GetSpaceUse()
    {
        int spaceUse = 0;
        foreach(var its in List_StockItem)
        {
            spaceUse += its.Amount;
        }
        return spaceUse;
    }
}

[Serializable]
public class StockItem
{
    public ItemRef ItemRef;
    public int Amount;
    public StockItem(ItemRef itemRef, int v)
    {
        ItemRef = itemRef;
        Amount = v;
    }
}