using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    [Header("PostProcessing")]
    public bool thetaStarMode = false;

    List<Node> openNodes = new List<Node>();
    List<Node> closeNodes = new List<Node>();

    Node finishNode = null;

    public List<Node> GetPath(Node start, Node finish, EPathfinderType pfT)
    {
        if (start == finish) return null;

        List<Node> path = new List<Node>();

        finishNode = finish;

        Vector3 diff = finish.transform.position - start.transform.position;
        Vector3 dir = diff.normalized;
        RaycastHit hit;

        if (Physics.Raycast(start.transform.position + dir * 0.5f, diff.normalized, out hit, diff.magnitude - 1.0f))
        {
            OpenNode(start, null);

            bool pathFound = false;

            while(openNodes.Count > 0)
            {
                Node actualNode = GetOpenNode(pfT);

                if (actualNode == finish)
                {
                    MakePath(ref path, actualNode);
                    pathFound = true;
                    break;
                }

                CloseNode(actualNode);
                OpenAdyNodes(actualNode);
            }

            CleanNodes();

            if (!pathFound)
            {
                return null;
            }
            else
            {
                if (thetaStarMode) PostProcessThetaStar(ref path);
            }
        }
        else
            MakePath(ref path, start, finish);

        return path;
    }

    void CloseNode(Node node)
    {
        node.nodeState = ENodeState.Close;
        openNodes.Remove(node);
        closeNodes.Add(node);
    }

    void OpenNode(Node node, Node opener)
    {
        node.nodeState = ENodeState.Open;
        node.Predecesor = opener;
        node.nodeValue.pathValue += opener.nodeValue.pathValue;
        openNodes.Add(node);
    }

    void OpenAdyNodes(Node node)
    {
        NodeAdy[] adyNodes = node.GetNodeAdyacents();

        for (int i = 0; i < (int)EAdyDirection.Count; i++)
            if (adyNodes[i].node && !adyNodes[i].node.IsObstacle && adyNodes[i].node.nodeState == ENodeState.Ok)
                OpenNode(adyNodes[i].node, node);
    }

    Node GetOpenNode(EPathfinderType pfT)
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

            case EPathfinderType.Dijkstra:
                int index = 0;
                int lowestValue = 9999999;

                for (int i = 0; i < openNodes.Count; i++)
                {
                    if(openNodes[i].nodeValue.pathValue < lowestValue)
                    {
                        index = i;
                        lowestValue = openNodes[i].nodeValue.pathValue;
                    }
                }

                node = openNodes[index];
            break;

            case EPathfinderType.Star:
                int starIndex = 0;
                int starLowestValue = 9999999;

                for (int i = 0; i < openNodes.Count; i++)
                {
                    if (openNodes[i].nodeValue.pathValue < starLowestValue)
                    {
                        starIndex = i;
                        lowestValue = openNodes[i].nodeValue.pathValue + Heuristic(openNodes[i]);
                    }
                }

                node = openNodes[starIndex];
                break;
        }

        return node;
    }

    void MakePath(ref List<Node> path, Node start, Node finish)
    {
        path.Add(start);
        path.Add(finish);
    }

    List<Node> MakePath(ref List<Node> path, Node finish)
    {
        path.Add(finish);

        Node actualNode = finish;

        while(actualNode.Predecesor)
        {
            actualNode = actualNode.Predecesor;
            path.Add(actualNode);
        }

        path.Reverse();

        return path;
    }

    void CleanNodes()
    {
        while(openNodes.Count > 0)
        {
            openNodes[0].nodeState = ENodeState.Ok;
            openNodes[0].Predecesor = null;
            openNodes[0].nodeValue.ResetPathValue();
            openNodes.RemoveAt(0);
        }

        while(closeNodes.Count > 0)
        {
            closeNodes[0].nodeState = ENodeState.Ok;
            closeNodes[0].Predecesor = null;
            closeNodes[0].nodeValue.ResetPathValue();
            closeNodes.RemoveAt(0);
        }
    }

    int Heuristic(Node actualNode)
    {
        return (int)Mathf.Abs(Mathf.Round((actualNode.transform.position - finishNode.transform.position).magnitude));
    }

    void PostProcessThetaStar(ref List<Node> path)
    {
        int actualIndex = path.Count - 1;

        while (actualIndex > 1)
        {
            RaycastHit hit;

            Vector3 diff = path[actualIndex - 2].transform.position - path[actualIndex].transform.position;

            if (!Physics.Raycast(path[actualIndex].transform.position, diff.normalized, out hit, diff.magnitude))
                path.Remove(path[actualIndex - 1]);

            actualIndex--;
        }
    }
}
