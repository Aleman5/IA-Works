using UnityEngine;

public class Base : Building
{
    [Header("Base Variables")]
    public int mineralsLimit = 0;

    void Awake()
    {
        
    }

    void Start()
    {
        MaterialsManager.Instance.IncreaseMineralsCapacity(mineralsLimit);
    }

    public void DeliverMinerals(ref int amount)
    {
        MaterialsManager mM = MaterialsManager.Instance;

        int deliver = amount;

        if (mM.actualMinerals < mM.maxMinerals)
        {
            if (mM.actualMinerals + amount > mM.maxMinerals)
                deliver = mM.maxMinerals - mM.actualMinerals;

            amount -= deliver;
            mM.actualMinerals += deliver;
        }
    }
}
