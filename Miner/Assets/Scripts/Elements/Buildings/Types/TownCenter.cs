using System.Collections.Generic;
using UnityEngine;

public class TownCenter : Building
{
    [Header("Base Variables")]
    public int mineralsLimit = 0;

    Villager[] villagers;
    Node node = null;
    int index = 0;

    Dictionary<Villager, int> villagerDic = new Dictionary<Villager, int>();

    override protected void Awake()
    {
        base.Awake();

        villagers = new Villager[(int)EAdyDirection.Count];
        for (int i = 0; i < villagers.Length; i++)
            villagers[i] = null;
    }

    override protected void Start()
    {
        base.Start();

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

    public Node GetAvailableNode()
    {
        if (!node)
            node = GameManager.Instance.nodeGenerator.GetClosestNode(transform.position);

        for (int i = 0; i < (int)EAdyDirection.Count; i++)
            if (!villagers[i])
            {
                index = i;
                return node.GetNodeAdyacents()[i].node;
            }

        UIManager.Instance.OnExcessedWorkersCapacity(elementType);

        return null;
    }
}
