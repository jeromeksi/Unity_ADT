using Assets.Script.Batiment.Assignement_Work;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



namespace Batiment.BatimentVente
{
    public class BatimentVente_WorkerComponent 
    {
        //gestion des employés, de l'ouverture de la vente

        public int NumberPosteMax;

        public BatimentVente_Controller Controller;

        public List<NPCController> List_Emp;
        public BatimentVente_WorkerComponent(BatimentVente_Controller cc)
        {
            Controller = cc;
            List_Emp = new List<NPCController>();
        }


        internal IEnumerator RoutineSell()
        {
            while(Controller.BatIsActive())
            {

                    foreach (var emp in List_Emp.Where(x => x.IsWorking && !x.Work_HadMainAssing()))
                    {
                        //emp
                        var work = new BatimentVente_Work();
                        work.BatimentVente_cmp = this;
                        work.BatimentVente = Controller.transform.position;



                        emp.Set_MainAssign(work);
                    }
                    if (List_Emp.Count(x => x.IsWorking) == 0)
                    {
                        Controller.SetCanShop(false);
                    }
                yield return new WaitForSeconds(1f);
            }
        }


        internal void SetNumberPosteMax(int empMax)
        {
            NumberPosteMax = empMax;
        }

        internal void AddEmploye(NPCController emp)
        {
            if(List_Emp == null)
            {
                List_Emp = new List<NPCController>();
            }
            List_Emp.Add(emp);
        }

        internal void AssignEnd(BatimentVente_Work batimentVente_Work, bool succed)
        {
            if(succed)
            {

                Controller.SetCanShop(true);                
            }
        }
    }

}
