using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public int mineralsLeft = 1;

    List<Miner> miners;
    Element element;

    void Awake()
    {
        element = Element.Mine;
    }

    public void AddMiner(Miner thisM)
    {
        miners.Add(thisM);
    }

    public void RemoveMiner(Miner thisM)
    {
        miners.Remove(thisM);
    }

    public void RemoveMaterial()
    {
        mineralsLeft--;

        if (mineralsLeft == 0)
        {
            foreach (Miner miner in miners)
                miner.MineDestroyed();
            GameManager.Instance.RemoveMine(this);
            Destroy(gameObject);
        }
    }

    public Element GetElementType()
    {
        return element;
    }
}
