using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Shop))]
[RequireComponent(typeof(BatimentProductionV2))]
public class BatimentProduction_Controller : MonoBehaviour
{
    private Shop Shop;
    private BatimentProductionV2 BatimentProductionV2;
    private StockBatiment StockBatiment;

    void Start()
    {
        TryGetComponent<Shop>(out Shop);
        TryGetComponent<BatimentProductionV2>(out BatimentProductionV2);
    }
    public Shop GetShopComptonent()
    {
        if (Shop != null)
            return Shop;
        throw new System.Exception("Pas de shop dans ce magasin");
    }
    public BatimentProductionV2 GetBatimentProductionV2Comptonent()
    {
        if (BatimentProductionV2 != null)
            return BatimentProductionV2;
        throw new System.Exception("Pas de shop dans ce BatimentProductionV2");
    }
    
}

