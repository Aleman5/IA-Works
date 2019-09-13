using System.Collections.Generic;
using UnityEngine;

public class NodeGenerator : MonoBehaviour
{
    [SerializeField] Transform nodeParent;
    [SerializeField] Transform ground;

    List<List<Node>> nodes;
    int planeWidth;
    int planeHeight;

    void Awake()
    {
        planeWidth  = (int)ground.localScale.x * 10 - 1;
        planeHeight = (int)ground.localScale.z * 10 - 1;

        nodes = new List<List<Node>>();
        for (int i = 0; i < planeWidth; i++)
            nodes.Add(new List<Node>());

        for (int i = 0; i < planeWidth; i++)
            for (int j = 0; j < planeHeight; j++)
                nodes[i].Add(null);
    }

    void Start()
    {
        Vector3 actualPos = new Vector3(-(planeWidth / 2), 6.0f, -(planeHeight / 2));

        for (int i = 0; i < planeWidth; i++)
        {
            for (int j = 0; j < planeHeight; j++)
            {
                RaycastHit hit;

                Node node = gameObject.AddComponent<Node>();
                node.position = new Vector3(actualPos.x, 0.5f, actualPos.z);
                nodes[j][i] = node;

                if (Physics.Raycast(actualPos, Vector3.down, out hit, actualPos.y))
                {
                    Element element = hit.transform.GetComponent<Element>();
                    
                    if (element.elementType == EElement.Mine || element.elementType == EElement.Base || element.elementType == EElement.Obstacle)
                    {
                        node.taken = true;
                        node.isObstacle = true;
                    }
                }
                actualPos.x += 1.0f;
            }
            actualPos.x = -(planeWidth / 2);
            actualPos.z += 1.0f;
        }

        for (int i = 0; i < planeWidth; i++)
        {
            for (int j = 0; j < planeHeight; j++)
            {
                if (nodes[i][j])
                {
                    // Directos
                    if (i+1 < planeWidth  && nodes[i+1][j]) nodes[i][j].AddAdyNode(nodes[i+1][j], EAdyDirection.Up);
                    if (j+1 < planeHeight && nodes[i][j+1]) nodes[i][j].AddAdyNode(nodes[i][j+1], EAdyDirection.Right);
                    if (i-1 >= 0          && nodes[i-1][j]) nodes[i][j].AddAdyNode(nodes[i-1][j], EAdyDirection.Down);
                    if (j-1 >= 0          && nodes[i][j-1]) nodes[i][j].AddAdyNode(nodes[i][j-1], EAdyDirection.Left);

                    // Diagonales
                    if (i+1 < planeWidth && j+1 < planeHeight && nodes[i+1][j+1]) nodes[i][j].AddAdyNode(nodes[i+1][j+1], EAdyDirection.UpRight);
                    if (i+1 < planeWidth && j-1 >= 0          && nodes[i+1][j-1]) nodes[i][j].AddAdyNode(nodes[i+1][j-1], EAdyDirection.UpLeft);
                    if (i-1 >= 0         && j+1 < planeHeight && nodes[i-1][j+1]) nodes[i][j].AddAdyNode(nodes[i-1][j+1], EAdyDirection.DownRight);
                    if (i-1 >= 0         && j-1 >= 0          && nodes[i-1][j-1]) nodes[i][j].AddAdyNode(nodes[i-1][j-1], EAdyDirection.DownLeft);
                }
            }
        }
    }

    public Node GetClosestNode(Vector3 pos)
    {
        int x = (int)Mathf.Round(pos.x + (planeWidth  - 1) * 0.5f);
        int y = (int)Mathf.Round(pos.z + (planeHeight - 1) * 0.5f);

        return nodes[x][y];
    }
}