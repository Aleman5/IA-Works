using UnityEngine;

public class Node : MonoBehaviour
{
    public ENodeState nodeState;
    public NodeValue nodeValue;

    NodeAdy[] ady;
    Node predecesor = null; 
    bool isObstacle = false;

    void Awake()
    {
        ady = new NodeAdy[(int)EAdyDirection.Count];

        for (int i = 0; i < ady.Length; i++)
        {
            ady[i].node = null;
            if (i % 2 == 0)
                ady[i].type = ENodeAdyType.Straight;
            else
                ady[i].type = ENodeAdyType.Diagonal;

        }
    }

    public void AddAdyNode(Node node, EAdyDirection direction)
    {
        ady[(int)direction].node = node;
    }

    public NodeAdy[] GetNodeAdyacents()
    {
        return ady;
    }

    public bool IsObstacle
    {
        get { return isObstacle; }
        set
        {
            isObstacle = value;
        }
    }

    public Node Predecesor
    {
        get { return predecesor;  }
        set { predecesor = value; }
    }
}