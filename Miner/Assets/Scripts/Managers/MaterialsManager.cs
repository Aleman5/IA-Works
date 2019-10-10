using UnityEngine;

public class MaterialsManager : MBSingleton<MaterialsManager>
{
    public int maxMinerals = 0;
    public int actualMinerals = 0;

    public void IncreaseMineralsCapacity(int amount)
    {
        maxMinerals += amount;
    }

    public void ReduceMineralsCapacity(int amount)
    {
        maxMinerals -= amount;
    }
}
