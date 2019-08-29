using UnityEngine;

public class Base : MonoBehaviour
{
    public int mineralsLimit = 0;

    Element element;

    void Awake()
    {
        element = Element.Base;
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

    public Element GetElementType()
    {
        return element;
    }
}
