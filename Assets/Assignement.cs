using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Assignement
{
    public string NomAssignement { get; set; }
    public Vector3 PosAssignement { get; set; }
    public string NameAnimation;
    public bool IsAssign { get; internal set; }
    public string ID { get; internal set; }
    public TypeAssignement TypeAssignement;
    public BatimentProduction Batiment { get; internal set; }
    public float LevelPrio;
    public NPControlerAssignement emp { get; set; }


}
public class Buy : Assignement
{
    public ItemRef ItemBuy;
    public Buy(ItemRef itemBuy)
    {
        TypeAssignement = TypeAssignement.Buy;
        ItemBuy = itemBuy;
    }
    public int AmountItem { get; set; }
    public int AmountMoney { get; internal set; }

}
public class Sell : Assignement
{
    public List<ItemSell> List_ItemSell = new List<ItemSell>();
    public Sell(List<ItemSell> lis)
    {
        TypeAssignement = TypeAssignement.Sell;
        List_ItemSell = lis;
    }
    public Sell()
    {
        TypeAssignement = TypeAssignement.Sell;
    }
    public void AddItemSell(ItemSell its)
    {
        List_ItemSell.Add(its);
    }
    public void AddItemSell(List<StockItem> its)
    {
        its.ForEach(x => List_ItemSell.Add(new ItemSell() {
            ItemRef = x.ItemRef,
            Amount = x.Amount
        }));
    }
    public Shop ShopRef { get; set; }

    public class ItemSell
    {
        public ItemRef ItemRef;
        public int Amount;
    }
}
public class Work : Assignement
{
    public ItemRef ItemCreate;
    public int AmountItem;
    public Work(ItemRef itemCreate)
    {
        TypeAssignement = TypeAssignement.Work;
        ItemCreate = itemCreate;
    }

    public int MaxNumberAssignement { get; internal set; }
    public int NumberAssignation { get; internal set; }
}
public enum TypeAssignement
{
    Buy,
    Sell,
    Goto,
    Work
}