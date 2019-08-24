using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGenerator : MonoBehaviour
{
    [SerializeField] GameObject nodeObject;
    [SerializeField] Transform nodeParent;
    [SerializeField] Transform ground;

    List<List<Node>> nodes;
    int planeSize;

    void Awake()
    {
        planeSize = (int)ground.localScale.x * 10 - 1;

        nodes = new List<List<Node>>();
        for (int i = 0; i < planeSize; i++)
            nodes.Add(new List<Node>());

        for (int i = 0; i < planeSize; i++)
            for (int j = 0; j < planeSize; j++)
                nodes[i].Add(null);
    }

    void Start()
    {
        Vector3 actualPos = new Vector3(-(planeSize / 2), 0.5f, -(planeSize / 2));

        for (int i = 0; i < planeSize; i++)
        {
            for (int j = 0; j < planeSize; j++)
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
            actualPos.x = -(planeSize / 2);
            actualPos.z += 1.0f;
        }

        for (int i = 0; i < planeSize; i++)
        {
            for (int j = 0; j < planeSize; j++)
            {
                if (nodes[i][j])
                {
                    // Directos
                    if (i+1 < planeSize && nodes[i+1][j]) nodes[i][j].AddAdyNode(nodes[i+1][j]);
                    if (j+1 < planeSize && nodes[i][j+1]) nodes[i][j].AddAdyNode(nodes[i][j+1]);
                    if (i-1 > 0         && nodes[i-1][j]) nodes[i][j].AddAdyNode(nodes[i-1][j]);
                    if (j-1 > 0         && nodes[i][j-1]) nodes[i][j].AddAdyNode(nodes[i][j-1]);

                    // Diagonales
                    if (i+1 < planeSize && j+1 < planeSize && nodes[i+1][j+1]) nodes[i][j].AddAdyNode(nodes[i+1][j+1]);
                    if (i+1 < planeSize && j-1 > 0         && nodes[i+1][j-1]) nodes[i][j].AddAdyNode(nodes[i+1][j-1]);
                    if (i-1 > 0         && j+1 < planeSize && nodes[i-1][j+1]) nodes[i][j].AddAdyNode(nodes[i-1][j+1]);
                    if (i-1 > 0         && j-1 > 0         && nodes[i-1][j-1]) nodes[i][j].AddAdyNode(nodes[i-1][j-1]);
                }
            }
        }
    }
}