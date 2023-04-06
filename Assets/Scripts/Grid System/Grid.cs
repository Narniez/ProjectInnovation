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

    public Vector3 nodePosition;

    public GameObject cube;

    public List<GameObject> nodesPrefab;
    public GameObject player;
    public bool playerShoot = false;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        nodes = new Node[rows, columns];

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
  
                //Calculate the position of the node based on the row, column, and spacing
               
                //Instantiate the node object and get its Node component
                GameObject nodePrefab = nodesPrefab[Random.Range(0, nodesPrefab.Count)];
                nodePosition = new Vector3(column * gapBetweenTheNodes, 0, row * gapBetweenTheNodes);
                GameObject nodeObject = Instantiate(nodePrefab, nodePosition, Quaternion.Euler(-90.0f, 0f, 0f), this.gameObject.transform);

                Node node = nodeObject.GetComponent<Node>();
                node.position = new Vector3(column * gapBetweenTheNodes, 0, row * gapBetweenTheNodes);
             



                //Assign the row and column of the node based on its position in the grid 
                node.row = row;
                node.column = column;

                //Store the node in the nodes array
                nodes[row, column] = node;
                //When you click on a node change the color of all neighbours for easier testing
               // node.OnClick.AddListener(() => ChangeNeighborColors(node));
                node.OnClick.AddListener(() => NodeScan(node));

                //When you click on a node destroy it and replace it with a destroyedNode asset
                //node.OnClick.AddListener(() => { if (TankScript.tankChosen) node.DestroyNode(node);});
                node.OnClick.AddListener(() => node.DestroyNode(node));
            }
            //this.gameObject.GetComponent<NetworkObject>().Spawn();
        }

        //Assign neighbours to each node

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Node node = nodes[row, column];

                //Check neighbouring nodes in all directions
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        //Skip the current node
                        if (i == 0 && j == 0)
                            continue;

                        int neighbourRow = row + i;
                        int neighbourColumn = column + j;

                        //Skip nodes outside of the grid
                        if (neighbourRow < 0 || neighbourRow >= rows || neighbourColumn < 0 || neighbourColumn >= columns)
                            continue;

                        //Get the neighbouring node from the nodes array
                        Node neighbourNode = nodes[neighbourRow, neighbourColumn];

                        //Add the neighbouring node to the current node's list of neighbours 
                        node.AddNeighbour(neighbourNode);
                    }
                }
            }
        }
    }

    private void Update()
    {
        
    }

    void NodeScan(Node clickedNode)
    {
        for(int column = 0; column < columns; column++)
        {
            Node node = nodes[clickedNode.row, column];
            node.ScanNode();
        }

        for(int row = 0; row < rows; row++)
        {
            Node node = nodes[row, clickedNode.column];
            node.ScanNode();
        }
    }

    void ChangeNeighborColors(Node clickedNode)
    {
        var neihbours = clickedNode.GetNeighbours();

        foreach (var neighbour in neihbours)
        {
            neighbour.GetComponent<Renderer>().materials[1].color = Color.red;
        }
    }

}
