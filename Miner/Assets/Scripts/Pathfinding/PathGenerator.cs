using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    static List<Vector3> path = new List<Vector3>();
    static List<Node> openNodes = new List<Node>();
    static List<Node> closeNodes = new List<Node>();

    public static List<Vector3> GetPath(Node start, Node finish, PathfinderType pfT)
    {
        Vector3 diff = finish.transform.position - start.transform.position;
        RaycastHit hit;

        if (Physics.Raycast(start.transform.position, diff.normalized, out hit, diff.magnitude))
        {
            if (hit.transform.tag == "Obstacle")
            {
                OpenNode(start);

                while(openNodes.Count > 0)
                {
                    Node actualNode = GetOpenNode(pfT);

                    if (actualNode == finish)
                    {
                        MakePath(actualNode);
                        break;
                    }

                    CloseNode(actualNode);
                    OpenAdyNodes(actualNode);
                }

                CleanNodes();
            }
            else
                MakePath(start, finish);
        }
        else
            MakePath(start, finish);

        return path;
    }

    static void CloseNode(Node node)
    {
        node.nodeState = NodeState.Close;
        openNodes.Remove(node);
        closeNodes.Add(node);
    }

    static void OpenNode(Node node)
    {
        node.nodeState = NodeState.Open;
        openNodes.Add(node);
    }

    static void OpenNode(Node node, Node opener)
    {
        node.nodeState = NodeState.Open;
        node.Predecesor = opener;
        openNodes.Add(node);
    }

    static void OpenAdyNodes(Node node)
    {
        NodeAdy[] adyNodes = node.GetNodeAdyacents();

        for (int i = 0; i < (int)AdyDirection.Count; i++)
            if (adyNodes[i].node && !adyNodes[i].node.IsObstacle && adyNodes[i].node.nodeState == NodeState.Ok)
                OpenNode(adyNodes[i].node, node);
    }

    static Node GetOpenNode(PathfinderType pfT)
    {
        Node node = null;

        switch(pfT)
        {
            case PathfinderType.BreadthFirst:
                node = openNodes[0];
            break;

            case PathfinderType.DepthFirst:
                node = openNodes[openNodes.Count - 1];
            break;

            case PathfinderType.Star:
                
            break;
        }

        return node;
    }

    static void MakePath(Node start, Node finish)
    {
        path = new List<Vector3>();

        path.Add(start.transform.position);
        path.Add(finish.transform.position);
    }

    static void MakePath(Node finish)
    {
        path = new List<Vector3>();

        path.Add(finish.transform.position);

        Node actualNode = finish;

        while(actualNode.Predecesor)
        {
            actualNode = actualNode.Predecesor;
            path.Add(actualNode.transform.position);
        }

        path.Reverse();
    }

    static void CleanNodes()
    {
        while(openNodes.Count > 0)
        {
            openNodes[0].nodeState = NodeState.Ok;
            openNodes[0].Predecesor = null;
            openNodes.RemoveAt(0);
        }

        while(closeNodes.Count > 0)
        {
            closeNodes[0].nodeState = NodeState.Ok;
            closeNodes[0].Predecesor = null;
            closeNodes.RemoveAt(0);
        }
    }
}
