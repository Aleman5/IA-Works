using UnityEngine;

public class Node : MonoBehaviour
{
    Node predecesor = null; 
    public NodeState nodeState;
    NodeAdy[] ady;
    bool isObstacle = false;

    void Awake()
    {
        ady = new NodeAdy[(int)AdyDirection.Count];

        for (int i = 0; i < ady.Length; i++)
        {
            ady[i].node = null;
            ady[i].type = NodeAdyType.Straight;
        }
    }

    public void AddAdyNode(Node node, NodeAdyType type, AdyDirection direction)
    {
        ady[(int)direction].node = node;
        ady[(int)direction].type = type;
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