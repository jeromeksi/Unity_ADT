using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Moulin : MonoBehaviour
{
    //public NPControler Emp;
    public List<NPControler> Emps = new List<NPControler>();

    public List<ItemRefAssociate> List_ItemRefIn = new List<ItemRefAssociate>();
    public List<ItemRefAssociate> List_ItemRefOut = new List<ItemRefAssociate>();


    public List<AssignementPrio_Emp> assignementPrio_Emps = new List<AssignementPrio_Emp>();


    public int NumberMaxWork_ItemOut; //a terme ce sera defini par le lvl du batiment
    public float Money;

    public int NombreRecetteMinInStock;

    public List<StockItem> StockItem;
    public int maxStockItem;
    public List<Assignement> List_Task;
    //public GameObject Magasin;
    //public GameObject Ferme;

    private int lastIDAsssignement;

    public bool IsLogg = false;
    public void Start()
    {
        StockItem = new List<StockItem>();
        lastIDAsssignement = 1;
        InitStock();
        foreach(var it in List_ItemRefIn)
        {
            CreateAssignement(TypeAssignement.Buy, it);
        }
        foreach (var it in List_ItemRefOut)
        {
            CreateAssignement(TypeAssignement.Sell, it);
            CreateAssignement(TypeAssignement.Work, it);
        }
        StartCoroutine(RoutineV2());
    }
    private void CreateAssignement(TypeAssignement typeAssignement, ItemRefAssociate ItemRefAssociate)
    {
        Assignement ass = null; ;
        switch(typeAssignement)
        {
            case TypeAssignement.Buy:
                ass = new Buy(ItemRefAssociate.ItemRef)
                {
                    //ID = lastIDAsssignement,
                    NomAssignement = "Achète",
                    AmountMoney = 100,
                    //Batiment = this,
                    IsAssign = false,
                    PosAssignement = ItemRefAssociate.Pos.transform.position
                };               
                break;
            case TypeAssignement.Sell:
                ass = new Sell(ItemRefAssociate.ItemRef)
                {
                    //ID = lastIDAsssignement,
                    NomAssignement = "Vend",
                    //Batiment = this,
                    AmountItem = 0,
                    IsAssign = false,
                    PosAssignement = ItemRefAssociate.Pos.transform.position,
                };               
                break;
            case TypeAssignement.Work:
                ass = new Work(ItemRefAssociate.ItemRef)
                {
                    //ID = lastIDAsssignement,
                    NomAssignement = "Create object",
                    IsAssign = false,
                    AmountItem = 1,
                    PosAssignement = this.transform.position,
                    MaxNumberAssignement = 6,
                    NumberAssignation = 0,
                    //Batiment = this,
                    //TypeAssignement = TypeAssignement.Work
                };

                break;
        }
        List_Task.Add(ass);


        lastIDAsssignement++;
    }
    private List<Work> CheckRecipeToCreateWork()
    {
        List<Work> listWork = new List<Work>();

        foreach(Work w in List_Task.Where(x => x.TypeAssignement == TypeAssignement.Work))
        {
            if (CanCreateItem(w.ItemCreate))
                listWork.Add(w);
        }

        return listWork;
    }
    public bool CanCreateItem(ItemRef itemRef)
    {
        foreach(var it in itemRef.Recipe)
        {
            if (StockItem.Exists(x => x.ItemRef == it.ItemRef && x.Amount < it.Amount))
            {
                return false;
            }
        }
        return true;
    }
    public List<ItemRef> CanCreateItem_Missing(ItemRef itemRef)
    {
        var listItemMissing = new List<ItemRef>();
        foreach (var it in itemRef.Recipe)
        {
            if (StockItem.Any(x => x.ItemRef == it.ItemRef && x.Amount < it.Amount))
            {
                listItemMissing.Add(it.ItemRef);
            }
        }
        return listItemMissing;
    }
    private void InitStock()
    {
        foreach(var it in List_ItemRefIn)
        {
            StockItem.Add(new StockItem(it.ItemRef, 0)
            {
                //StockItemMin = StockItemMin.Medium
            });
        }
        foreach (var it in List_ItemRefOut)
        {
            StockItem.Add(new StockItem(it.ItemRef, 0));
        }
    }

    private IEnumerator Routine()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {

            // SI on peux taff on taff !
            var listWork = CheckRecipeToCreateWork();
            foreach (Work tw in listWork)
            {
                if(tw.NumberAssignation<tw.MaxNumberAssignement)
                {
                    tw.NumberAssignation += 1;
                    //NPControlerFree().AddAssignement(tw);
                }
            }
            //Check le stock ! Go acheté ou vendre
            foreach (Buy tb in List_Task.Where(x => x.TypeAssignement == TypeAssignement.Buy))
            {
                if (Money >= 100 && !tb.IsAssign)
                {
                    tb.IsAssign = true;
                    //NPControlerFree().AddAssignement(tb);
                }
            }
            //Check le stock ! Go acheté ou vendre
            foreach (Sell ts in List_Task.Where(x => x.TypeAssignement == TypeAssignement.Sell))
            {
                //if (Money <= 100 && StockItem[ts.ItemSell] >= 100 && !ts.IsAssign)
                {
                    ts.IsAssign = true;
                    //NPControlerFree().AddAssignement(ts);
                }
            }
            if (IsLogg)
                SomeLog();
            yield return new WaitForSeconds(1);

        }
    }


    private IEnumerator RoutineV2()
    {
        yield return new WaitForSeconds(1);
        var listAssignementTemp = new List<AssignementPrio>();

        while (true)
        {
            listAssignementTemp.Clear();
            //Work
            foreach (Work work in List_Task.Where(x => x.TypeAssignement == TypeAssignement.Work))
            {
                var numberRecipe = NumberPossibleRecipeItem(work.ItemCreate);

                if (numberRecipe > 0 && work.NumberAssignation < work.MaxNumberAssignement)
                {
                    listAssignementTemp.Add(new AssignementPrio(work, PrioriteTemp.Moyenne));
                }                                              
            }

            //BUY
            List<Assignement> listBuy = List_Task.Where(x => x.TypeAssignement == TypeAssignement.Buy && x.IsAssign ==false).ToList();
            foreach (Buy itBuy in listBuy)
            {
                var stockItem = StockItem.First(x => x.ItemRef == itBuy.ItemBuy);
                //int amountMin = (int)stockItem.StockItemMin * maxStockItem / 100;
                //var diff = stockItem.Amount - amountMin;

                //if (stockItem.Amount < amountMin && Money > 0)
                //{
                //    if(Money> amountMin + 100)
                //    {
                //        itBuy.AmountItem = amountMin + 100;
                //        itBuy.AmountMoney = amountMin + 100;
                //    }
                //    else
                //    {
                //        itBuy.AmountItem = (int)Money;
                //        itBuy.AmountMoney = (int)Money;
                //    }

                //    if(itBuy.AmountMoney > 0)

                //        listAssignementTemp.Add(new AssignementPrio(itBuy, PrioriteTemp.Forte));
                //}
                //Debug.Log($"Ici");                
            }

            //Sell
            //List<Assignement> listSell = List_Task.Where(x => x.TypeAssignement == TypeAssignement.Sell).ToList();



            //foreach (Sell sel in listSell)
            //{
            //    var stockItem = StockItem.First(x => x.ItemRef == sel.ItemSell).Amount;
            //    sel.AmountItem = stockItem;
            //    var prio = GetPrioMoney();


            //    if (!sel.IsAssign  && stockItem > 0 && prio != PrioriteTemp.Null)
            //    {


            //        listAssignementTemp.Add(new AssignementPrio(sel, prio));
            //    }
            //    if(sel.IsAssign)
            //        sel = 
            //}
            
            
            


            
            //Distribute assignement
            foreach (var ass in listAssignementTemp.OrderBy(x => x.LevelPrio))
            {
                ass.Assignement.IsAssign = true;
                Type v = ass.Assignement.GetType();
                if (v == typeof(Work))
                    ((Work)ass.Assignement).NumberAssignation++;

                //NPControlerFree().AddAssignement(ass);
            }

            if (IsLogg)
                SomeLog();
            yield return new WaitForSeconds(0.1f);

        }
    }
    // Refaire lea routine Assignemnt; en partant du stock, des besoins de matieère ou d'argent)
    // Le but c'est que le batiment sache ou il en est en nombre Assignement lancé... 
    // Et qui fait quoi ... 
    private IEnumerator RoutineV3()
    {
        yield return new WaitForSeconds(1);
        var listAssignementTemp = new List<AssignementPrio>();

        while (true)
        {
            foreach(var it in List_ItemRefOut)
            {
                //assignementPrio_Emps
            }




            yield return new WaitForSeconds(0.1f);
        }
    }
    public PrioriteTemp GetPrioMoney()
    {
        PrioriteTemp prio = PrioriteTemp.Null;
        switch (Money)
        {
            case var exp when Money <= 10:
                prio = PrioriteTemp.Urgente;
                break;
            case var exp when Money <= 125:
                prio = PrioriteTemp.Forte;
                break;
            case var exp when Money <= 200:
                prio = PrioriteTemp.Moyenne;
                break;
            case var exp when Money <= 300:
                prio = PrioriteTemp.Faible;
                break;
            case var exp when Money > 300:
                prio = PrioriteTemp.Null;
                break;
        }
        return prio;
    }
    public int NumberPossibleRecipeItem(ItemRef itemRef)
    {
        float numberRecipe = Mathf.Infinity;
        foreach(var it in itemRef.Recipe)
        {
            var n = StockItem.FirstOrDefault(x => x.ItemRef == it.ItemRef).Amount/ it.Amount;
            if(numberRecipe>n)
            {
                numberRecipe = n;
            }
        }
        return Convert.ToInt32(numberRecipe);
    }
    public void SomeLog()
    {
        foreach(var it in StockItem)
        {
            Debug.Log(it.ItemRef.Name + " : " + it.Amount);
        }
        Debug.Log("Money : " + Money );

    }
    public NPControler NPControlerFree()
    {
        NPControler npc = Emps[0];

        for (int i = 1; i < Emps.Count; i++)
        {
            if (npc.List_Assignement.Count > Emps[i].List_Assignement.Count)
            {
                npc = Emps[i];
            }
        }
        return npc;
    }
    public bool SuppMoney(int amount)
    {
        bool isPossible = false;
        if (Money >= amount)
        {
            isPossible = true;
            Money -= amount;
        }
        return isPossible;

    }
    public void AddMoney(int amount)
    {
        Money += amount;
    }
    public void AddItemStock(ItemRef item, int amount)
    {
        StockItem.First(x=> x.ItemRef == item).Amount += amount;
    }
    public void SuppItemStock(ItemRef item, int amount)
    {
        StockItem.First(x => x.ItemRef == item).Amount -= amount;
    }
    public bool CheckItemAmount(ItemRef item, int amount)
    {
        return StockItem.First(x => x.ItemRef == item).Amount >= amount;
    }


}

[Serializable]
public class AssignementPrio_Emp
{
    public AssignementPrio AssignementPrio { get; set; }
    public NPControler emp { get; set; }
}

[Serializable]
public class AssignementPrio
{
    public Assignement Assignement;
    public PrioriteTemp LevelPrio;
    public AssignementPrio(Assignement work, PrioriteTemp v)
    {
        this.Assignement = work;
        this.LevelPrio = v;
    }
}
public enum PrioriteTemp
{
    Faible=1,
    Moyenne=2,
    Forte=3,
    Urgente=4,
    Null = 0
}
[Serializable]
public class StockItem
{
    public ItemRef ItemRef;
    public int Amount;
    public float AmountScaleNeedMin;
    public StockItem(ItemRef itemRef, int v)
    {
        ItemRef = itemRef;
        Amount = v;
    }
}
public enum StockItemMin
{
    Null, //pas de besoi
    Low = 15, // 15 %
    Medium = 30, // 40 %
    High =65// 65 %
}