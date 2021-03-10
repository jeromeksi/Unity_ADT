using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemRef",menuName ="ItemRef/ItemRef")]
public class ItemRef : ScriptableObject
{
    public float ID;
    public string Name;
    public float baseTimeProduct;
    public List<RecipeItem> Recipe = new List<RecipeItem>();
}
[Serializable]
public class ItemRefAssociate
{
    public ItemRef ItemRef;
    public GameObject Pos;
}
[Serializable]
public class RecipeItem
{
    public ItemRef ItemRef;
    public int Amount;
}


//public class ItemStock : ScriptableObject
//{
//    ItemRef itemRef { get; set; }
//    public float Amount {get;set;}
//    public ItemStock(ItemRef it)
//    {
//        itemRef = it;
//    }
//    public ItemStock(ItemRef it, float amount)
//    {
//        itemRef = it;
//        Amount = amount;
//    }
//}