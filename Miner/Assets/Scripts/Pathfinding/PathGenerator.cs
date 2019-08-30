using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    static List<Vector3> path = new List<Vector3>();
    static List<Node> openNodes = new List<Node>();
    static List<Node> closeNodes = new List<Node>();

    public static List<Vector3> GetPath(Node start, Node finish, EPathfinderType pfT)
    {
        Vector3 diff = finish.transform.position - start.transform.position;
        Vector3 dir = diff.normalized;
        RaycastHit hit;

        if (Physics.Raycast(start.transform.position + dir * 0.5f, diff.normalized, out hit, diff.magnitude - 1.0f))
        {
            OpenNode(start, null);

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

        return path;
    }

    static void CloseNode(Node node)
    {
        node.nodeState = ENodeState.Close;
        openNodes.Remove(node);
        closeNodes.Add(node);
    }

    static void OpenNode(Node node, Node opener)
    {
        node.nodeState = ENodeState.Open;
        node.Predecesor = opener;
        openNodes.Add(node);
    }

    static void OpenAdyNodes(Node node)
    {
        NodeAdy[] adyNodes = node.GetNodeAdyacents();

        for (int i = 0; i < (int)EAdyDirection.Count; i++)
            if (adyNodes[i].node && !adyNodes[i].node.IsObstacle && adyNodes[i].node.nodeState == ENodeState.Ok)
                OpenNode(adyNodes[i].node, node);
    }

    static Node GetOpenNode(EPathfinderType pfT)
    {
        Node node = null;

        switch(pfT)
        {
            case EPathfinderType.BreadthFirst:
                node = openNodes[0];
            break;

            case EPathfinderType.DepthFirst:
                node = openNodes[openNodes.Count - 1];
            break;

            case EPathfinderType.Star:
                
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
            openNodes[0].nodeState = ENodeState.Ok;
            openNodes[0].Predecesor = null;
            openNodes.RemoveAt(0);
        }

        while(closeNodes.Count > 0)
        {
            closeNodes[0].nodeState = ENodeState.Ok;
            closeNodes[0].Predecesor = null;
            closeNodes.RemoveAt(0);
        }
    }
}
