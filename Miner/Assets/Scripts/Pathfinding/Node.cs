using UnityEngine;

public class Node : MonoBehaviour
{
    public ENodeState nodeState;
    public NodeValue nodeValue;
    public Vector3 position;
    public Node predecesor = null; 
    public bool taken = false;
    public bool isObstacle = false;
    
    NodeAdy[] ady;

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
}