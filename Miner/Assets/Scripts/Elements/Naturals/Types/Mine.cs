using System.Collections.Generic;
using UnityEngine;

public class Mine : Natural
{
    Miner[] miners;
    Node node = null;
    int index = 0;

    Dictionary<Miner, int> minersDic = new Dictionary<Miner, int>();

    override protected void Awake()
    {
        base.Awake();

        miners = new Miner[maxWorkers];
        for (int i = 0; i < miners.Length; i++)
            miners[i] = null;
    }

    public void AddMiner(Miner thisM)
    {
        miners[index] = thisM;

        if (minersDic.ContainsKey(thisM))
            minersDic[thisM] = index;
        else
            minersDic.Add(thisM, index);
    }

    public void RemoveMiner(Miner thisM)
    {
        miners[minersDic[thisM]] = null;
        minersDic[thisM] = -1;
    }

    public void RemoveMaterial()
    {
        materialsLeft--;

        if (materialsLeft <= 0)
        {
            foreach (Miner miner in miners)
                if (miner)
                    miner.MineDestroyed();
            
            GameManager.Instance.RemoveMine(this);
            Destroy(gameObject);
        }
    }

    public Node GetAvailableNode()
    {
        if (!node)
            node = GameManager.Instance.nodeGenerator.GetClosestNode(transform.position);

        for (int i = 0; i < maxWorkers; i++)
            if (!miners[i]){
                index = i;
                return node.GetNodeAdyacents()[i].node;
            }

        UIManager.Instance.OnExcessedWorkersCapacity(elementType);

        return null;
    }
}
