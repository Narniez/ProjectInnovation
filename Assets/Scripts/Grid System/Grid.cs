using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Grid : NetworkBehaviour
{
    public static Dictionary<Node, Vector3Int> nodesDictionary = new Dictionary<Node, Vector3Int>();

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
    public List<GameObject> nodesPrefabs;
    public bool playerShoot = false;
    TankScript tank;

    public static UnityAction onServerJoined;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        nodes = new Node[rows, columns];

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                //nodes[row, column] = new Node(new Vector3(row, 0, column), false);
                //nodesDictionary.Add(nodes[row, column], new Vector3Int(row, 0, column));

                //Calculate the position of the node based on the row, column, and spacingfab
                //Vector3 cubePosition = nodes[row, column].position * gapBetweenTheNodes + new Vector3(gapBetweenTheNodes / 2f, 0f, gapBetweenTheNodes / 2f);
                //Instantiate the node object and get its Node component
                GameObject nodePrefab = nodesPrefab[Random.Range(0, nodesPrefab.Count)];
                nodePosition = new Vector3(column * gapBetweenTheNodes, 0, row * gapBetweenTheNodes);
                GameObject nodeObject = Instantiate(nodePrefab, nodePosition, Quaternion.Euler(0.0f, 0f, 0f), this.gameObject.transform);
                nodeObject.gameObject.GetComponent<NetworkObject>().Spawn();
                NetworkObject node = nodeObject.GetComponent<NetworkObject>();
                 node.gameObject.GetComponent<Node>().position = new Vector3(column * gapBetweenTheNodes, 0, row * gapBetweenTheNodes);

                //Assign the row and column of the node based on its position in the grid 
                node.gameObject.GetComponent<Node>().row = row;
                node.gameObject.GetComponent<Node>().column = column;

                //Store the node in the nodes array
                nodes[row, column] = node.gameObject.GetComponent<Node>();

               // node.OnClick.AddListener(() => NodeScan(node));
                //node.OnClick.AddListener(() => ChangeNeighborColors(node));

                //When you click on a node destroy it and replace it with a destroyedNode asset
                //node.OnClick.AddListener(() => node.DestroyNode(node));
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
    
    [ServerRpc(RequireOwnership = false)]
    void NodeBehaviourServerRpc(NetworkBehaviourReference node)
    {
        if (node.TryGet<Node>(out Node nodee))
        {
            //Destroy(nodee.gameObject);

        }
    }

    [ClientRpc]
    void NodeBehaviourClientRpc(NetworkBehaviourReference node)
    {
        if (node.TryGet<Node>(out Node nodee))
        {
            //Destroy(nodee.gameObject);       
        }
    }

    public Node[,] GetGridNodes()
    {
        return nodes;
    }

    private void Update()
    {
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.gameObject.tag == "Node")
                {
                    if (!IsServer) { }
                    //NodeBehaviourServerRpc(hit.collider.gameObject.GetComponent<Node>());
                    //NodeBehaviourClientRpc(hit.collider.gameObject.GetComponent<Node>());
                }
            }
        }
    }


    public void ChangeNeighborColors(Node clickedNode)
    {
        var neihbours = clickedNode.GetNeighbours();

        foreach (var neighbour in neihbours)
        {
            if(!neighbour.isDestroyed)
            neighbour.GetComponent<Renderer>().material.color = Color.red;
        }
    }

}
