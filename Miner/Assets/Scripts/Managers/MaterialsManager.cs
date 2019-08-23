using UnityEngine;

public class MaterialsManager : MonoBehaviour
{
    private static MaterialsManager instance;

    public static MaterialsManager Instance {
        get {
            instance = FindObjectOfType<MaterialsManager>();
            if(instance == null) {
                GameObject go = new GameObject("Managers");
                instance = go.AddComponent<MaterialsManager>();
            }
            return instance;
        }
    }

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
