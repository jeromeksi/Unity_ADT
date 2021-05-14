using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatimentProduction_InitComponent : MonoBehaviour
{
    //Un script Mono qui permet d'initier en dur les variables du batiment

    public int MoneyStart;
    public int StockMaxStart;

    public int NumberPosteMax;
    public List<NPCController> List_Employe = new List<NPCController>();
    public ItemRef Farine_ref;
    public ItemRef Ble_ref;
    public ItemRef Orge_ref;

    public Shop Shop_1;
    public float Distance_1;

    public Shop Shop_2;
    public float Distance_2;

}
