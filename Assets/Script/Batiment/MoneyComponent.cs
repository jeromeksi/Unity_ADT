using System;

[Serializable]
public class MoneyComponent
{
    private float Money;

    public void AddMoney(float mon)
    {
        Money += mon;
    }
    public bool RemoveMoney(float mon)
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
    public float GetMoney()
    {
        return Money;
    }
}
