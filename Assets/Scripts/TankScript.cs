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
        //Debug.Log("NODES IN NODES LIST IN TANK SCRIPT" + nodes[0, 1]);
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
            if (Input.GetMouseButtonDown(0) && hit.collider.gameObject.CompareTag("Node"))
            {
                Debug.Log("Hit node! " + hit.collider.gameObject.tag);
                FirstNodeServerRpc(hit.collider.gameObject.GetComponent<Node>());
                //SelectNodeServerRpc(hit.collider.gameObject.GetComponent<Node>().transform.position);
            }
        }

        if (!canShoot && Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit) && tankPlaced.Value)
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
            NodeScan(hit.collider.gameObject);
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
                nodee.isOccupied.Value = true;
                currentNode = nodee.NetworkObject;
                currentNode.GetComponent<Node>().isOccupied.Value = true;
                this.gameObject.transform.position = currentNode.transform.position + new Vector3(0.5f, 0, 0.5f);
            }
        }
    }

    public void NodeScan(GameObject objectHit)
    {
        //RaycastHit hit;
        RaycastHit[] hits;
        Vector3[] directions = new Vector3[] { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
        for (int i = 0; i < directions.Length; i++)
        {
            hits = Physics.RaycastAll(objectHit.transform.position, directions[i], 100.0F);

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
            }
        }

    }

    IEnumerator ResetColorsAfterDelay(Material[] materials, Color[] originalColors, float delay)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i]; // Reset color
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
    void ChangeTurnLogicServerRpc()
    {
        //StartCoroutine(TurnChange());
        ServerScript.instance.playerTurn.Value = !ServerScript.instance.playerTurn.Value;
        Debug.Log(ServerScript.instance.playerTurn.Value);
        Debug.Log("The turn has been changed");
    }

    IEnumerator TurnChange()
    {
        yield return new WaitForSeconds(2f);
        yield return new WaitUntil(() => true);
    }
}
