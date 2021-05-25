using Batiment.BatimentVente;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Batiment.BatimentProduction
{
    public class BatimentProduction_InitComponent : MonoBehaviour
    {
        //Un script Mono qui permet d'initier en dur les variables du batiment

        public int MoneyStart;
        public int StockMaxStart;

        public int NumberPosteMax;
        public List<NPCController> List_Employe = new List<NPCController>();

        //public List<ItemRef> List_ItemRef = new List<ItemRef>();
        public List<BatimentVente_Controller> List_Shop = new List<BatimentVente_Controller>();


        public List<ItemRef> List_ItemCreate;
    }
}
