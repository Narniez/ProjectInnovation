using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Grid : NetworkBehaviour
{

    public static Node[,] nodes;

    [Range(1, 100)]
    public float gapBetweenTheNodes;

    [Space]
    public int rows;
    public int columns;
    public List<GameObject> nodesPrefab;

    private Vector3 nodePosition;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        nodes = new Node[rows, columns];

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject nodePrefab = nodesPrefab[Random.Range(0, nodesPrefab.Count)];
                nodePosition = new Vector3(column * gapBetweenTheNodes, 0, row * gapBetweenTheNodes);

                GameObject nodeObject = Instantiate(nodePrefab, nodePosition, Quaternion.Euler(-90.0f, 0f, 0f), this.gameObject.transform);
                nodeObject.gameObject.GetComponent<NetworkObject>().Spawn();

                NetworkObject node = nodeObject.GetComponent<NetworkObject>();
                Node _node = node.GetComponent<Node>();
                _node.position = new Vector3(column * gapBetweenTheNodes, 0, row * gapBetweenTheNodes);

                _node.row = row;
                _node.column = column;

                nodes[row, column] = _node;
            }
        }

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Node node = nodes[row, column];

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0)
                            continue;

                        int neighbourRow = row + i;
                        int neighbourColumn = column + j;

                        if (neighbourRow < 0 || neighbourRow >= rows || neighbourColumn < 0 || neighbourColumn >= columns)
                            continue;

                        Node neighbourNode = nodes[neighbourRow, neighbourColumn];

                        node.AddNeighbour(neighbourNode);
                    }
                }
            }
        }

    }
}
