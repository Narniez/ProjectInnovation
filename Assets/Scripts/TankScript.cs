using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
<<<<<<< HEAD
using Unity.VisualScripting.Antlr3.Runtime;
=======
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
>>>>>>> main
using UnityEngine;

public class TankScript : NetworkBehaviour
{
    [SerializeField]
    public Node[,] nodes;
    public NetworkVariable<bool> canShoot = new(readPerm: NetworkVariableReadPermission.Everyone,
    writePerm: NetworkVariableWritePermission.Server);
    public bool canScan = false;

    public NetworkVariable<bool> hasMoved = new(readPerm: NetworkVariableReadPermission.Everyone,
    writePerm: NetworkVariableWritePermission.Server);


    public Grid grid;

<<<<<<< HEAD
    public NetworkObject currentNode;
    public NetworkVariable<bool> tankPlaced = new(readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    public bool tankCanMove = false;

    private bool tankChosen = false;

    public NetworkVariable<int> tankHealth = new(3, readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);
    private int numMoves = 200;
    private GameObject[] cam;

    public bool hasShooted;
=======
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
>>>>>>> main

    public override void OnNetworkSpawn()
    {
        cam = GameObject.FindGameObjectsWithTag("MainCamera");
        if (IsOwnedByServer)
        {
<<<<<<< HEAD
            this.gameObject.transform.position = new Vector3(0.5f, 0, 0.5f);
            foreach (Transform child in transform)
            {
                child.gameObject.layer = 6;
            }
            this.gameObject.layer = 6;
            Camera.main.cullingMask &= ~(1 << 7);
        }
        if (IsClient && !IsOwnedByServer)
        {
            this.gameObject.transform.position = new Vector3(11.5f, 0, 11.5f);
            foreach (Transform child in transform)
            {
                child.gameObject.layer = 7;
            }
            this.gameObject.layer = 7;
            if (IsOwner)
            {
                Camera.main.cullingMask &= ~(1 << 6);
                Camera.main.cullingMask |= (1 << 7);
            }
=======
            SetPosition(clickedObject.transform.position + new Vector3(-0.5f, 0f, 0.5f));
            currentNode = clickedObject.GetComponent<Node>();
            currentNode.GetComponent<Renderer>().material.color = Color.yellow;
            tankPlaced = true;
>>>>>>> main
        }
    }

    void Start()
    {
        grid = FindAnyObjectByType<Grid>();
        //Debug.Log("NODES IN NODES LIST IN TANK SCRIPT" + nodes[0, 1]);
        //GyroControls.ObjectClicked += OnObjectClicked;
    }

    void Update()
    {
        if (!IsOwner) return;
        TankDead(tankHealth.Value);
        //tankPlaced = !ServerScript.instance.playerTurn.Value;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!tankPlaced.Value && Physics.Raycast(ray, out hit))
        {
<<<<<<< HEAD
            if (Input.GetMouseButtonDown(0) && hit.collider.gameObject.CompareTag("Node"))
            {
                Debug.Log("Hit node! " + hit.collider.gameObject.tag);
                FirstNodeServerRpc(hit.collider.gameObject.GetComponent<Node>());
                //SelectNodeServerRpc(hit.collider.gameObject.GetComponent<Node>().transform.position);
            }
=======
            tankCanMove = !tankCanMove;
>>>>>>> main
        }

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit) && tankPlaced.Value)
        {
<<<<<<< HEAD
            if (IsOwnedByServer && !ServerScript.instance.playerTurn.Value)
            {
                PlayerTurnsServerRpc(hit.collider.gameObject.GetComponent<Node>());
            }

            if (IsClient && !IsOwnedByServer && ServerScript.instance.playerTurn.Value)
            {
                if (IsOwner && !hasMoved.Value && Input.GetMouseButtonDown(0))
                {
                    MoveServerRpc(hit.collider.gameObject.GetComponent<Node>());
                    
                }
                //Debug.Log(hasMoved);
                //Debug.Log(canShoot);
                if (hasMoved.Value && canShoot.Value && Input.GetMouseButtonDown(0))
                {
                    Debug.Log("ami tuk?");
                    TankShootServerRpc(hit.collider.gameObject.GetComponent<Node>());

                    ChangeTurnLogicServerRpc();
                }
                Debug.Log("Player2 move");
                //TankShootServerRpc(hit.collider.gameObject.GetComponent<Node>());
                //if (!hasMoved && Input.GetMouseButtonDown(0))
                //{
                //    Debug.Log("The second player has moved");
                //    MoveServerRpc(hit.collider.gameObject.GetComponent<Node>());
                //    hasMoved = true;
                //    canShoot = true;
                //}
                //else if (canShoot && Input.GetMouseButtonDown(0))
                //{
                //    Debug.Log("The second player has shooted");

                //    TankShootServerRpc(hit.collider.gameObject.GetComponent<Node>());
                //    hasShooted = true;
                //    canShoot = false;
                //    ChangeTurnLogicServerRpc();
                //}

            }

            //if (IsClient && !IsOwnedByServer && ServerScript.instance.playerTurn.Value)
            //{
            //    Debug.Log("Player1 move");
            //    MoveServerRpc(hit.collider.gameObject.GetComponent<Node>());
            //    ChangeTurnLogicServerRpc();
            //}
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            canShoot.Value = !canShoot.Value;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            canScan = !canScan;
        }

        if (Input.GetMouseButtonDown(0) && canScan && Physics.Raycast(ray, out hit))
        {
            NodeScanServerRpc(hit.collider.gameObject.GetComponent<Node>());
        }

