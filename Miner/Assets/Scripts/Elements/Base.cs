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

    public int DeliverMinerals(int amount)
    {
        MaterialsManager mM = MaterialsManager.Instance;

        int refund = amount;

        if (mM.actualMinerals < mM.maxMinerals)
        {
            if (mM.actualMinerals + amount > mM.maxMinerals)
                amount = mM.maxMinerals - mM.actualMinerals;

            refund -= amount;
            mM.actualMinerals += amount;
        }

        return refund;
    }

    public Element GetElementType()
    {
        return element;
    }
}
