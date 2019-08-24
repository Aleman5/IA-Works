using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    List<Node> ady;
    NodeState nodeState;
    bool isObstacle = false;

    public void AddAdyNode(Node node)
    {
        ady.Add(node);
    }

    public bool IsObstacle
    {
        get { return isObstacle; }
        set
        {
            isObstacle = value;
        }
    }

}