using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance {
        get {
            instance = FindObjectOfType<GameManager>();
            if(instance == null) {
                GameObject go = new GameObject("Managers");
                instance = go.AddComponent<GameManager>();
            }
            return instance;
        }
    }

    public NodeGenerator nodeGenerator;
    public Base theBase;
    public List<Mine> mines;

    void Start()
    {
        Mine[] aMines = FindObjectsOfType<Mine>();

        foreach (Mine mine in aMines)
            mines.Add(mine);
    }

    public Mine FindClosestMine(Vector3 pos)
    {
        if (mines.Count == 0) return null;

        int index = 0;
        float minDist = 9999999;

        for (int i = 0; i < mines.Count; i++)
        {
            float dist = (mines[i].transform.position - pos).magnitude;

            if (dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }

        return mines[index];
    }

    public Node FindClosestNode(Vector3 pos)
    {
        return nodeGenerator.GetClosestNode(pos);
    }

    public void RemoveMine(Mine thisMine)
    {
        foreach (Mine mine in mines)
            if (mine == thisMine)
                mines.Remove(mine);
    }
}
