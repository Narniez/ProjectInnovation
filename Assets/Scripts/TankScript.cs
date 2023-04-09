using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class TankScript : NetworkBehaviour
{
    // Start is called before the first frame update
    public Grid grid;

    bool tankChosen = false;
    public bool tankPlaced = false;
    Ray ray;
    RaycastHit hit;

    private Node currentNode;
    /// <summary>
    /// PROMENI GO POSLE
    /// </summary>
    private int numMoves = 200;
    public bool tankCanMove = false;
    private bool canMoveOnPhone;


    public List<Node> newList = new List<Node>();


    public override void OnNetworkSpawn()
    {
        if (IsOwnedByServer)
            this.gameObject.transform.position = new Vector3(0.5f, 0, 0.5f);
        if (IsClient && !IsOwnedByServer)
            this.gameObject.transform.position = new Vector3(11.5f, 0, 11.5f);

    }
    void Start()
    {
        GyroControls.ObjectClicked += OnObjectClicked;
    }

    void OnObjectClicked(GameObject clickedObject)
    {
        if (tankChosen)
        {
            SetPosition(clickedObject.transform.position + new Vector3(-0.5f, 0f, 0.5f));
            currentNode = clickedObject.GetComponent<Node>();
            currentNode.GetComponent<Renderer>().material.color = Color.yellow;
            tankPlaced = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        tankPlaced = !ServerScript.instance.playerTurn.Value;


        if (Input.GetKeyDown(KeyCode.P))
        {
            tankCanMove = !tankCanMove;
        }
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.gameObject.tag == "Node")
                {
                    TankMoveServerRpc(hit.collider.gameObject.GetComponent<Node>());
                }
            }
            //if (Input.GetMouseButtonDown(0) && hit.collider.gameObject.CompareTag("tank1"))
            //{
            //    Debug.Log("Selected: " + hit.collider.gameObject.name);
            //    tankChosen = true;
            //}
            //if (tankCanMove && canMoveOnPhone)
            //{
            //    if (Input.GetTouch(0).phase == TouchPhase.Began)
            //        TankMove(hit);

            //}
            //if (tankCanMove && Input.GetMouseButtonDown(0))
            //{
            //    TankMove(hit);
            //}
        }
    }


    public void SetPosition(Vector3 newVec)
    {
        transform.localPosition = new Vector3(newVec.x, 0, newVec.z);
        GyroControls.ObjectClicked -= OnObjectClicked;
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
