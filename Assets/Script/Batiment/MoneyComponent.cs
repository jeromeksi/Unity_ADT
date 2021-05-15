using System;

[Serializable]
public class MoneyComponent
{
    private int Money;

    public void AddMoney(int mon)
    {
        Money += mon;
    }
    public bool RemoveMoney(int mon)
    {
        if (mon > Money)
            return false;
        else
        {
            Money -= mon;
            return true;
        }
    }
    public bool CheckMoney(float mon)
    {
        return Money > mon;
    }
    public int GetMoney()
    {
        return Money;
    }
}
