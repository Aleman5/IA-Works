using System.Collections.Generic;
using UnityEngine;

public class Mine : Natural
{
    List<Miner> miners;

    override protected void Awake()
    {
        base.Awake();

        materialsLeft = 100;

        miners = new List<Miner>();
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
        materialsLeft--;

        if (materialsLeft >= 0)
        {
            foreach (Miner miner in miners)
                miner.MineDestroyed();
            GameManager.Instance.RemoveMine(this);
            Destroy(gameObject);
        }
    }
}
