using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Node[,] nodes;

    public int width;
    public int height;

    public GameObject cube;

    void Start()
    {
        nodes = new Node[width, height];

        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                nodes[x, y] = new Node(new Vector3(x,0,y));
                Instantiate(cube, nodes[x,y].position, Quaternion.identity);
            }
        }
    }
}
