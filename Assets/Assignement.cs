using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Assignement
{
    public bool IsMainAssignement;
    public BatimentProductionV2 BatimentProduction;
    public TypeAssignement TypeAssignement;
    public Shop Shop;
    public Vector3 Destination;


    public float Money;
    public List<ItemAmount> List_Item = new List<ItemAmount>();

    public Assignement(BatimentProductionV2 batimentProductionV2)
    {
        this.BatimentProduction = batimentProductionV2;
    }
    public void EndAssign(bool Succed)
    {
        BatimentProduction.AssignEnd(this, Succed);
    }
}