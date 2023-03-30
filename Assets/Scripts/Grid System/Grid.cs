using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public static Dictionary<Node, Vector3Int> nodesDictionary = new Dictionary<Node, Vector3Int>();

    public Node[,] nodes;

    [Range(1, 100)]
    public float gapBetweenTheNodes;
    [Space]

    public int width;
    public int height;

    public GameObject cube;

    public GameObject player;

    void Start()
    {
        nodes = new Node[width, height];

        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                nodes[x, y] = new Node(new Vector3(x, 0, y), false);
                nodesDictionary.Add(nodes[x, y], new Vector3Int(x, 0, y));
                Vector3 cubePosition = nodes[x, y].position * gapBetweenTheNodes + new Vector3(gapBetweenTheNodes / 2f, 0f, gapBetweenTheNodes / 2f);
                Instantiate(cube, cubePosition, Quaternion.identity);
            }
        }
        // Generate a random index within the nodes array bounds
        int randomX = Random.Range(0, width);
        int randomY = Random.Range(0, height);

        // Set the player position to the position of the randomly selected node
        Vector3 nodePosition = nodes[randomX, randomY].position;

        // Instantiate the player at the node position with an offset in the y direction
        Vector3 playerPosition = nodePosition * gapBetweenTheNodes + new Vector3(gapBetweenTheNodes / 2f, 1f, gapBetweenTheNodes / 2f);
        Instantiate(player, playerPosition, Quaternion.identity);
    }
}