        //if (Input.GetMouseButtonDown(0) && canShoot && Physics.Raycast(ray, out hit))
        //{
        //    TankShootServerRpc(hit.collider.gameObject.GetComponent<Node>());
        //}
    }

    [ServerRpc(RequireOwnership = false)]
    void PlayerTurnsServerRpc(NetworkBehaviourReference hit)
    {
        Debug.Log(hit);
        if (IsOwner && !hasMoved.Value && Input.GetMouseButtonDown(0))
        {
            Debug.Log("trqbva da myrda");
            if (hit.TryGet<Node>(out Node nodee))
                MoveServerRpc(nodee);
            //hasMoved = true;
        }
        else if (hasMoved.Value && canShoot.Value && Input.GetMouseButtonDown(0))
        {
            if (hit.TryGet<Node>(out Node nodee))
                TankShootServerRpc(nodee);
            hasShooted = true;
            canShoot.Value = false;
            ChangeTurnLogicServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void FirstNodeServerRpc(NetworkBehaviourReference curNode)
    {
        Debug.Log("Assign the current node!");

        if (curNode.TryGet<Node>(out Node nodee))
        {
            currentNode = nodee.NetworkObject;
            currentNode.GetComponent<Node>().isOccupied.Value = true;
            this.transform.position = currentNode.transform.position + new Vector3(0.5f, 0, 0.5f);
            Debug.Log("First node assigned " + currentNode.name);
            tankPlaced.Value = true;
            currentNode.GetComponent<Node>().occupyingObject = this.gameObject;
            Debug.Log("Cant no longer run this method!");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void MoveServerRpc(NetworkBehaviourReference selectedNode)
    {
        if (hasMoved.Value) return;
        Debug.Log("No current node");
        //if (currentNode == null) return;

        if (selectedNode.TryGet<Node>(out Node nodee))
        {
            Debug.Log(nodee.name);

            if (Vector3.Distance(nodee.gameObject.transform.position, currentNode.gameObject.transform.position) <= 1 && nodee.isWalkable)
            {
                currentNode.GetComponent<Node>().isOccupied.Value = false;
                Debug.Log("Can move to this node");
                nodee.isOccupied.Value = true;
                currentNode = nodee.NetworkObject;
                currentNode.GetComponent<Node>().isOccupied.Value = true;
                currentNode.GetComponent<Node>().occupyingObject = this.gameObject;

                this.gameObject.transform.position = currentNode.transform.position + new Vector3(0.5f, 0, 0.5f);
                Debug.Log("vlizame tuka");
                hasMoved.Value = true;
                canShoot.Value = true;
                Debug.Log(hasMoved); 
                Debug.Log(canShoot);
=======
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.gameObject.tag == "Node")
                {
                    TankMoveServerRpc(hit.collider.gameObject.GetComponent<Node>());
                }
>>>>>>> main
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

    [ServerRpc(RequireOwnership = false)]
    public void TankShootServerRpc(NetworkBehaviourReference nodeToShoot)
    {
        if (nodeToShoot.TryGet<Node>(out Node node))
        {
            Debug.Log("Shoooot" + node.name);
            if (node.isOccupied.Value)
            {
                node.GetComponent<Node>().occupyingObject.GetComponent<TankScript>().tankHealth.Value--;
                canShoot.Value = false;
                Debug.Log("The tank has been attacked");
            }
            else
            {
                node.DestroyNodeServerRpc(node);
                canShoot.Value = false;
            }

        }
    }

<<<<<<< HEAD
    [ServerRpc(RequireOwnership = false)]
    public void NodeScanServerRpc(NetworkBehaviourReference objectHit)
    {
        if (objectHit.TryGet<Node>(out Node nodeToScan))
        {
            //RaycastHit hit;
            RaycastHit[] hits;
            Vector3[] directions = new Vector3[] { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };


            for (int i = 0; i < directions.Length; i++)

            {
                hits = Physics.RaycastAll(nodeToScan.transform.position, directions[i], 100.0F);

                for (int k = 0; k < hits.Length; k++)

                {
                    RaycastHit hit = hits[k];
                    if (hit.collider.gameObject.tag == "Node")
                    {
                        Debug.Log("Node hit is " + hit.collider.gameObject.name + "Node position is " + hit.collider.gameObject.transform.position);
                        Renderer renderer1 = hit.collider.gameObject.GetComponent<Renderer>();
                        Material[] materials = renderer1.materials;
                        Color[] originalColors = new Color[materials.Length];
                        for (var m = 0; m < renderer1.materials.Length; m++)
                        {
                            if (hit.collider.gameObject.GetComponent<Node>().isOccupied.Value == false)
                            {
                                originalColors[m] = materials[m].color;
                                renderer1.materials[m].color = Color.red;

                            }
                            else
                            {
                                originalColors[m] = materials[m].color;
                                renderer1.materials[m].color = Color.blue;
                            }
                        }
                        StartCoroutine(ResetColorsAfterDelay(materials, originalColors, 1.5f));
                    }

=======
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
>>>>>>> main
                }

            }
        }
    }
<<<<<<< HEAD

    IEnumerator ResetColorsAfterDelay(Material[] materials, Color[] originalColors, float delay)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i]; // Reset color
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void ChangeTurnLogicServerRpc()
    {
        StartCoroutine(TurnChange());
     
    }

    IEnumerator TurnChange()
    {
        yield return new WaitForSeconds(2f);
        ServerScript.instance.playerTurn.Value = !ServerScript.instance.playerTurn.Value;
        canShoot.Value = false;
        hasMoved.Value = false;
        hasShooted = false;
    }

    void TankDead(int health)
    {
        if (health > 0) return;

        if (health <= 0)
            Debug.Log("The tank is destroyed");
    }
=======
>>>>>>> main
}
