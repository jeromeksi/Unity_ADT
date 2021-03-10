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
    public int ID { get; internal set; }
    public TypeAssignement TypeAssignement;
    public Moulin Batiment { get; internal set; }

}
[CreateAssetMenu(fileName = "Assignement", menuName = "Assignement/Buy")]
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
    public ItemRef ItemSell;
    public int AmountItem;
    public Sell(ItemRef itemSell)
    {
        TypeAssignement = TypeAssignement.Sell;
        ItemSell = itemSell;
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