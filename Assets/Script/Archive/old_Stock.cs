
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class old_Stock
{
    public List<ItemAmount> stockItems = new List<ItemAmount>();
    
    public void Add(ItemRef itr, int Amount)
    {
        if(!stockItems.Any(x => x.ItemRef == itr))
        {
            stockItems.Add(new ItemAmount(itr, Amount));
        }
        else
        {
            stockItems.First(x => x.ItemRef == itr).Amount += Amount;
        }
    }
    public void Add(ItemAmount ItemAmount) 
    {
        Add(ItemAmount.ItemRef, ItemAmount.Amount);
    }
    public void Remove(ItemRef itr, int Amount)
    {
        var stc = stockItems.First(x => x.ItemRef == itr);
        if (stc != null)
            stc.Amount -= Amount;

    }
    public int GetNumber(ItemRef itr)
    {
        var stc = stockItems.First(x => x.ItemRef == itr);
        if (stc != null)
            return stc.Amount;
        else
            return 0;
    }

    public ItemAmount GetStockItems(ItemRef it)
    {
        return stockItems.FirstOrDefault(x => x.ItemRef == it);
    }


    public List<ItemAmount> Get_ListIteamAmount(List<ItemRef> list_itr)
    {
        return stockItems.Where(x => list_itr.Exists(v => v == x.ItemRef)).ToList();
    }
}
