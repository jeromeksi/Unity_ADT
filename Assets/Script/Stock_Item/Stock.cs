
using System;
using System.Collections.Generic;

[Serializable]
public class Stock
{
    public int stockMax;
    private List<StockItem> List_StockItem = new List<StockItem>();

    public bool Add(ItemRef itemRef, int Amoount, bool forceAdd = true)
    {
        var its = List_StockItem.Find(x => x.ItemRef == itemRef);
        if (its != null)
        {
            var SpaceFree = GetSpaceFree();

            if (SpaceFree > Amoount)
            {
                its.Amount += Amoount;
            }
            else
            {
                if(forceAdd)
                    its.Amount += Amoount;
            }
        }
        else
        {
            InitAdd(itemRef, Amoount);
        }
        return true;

    }
    public bool Remove(ItemRef itemRef, int Amoount, bool exact = false)
    {

        var its = List_StockItem.Find(x => x.ItemRef == itemRef);
        if(its != null)
        {
            if(exact)
            {
                if (its.Amount >= Amoount)
                {
                    its.Amount -= Amoount;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (its.Amount >= Amoount)
                {
                    its.Amount -= Amoount;
                }
                else
                {
                    its.Amount = 0;
                }
                return true;
            }
        }
        return false;
    }
    public int GetAmount(ItemRef itemRef)
    {
        var it = List_StockItem.Find(x => x.ItemRef == itemRef);
        if (it != null)
            return it.Amount;
        return 0;
    }
    private void InitAdd(ItemRef itemRef, int Amoount)
    {
        List_StockItem.Add(new StockItem(itemRef, Amoount));
    }
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

    internal bool Add(ItemAmount ita, bool forceAdd = true)
    {
         return Add(ita.ItemRef, ita.Amount, forceAdd);
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
//public class OutOfSpaceException : Exception
//{
//    public StockV2 stockV2;    
//    public OutOfSpaceException(StockV2 v)
//    {
//        stockV2 = v;
//    }
//}