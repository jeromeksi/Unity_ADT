
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Stock
{
    public List<StockItem> stockItems = new List<StockItem>();
    
    public void Add(ItemRef itr, int Amount)
    {
        var stc = stockItems.First(x => x.ItemRef == itr);
        if(stc != null)
            stc.Amount += Amount;
        else
            stockItems.Add(new StockItem( itr, Amount));
    }
    public void Add(StockItem StockItem) 
    {
        stockItems.Add(StockItem);
    }
    public void Remove(ItemRef itr, int Amount)
    {
        var stc = stockItems.First(x => x.ItemRef == itr);
        if (stc != null)
            stc.Amount -= Amount;

        //if (stc.Amount <= 0)
        //    stockItems.Remove(stc);
    }
    public int GetNumber(ItemRef itr)
    {
        var stc = stockItems.First(x => x.ItemRef == itr);
        if (stc != null)
            return stc.Amount;
        else
            return 0;
    }

    public StockItem GetStockItems(ItemRef it)
    {
        return stockItems.FirstOrDefault(x => x.ItemRef == it);
    }


    internal void Add(ItemAmount ita)
    {
        Add(ita.ItemRef, ita.Amount);
    }
}
