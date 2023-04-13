using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankScript : NetworkBehaviour
{
    public NetworkVariable<int> tankHealth = new(3, readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> canShoot = new(readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> tankPlaced = new(readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> hasMoved = new(readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    private NetworkObject currentNode;
    private int numMoves = 200;

    private bool canScan = false;
    private bool hasShooted;

    private PlayerDead _playerDead;
    public LayerMask layerMask;

    void Start()
    {
        _playerDead = GetComponent<PlayerDead>();
        //GyroControls.ObjectClicked += OnObjectClicked;
    }

    void Update()
    {
        if (!IsOwner) return;
        _playerDead.TankDead(tankHealth.Value);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!tankPlaced.Value && Physics.Raycast(ray, out hit, layerMask))
        {
            if (Input.GetMouseButtonDown(0) && hit.collider.gameObject.CompareTag("Node"))
            {
                FirstNodeServerRpc(hit.collider.gameObject.GetComponent<Node>());
            }
        }

        if (tankPlaced.Value && Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
        {
            if (IsOwnedByServer && !ServerScript.instance.playerTurn.Value)
            {
                PlayerTurnsServerRpc(hit.collider.gameObject.GetComponent<Node>());
            }

            if (!IsOwnedByServer && ServerScript.instance.playerTurn.Value)
            {
                if (!hasMoved.Value && Input.GetMouseButtonDown(0))
                {
                    MoveServerRpc(hit.collider.gameObject.GetComponent<Node>());
                }
                if (hasMoved.Value && canShoot.Value && Input.GetMouseButtonDown(0))
                {
                    TankShootServerRpc(hit.collider.gameObject.GetComponent<Node>());
                    ChangeTurnLogicServerRpc();
                }
            }
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
            ServerCallingServerRpc(hit.collider.gameObject.GetComponent<Node>());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void PlayerTurnsServerRpc(NetworkBehaviourReference hit)
    {
        if (!hasMoved.Value && Input.GetMouseButtonDown(0))
        {
            if (hit.TryGet<Node>(out Node nodee))
                MoveServerRpc(nodee);
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
        if (curNode.TryGet<Node>(out Node nodee))
        {
            currentNode = nodee.NetworkObject;
            currentNode.GetComponent<Node>().isOccupied.Value = true;
            this.transform.position = currentNode.transform.position + new Vector3(0,.25f,0);
            this.transform.rotation = Quaternion.Euler(-90,0,0);
            tankPlaced.Value = true;
            currentNode.GetComponent<Node>().occupyingObject = this.gameObject;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void MoveServerRpc(NetworkBehaviourReference selectedNode)
    {
        if (hasMoved.Value) return;

        if (selectedNode.TryGet<Node>(out Node nodee))
        {
            if (Vector3.Distance(nodee.gameObject.transform.position, currentNode.gameObject.transform.position) <= 1 && nodee.isWalkable && !nodee.isOccupied.Value)
            {
                PlayerCloseDetection.checkDistance?.Invoke();

                currentNode.GetComponent<Node>().isOccupied.Value = false;
                nodee.isOccupied.Value = true;
                currentNode = nodee.NetworkObject;
                currentNode.GetComponent<Node>().isOccupied.Value = true;
                currentNode.GetComponent<Node>().occupyingObject = this.gameObject;

                this.gameObject.transform.position = currentNode.transform.position + new Vector3(0, .25f, 0);
                this.transform.rotation = Quaternion.Euler(-90, 180, 0);

                hasMoved.Value = true;
                canShoot.Value = true;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TankShootServerRpc(NetworkBehaviourReference nodeToShoot)
    {
        if (nodeToShoot.TryGet<Node>(out Node node))
        {
            if (node.isOccupied.Value)
            {
                node.occupyingObject.GetComponent<TankScript>().tankHealth.Value--;
                canShoot.Value = false;
            }
            else
            {
                node.DestroyNodeServerRpc(node);
                
                canShoot.Value = false;
            }

        }
    }

    [ServerRpc(RequireOwnership = false)]
    void ServerCallingServerRpc(NetworkBehaviourReference objectHit) {
        NodeScanClientRpc(objectHit);
    }

    [ClientRpc]
    public void NodeScanClientRpc(NetworkBehaviourReference objectHit)
    {
        if (objectHit.TryGet<Node>(out Node nodeToScan))
        {
            RaycastHit[] hits;
            Vector3[] directions = new Vector3[] { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };

            for (int i = 0; i < directions.Length; i++)
            {
                hits = Physics.RaycastAll(nodeToScan.transform.position, directions[i], 100.0F);
                for (int k = 0; k < hits.Length; k++)
                {
                    RaycastHit hit = hits[k];
                    if (hit.collider.gameObject.CompareTag("Node"))
                    {
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

}
