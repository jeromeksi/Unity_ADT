using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Batiment.BatimentVente
{
    public class BatimentVente_MemoryComponent : MonoBehaviour
    {
        //Sotckage des prix des objets en stocks
        //Prix Achat =/= Prix vente

        // Start is called before the first frame update
        List<InfoItemRef> list_InfoItemRef;

    void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
    public class InfoItemRef
    {
        public ItemRef ItemRef;
        public int PriceBuy;
        public int PriceSell;

        public float PercentMarge;

    }

}
