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
        
        Vector3 diff = finish.position - start.position;
        Vector3 dir = diff.normalized;
        RaycastHit hit;
        
        if (Physics.Raycast(start.position + dir * 0.5f, diff.normalized, out hit, diff.magnitude - 1.0f))
        {
            OpenNode(start);

            bool pathFound = false;

            while(openNodes.Count > 0)
            {
                Node actualNode = GetOpenNode(pfT);
                CloseNode(actualNode);
                Node node = OpenAdyNodes(actualNode);

                if (node)
                {
                    MakePath(ref path, node);
                    pathFound = true;
                    break;
                }
            }

            CleanNodes();

            if (pathFound)
            {
                if (thetaStarMode) PostProcessThetaStar(ref path);
            }
            else
            {
                UIManager.Instance.OnGoalNotOAttainable();
                return null;
            }
        }
        else
            MakePath(ref path, finish);

        return path;
    }

    void CloseNode(Node node)
    {
        node.nodeState = ENodeState.Close;
        openNodes.Remove(node);
        closeNodes.Add(node);
    }

    void OpenNode(Node node)
    {
        node.nodeState = ENodeState.Open;
        openNodes.Add(node);
    }

    void OpenNode(Node node, Node opener)
    {
        node.nodeState = ENodeState.Open;
        node.predecesor = opener;
        node.nodeValue.pathValue += opener.nodeValue.pathValue;
        openNodes.Add(node);
    }

    Node OpenAdyNodes(Node node)
    {
        NodeAdy[] adyNodes = node.GetNodeAdyacents();

        for (int i = 0; i < (int)EAdyDirection.Count; i++)
            if (adyNodes[i].node)
            {
                if (adyNodes[i].node == finishNode)
                {
                    OpenNode(adyNodes[i].node, node);
                    return adyNodes[i].node;
                }
                else if (!adyNodes[i].node.isObstacle && adyNodes[i].node.nodeState == ENodeState.Ok)
                    OpenNode(adyNodes[i].node, node);
            }

        return null;
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
                    int cost = openNodes[i].nodeValue.pathValue + Heuristic(openNodes[i]);
                    if (cost < starLowestValue)
                    {
                        starIndex = i;
                        starLowestValue = cost;
                    }
                }

                node = openNodes[starIndex];
                break;
        }

        return node;
    }

    List<Node> MakePath(ref List<Node> path, Node finish)
    {
        Node actualNode = finish;
        path.Add(actualNode);

        while(actualNode.predecesor)
        {
            actualNode = actualNode.predecesor;
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
            openNodes[0].predecesor = null;
            openNodes[0].nodeValue.ResetPathValue();
            openNodes.RemoveAt(0);
        }

        while(closeNodes.Count > 0)
        {
            closeNodes[0].nodeState = ENodeState.Ok;
            closeNodes[0].predecesor = null;
            closeNodes[0].nodeValue.ResetPathValue();
            closeNodes.RemoveAt(0);
        }
    }

    int Heuristic(Node actualNode)
    {
        return (int)Mathf.Abs(Mathf.Round((actualNode.position - finishNode.position).magnitude));
    }

    void PostProcessThetaStar(ref List<Node> path)
    {
        int actualIndex = path.Count - 1;

        while (actualIndex > 1)
        {
            RaycastHit hit;

            Vector3 diff = path[actualIndex - 2].position - path[actualIndex].position;

            if (!Physics.Raycast(path[actualIndex].position, diff.normalized, out hit, diff.magnitude))
                path.Remove(path[actualIndex - 1]);

            actualIndex--;
        }
    }
}
