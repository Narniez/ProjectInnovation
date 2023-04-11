using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TankScript : NetworkBehaviour
{

    bool tankChosen = false;
    public NetworkVariable<bool> tankPlaced = new(readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);


    public NetworkObject currentNode;

    public Grid grid;

    [SerializeField]
    public Node[,] nodes;
    public bool canShoot = false;
    public bool canScan = false;

    /// <summary>
    /// PROMENI GO POSLE
    /// </summary>
    private int numMoves = 200;
    public bool tankCanMove = false;

    /// <summary>
    /// if isServer you are player1 and if !isServer and isClient you are player2.
    /// player1 can move if the bool is true, player2 can move you the bool is false
    /// </summary>
    public Camera cam;

    public override void OnNetworkSpawn()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (IsOwnedByServer)
        {
            this.gameObject.transform.position = new Vector3(0.5f, 0, 0.5f);
            foreach (Transform child in transform)
            {
                child.gameObject.layer = 6;
            }
            this.gameObject.layer = 6;
            cam.cullingMask &= ~(1 << 7);
        }
        if (IsClient && !IsOwnedByServer)
        {
            this.gameObject.transform.position = new Vector3(11.5f, 0, 11.5f);
            foreach (Transform child in transform)
            {
                child.gameObject.layer = 7;
            }
            this.gameObject.layer = 7;
            cam.cullingMask &= ~(1 << 6);
        }

    }

    void Start()
    {
        grid = FindAnyObjectByType<Grid>();
        nodes = FindAnyObjectByType<Grid>().GetGridNodes();
        Debug.Log("NODES IN NODES LIST IN TANK SCRIPT" + nodes[0, 1]);
        //GyroControls.ObjectClicked += OnObjectClicked;
    }

    void Update()
    {
        if (!IsOwner) return;
        //tankPlaced = !ServerScript.instance.playerTurn.Value;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!tankPlaced.Value && Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0) && hit.collider.gameObject.tag == "Node")
            {
                Debug.Log("Hit node! " + hit.collider.gameObject.tag);
                FirstNodeServerRpc(hit.collider.gameObject.GetComponent<Node>());
                //SelectNodeServerRpc(hit.collider.gameObject.GetComponent<Node>().transform.position);
            }
        }

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit) && tankPlaced.Value && !canShoot)
        {
            if (IsOwnedByServer && !ServerScript.instance.playerTurn.Value)
            {
                Debug.Log("Player1 move");
                MoveServerRpc(hit.collider.gameObject.GetComponent<Node>());
                ChangeTurnLogicServerRpc();
            }
            if (IsClient && !IsOwnedByServer && ServerScript.instance.playerTurn.Value)
            {
                Debug.Log("Player1 move");
                MoveServerRpc(hit.collider.gameObject.GetComponent<Node>());
                ChangeTurnLogicServerRpc();


            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            canShoot = !canShoot;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            canScan = !canScan;
        }

        if (Input.GetMouseButtonDown(0) && canScan && Physics.Raycast(ray, out hit))
        {
            NodeScan(hit.collider.gameObject.GetComponent<Node>());
        }

        if (Input.GetMouseButtonDown(0) && canShoot && Physics.Raycast(ray, out hit))
        {
            TankShootServerRpc(hit.collider.gameObject.GetComponent<Node>());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void FirstNodeServerRpc(NetworkBehaviourReference curNode)
    {
        Debug.Log("Assign the current node!");

        if (curNode.TryGet<Node>(out Node nodee))
        {
            currentNode = nodee.NetworkObject;
            this.transform.position = currentNode.transform.position + new Vector3(0.5f, 0, 0.5f);
            Debug.Log("First node assigned " + currentNode.name);
            tankPlaced.Value = true;
            Debug.Log("Cant no longer run this method!");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void MoveServerRpc(NetworkBehaviourReference selectedNode)
    {
        Debug.Log("No current node");
        //if (currentNode == null) return;

        if (selectedNode.TryGet<Node>(out Node nodee))
        {
            Debug.Log(nodee.name);

            if (Vector3.Distance(nodee.gameObject.transform.position, currentNode.gameObject.transform.position) <= 1 && nodee.isWalkable)
            {
                Debug.Log("Can move to this node");
                currentNode = nodee.NetworkObject;
                this.gameObject.transform.position = currentNode.transform.position + new Vector3(0.5f, 0, 0.5f);

            }
        }
    }

    public void NodeScan(NetworkBehaviourReference clickedNode)
    {
        if (clickedNode.TryGet<Node>(out Node nodeToScan))
        {
            for (int column = 0; column < grid.columns; column++)
            {
                Node node = nodes[nodeToScan.row, column];            
                if (!nodeToScan.isDestroyed)
                    node.ScanNodeServerRpc(nodeToScan);
            }

            for (int row = 0; row < grid.rows; row++)
            {
                Node node = nodes[row, nodeToScan.column];
                if (!node.isDestroyed)
                    node.ScanNodeServerRpc(nodeToScan);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TankShootServerRpc(NetworkBehaviourReference nodeToShoot)
    {
        if (nodeToShoot.TryGet<Node>(out Node node))
        {  
                Debug.Log("Shoooot" + node.name);
                node.DestroyNode(node);          
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void ChangeTurnLogicServerRpc() {
        //StartCoroutine(TurnChange());
        ServerScript.instance.playerTurn.Value = !ServerScript.instance.playerTurn.Value;
        Debug.Log(ServerScript.instance.playerTurn.Value);
        Debug.Log("The turn has been changed");
    }

    IEnumerator TurnChange() {
        yield return new WaitForSeconds(2f); 
    }

    [ServerRpc]
    public void TankMoveServerRpc(NetworkBehaviourReference selectedNode)
    {
        //Debug.Log("eeeee ma neska pih 2 pyti");

        if (selectedNode.TryGet<Node>(out Node nodee))
        {

            if (numMoves > 0)
            {
                // Get a list of the neighbours of the current node
                Node currentNode = nodee;
                Debug.Log(currentNode + " ");
                //List<Node> neighbours = currentNode.GetNeighbours();
                List<Node> neighbours = currentNode.GetNeighbours();

                //Loop through each neighbouring node and check if it's next to the current node and not diagonal to it
                foreach (Node node in neighbours)
                {
                    // If the node is next to the current node, move to it
                    if (node.isWalkable && Vector3.Distance(nodee.transform.position, node.position) < 1.5f && (node.row == currentNode.row ||
                        node.column == currentNode.column) &&
                        Mathf.Abs(node.row - currentNode.row) + Mathf.Abs(node.column - currentNode.column) == 1)
                    {
                        currentNode.GetComponent<Renderer>().materials[1].color = Color.white;
                        nodee = node;
                        currentNode.GetComponent<Renderer>().materials[1].color = Color.yellow;
                        transform.position = currentNode.gameObject.transform.position + new Vector3(0.5f, 0f, 0.5f);
                        currentNode.occupyingObject = gameObject;

                        // Decrement the number of moves remaining
                        numMoves--;

                        // If there are no more moves remaining, disable the controls panel
                        if (numMoves == 0)
                        {
                            break;
                        }

                        break;
                    }
                }
            }
        }
    }
}
