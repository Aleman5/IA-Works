using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGenerator : MonoBehaviour
{
    [SerializeField] GameObject nodeObject;
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
        Vector3 actualPos = new Vector3(-(planeWidth / 2), 0.5f, -(planeHeight / 2));

        for (int i = 0; i < planeWidth; i++)
        {
            for (int j = 0; j < planeHeight; j++)
            {
                RaycastHit hit;

                if (Physics.Raycast(actualPos, Vector3.down, out hit, actualPos.y))
                {
                    if (hit.collider.tag != "Obstacle")
                    {
                        Node node = Instantiate(nodeObject, actualPos, Quaternion.identity, nodeParent).GetComponent<Node>();
                        nodes[i][j] = node;

                        if (hit.collider.tag == "Mine" || hit.collider.tag == "Base")
                            node.IsObstacle = true;
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
                    if (i+1 < planeWidth  && nodes[i+1][j]) nodes[i][j].AddAdyNode(nodes[i+1][j], ENodeAdyType.Straight, EAdyDirection.Up);
                    if (j+1 < planeHeight && nodes[i][j+1]) nodes[i][j].AddAdyNode(nodes[i][j+1], ENodeAdyType.Straight, EAdyDirection.Right);
                    if (i-1 >= 0          && nodes[i-1][j]) nodes[i][j].AddAdyNode(nodes[i-1][j], ENodeAdyType.Straight, EAdyDirection.Down);
                    if (j-1 >= 0          && nodes[i][j-1]) nodes[i][j].AddAdyNode(nodes[i][j-1], ENodeAdyType.Straight, EAdyDirection.Left);

                    // Diagonales
                    if (i+1 < planeWidth && j+1 < planeHeight && nodes[i+1][j+1]) nodes[i][j].AddAdyNode(nodes[i+1][j+1], ENodeAdyType.Diagonal, EAdyDirection.UpRight);
                    if (i+1 < planeWidth && j-1 >= 0          && nodes[i+1][j-1]) nodes[i][j].AddAdyNode(nodes[i+1][j-1], ENodeAdyType.Diagonal, EAdyDirection.UpLeft);
                    if (i-1 >= 0         && j+1 < planeHeight && nodes[i-1][j+1]) nodes[i][j].AddAdyNode(nodes[i-1][j+1], ENodeAdyType.Diagonal, EAdyDirection.DownRight);
                    if (i-1 >= 0         && j-1 >= 0          && nodes[i-1][j-1]) nodes[i][j].AddAdyNode(nodes[i-1][j-1], ENodeAdyType.Diagonal, EAdyDirection.DownLeft);
                }
            }
        }
    }

    public Node GetClosestNode(Vector3 pos)
    {
        int x = (int)(pos.x - planeWidth / 2 - 1);
        int y = (int)(pos.z - planeHeight / 2 - 1);

        return nodes[x][y];
    }
}