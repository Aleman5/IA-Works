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

    [Header("Generation")]
    public NodeGenerator nodeGenerator;
    public PathGenerator pathGenerator;

    [Header("Elements")]
    public TownCenter myTownCenter;
    public List<Mine> mines = new List<Mine>();

    [Header("Pathfinding")]
    public EPathfinderType pathfinderType;

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

    public void RemoveMine(Mine thisMine)
    {
        for (int i = 0; i < mines.Count; i++)
            if (mines[i] == thisMine)
            {
                mines.Remove(mines[i]);
                break;
            }
    }
}
