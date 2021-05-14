using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Assignement
{
    public bool IsMainAssignement;
    public BatimentProduction_WorkComponent BatimentProduction;
    public TypeAssignement TypeAssignement;
    public Shop Shop;
    public Vector3 Destination;
    public List<UpdateShopInfo> List_UpdateShopInfo = new List<UpdateShopInfo>();

    public float Money;
    public List<ItemAmount> List_Item = new List<ItemAmount>();

    public Assignement(BatimentProduction_WorkComponent batimentProductionV2)
    {
        this.BatimentProduction = batimentProductionV2;
    }


    public void EndAssign(bool Succed)
    {
        BatimentProduction.AssignEnd(this, Succed);
    }
}

public class UpdateShopInfo
{
    public ShopInfo ShopInfo;
    public ItemRef ItemRef;
}