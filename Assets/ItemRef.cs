using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemRef",menuName ="ItemRef/ItemRef")]
[Serializable]
public class ItemRef : ScriptableObject
{
    public float ID;
    public string Name;
    public float baseWorkingNeed;
    public int AmountByWorking;

    public float BasePrice;
    public TypeCreate TypeCreate;
    public List<RecipeItem> Recipe = new List<RecipeItem>();

    public float GetBasePrice()
    {
        float BasePriceCalc = 0;
        if(Recipe.Count>0)
        {
            foreach (var v in Recipe)
            {
                BasePriceCalc += (v.ItemRef.GetBasePrice() * v.Amount);
            }
        }
        else
        {
            BasePriceCalc = BasePrice;
        }
        return BasePriceCalc;
    }
}
[Serializable]
public class RecipeItem
{
    public ItemRef ItemRef;
    public int Amount;
}
public enum TypeCreate
{
    Craft,
    Recolt
}
