using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TankScript : NetworkBehaviour
{

    bool tankChosen = false;
    public NetworkVariable<bool> tankPlaced = new(readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    public NetworkObject currentNode;

    /// <summary>
    /// PROMENI GO POSLE
    /// </summary>
    private int numMoves = 200;
    public bool tankCanMove = false;


    public Camera cam;

    public override void OnNetworkSpawn()
    {
        if (IsOwnedByServer)
        {
            this.gameObject.transform.position = new Vector3(0.5f, 0, 0.5f);
            //foreach (Transform child in transform)
            //{
            //    child.gameObject.layer = 6;
            //}
            //this.gameObject.layer = 6;
            //Camera.main.cullingMask &= ~(1 << 7);
        }
        if (IsClient && !IsOwnedByServer)
        {

            this.gameObject.transform.position = new Vector3(11.5f, 0, 11.5f);
            //foreach (Transform child in transform)
            //{
            //    child.gameObject.layer = 7;
            //}
            //this.gameObject.layer = 7;
            //cam.cullingMask &= ~(1 << 6);
        }

    }
    void Start()
    {
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

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit) && tankPlaced.Value)
        {
            MoveServerRpc(hit.collider.gameObject.GetComponent<Node>());
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

            if (Vector3.Distance(nodee.gameObject.transform.position, currentNode.gameObject.transform.position) <= 1)
            {
                Debug.Log("Can move to this node");
                currentNode = nodee.NetworkObject;
                this.gameObject.transform.position = currentNode.transform.position + new Vector3(0.5f, 0, 0.5f);
            }
        }
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
