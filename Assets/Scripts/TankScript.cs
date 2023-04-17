using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TankScript : NetworkBehaviour
{
    public NetworkVariable<float> tankHealth = new(3, readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> canShoot = new(readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> tankPlaced = new(readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> hasMoved = new(readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> canInteract = new(readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    private NetworkObject currentNode;
    public int numMoves = 2;

    public bool canBePlaced = false;
    public bool canScan = false;



    public GameObject hearts;

    private PlayerDead _playerDead;
    public LayerMask layerMask;

    public GameObject explosion;
    public GameObject trail;

    public Button zoomButton;

    [Header("Sounds")]
    public AudioSource soundEffects;

    public AudioClip shooting;
    public AudioClip moving;
    public AudioClip zoomInSounds;
    public AudioClip zoomOutSounds;

    void Start()
    {
        GameObject tank = this.gameObject;
        if (!IsOwner) return;
        zoomButton = GameObject.Find("ZoomButton").GetComponent<Button>();
        _playerDead = GetComponent<PlayerDead>();
        hearts = GameObject.FindGameObjectWithTag("hearts");
        hearts.GetComponent<Hearts>().tank = this.gameObject;
        //GyroControls.ObjectClicked += OnObjectClicked;
    }


    void Update()
    {
        if (!IsOwner) return;
        hearts.gameObject.GetComponent<Hearts>().currentHealth = tankHealth.Value;
        _playerDead.TankDead(tankHealth.Value);
        //CameraZoomBehavServerRpc();
       //neshtoServerRpc(true);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!tankPlaced.Value && Physics.Raycast(ray, out hit, layerMask))
        {
            if (Input.GetMouseButtonDown(0) && hit.collider.gameObject.CompareTag("Node") && canBePlaced)
            {
                FirstNodeServerRpc(hit.collider.gameObject.GetComponent<Node>());
                canBePlaced = false;
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
                if (canShoot.Value && Input.GetMouseButtonDown(0))
                {
                    TankShootServerRpc(hit.collider.gameObject.GetComponent<Node>());

                    ChangeTurnLogicServerRpc();
                }
                if (tankPlaced.Value && !hasMoved.Value && Input.GetMouseButtonDown(0))
                {
                    MoveServerRpc(hit.collider.gameObject.GetComponent<Node>());
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void CameraZoomBehavServerRpc()
    {
        CameraZoomBehavClientRpc();
    }

    [ClientRpc]
    void CameraZoomBehavClientRpc()
    {
        if (!IsOwner) return;
        soundEffects.PlayOneShot(zoomOutSounds);
        GyroControls.Instance.gyroControl = false;
        CameraBehaviour.instance.zoom = false;
    }

    [ServerRpc(RequireOwnership = false)]
    void PlayerTurnsServerRpc(NetworkBehaviourReference hit)
    {
        if (tankPlaced.Value && !canShoot.Value && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Moving player");
            if (hit.TryGet<Node>(out Node nodee))
                MoveServerRpc(nodee);
        }
        else if (canShoot.Value && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Shooting Player");
            if (hit.TryGet<Node>(out Node nodee))
                TankShootServerRpc(nodee);
            canShoot.Value = false;
            ChangeTurnLogicServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void FirstNodeServerRpc(NetworkBehaviourReference curNode)
    {
        if (curNode.TryGet<Node>(out Node nodee))
        {
            CameraClientRpc();
            canInteract.Value = true;
            currentNode = nodee.NetworkObject;
            currentNode.GetComponent<Node>().isOccupied.Value = true;
            currentNode.GetComponent<Node>().occupyingObject = this.gameObject;
            AssignOwnerClientRpc(currentNode.GetComponent<Node>());
            this.transform.position = currentNode.transform.position + new Vector3(0, .45f, 0.1f);
            tankPlaced.Value = true;
        }
    }

    [ClientRpc]
    void CameraClientRpc()
    {
        if (!IsOwner) return;
        CameraBehaviour.instance.zoom = true;
    }

    [ServerRpc(RequireOwnership = false)]
    void MoveServerRpc(NetworkBehaviourReference selectedNode)
    {
        if (hasMoved.Value || !canInteract.Value) return;

        if (selectedNode.TryGet<Node>(out Node nodee))
        {
            if (Vector3.Distance(nodee.gameObject.transform.position, currentNode.gameObject.transform.position) <= 1 && nodee.isWalkable && !nodee.isOccupied.Value)
            {

                // Determine which direction to rotate towards
                Vector3 targetDirection = transform.position - nodee.position;

                // The step size is equal to speed times frame time.
                float singleStep = 10;

                // Rotate the forward vector towards the target direction by one step
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

                // Calculate a rotation a step closer to the target and applies rotation to this object
                transform.rotation = Quaternion.LookRotation(newDirection + new Vector3(0, 180, 0));

                PlayerCloseDetection.checkDistance?.Invoke();

                currentNode.GetComponent<Node>().isOccupied.Value = false;
                if (IsHost)
                {
                    currentNode.GetComponent<Node>().isOccupiedByHost.Value = false;
                }
                else if (!IsServer && IsClient && !IsHost)
                    currentNode.GetComponent<Node>().isOccupiedByClient.Value = false;
                currentNode.GetComponent<Node>().occupyingObject = null;
                nodee.isOccupied.Value = true;
                currentNode = nodee.NetworkObject;
                currentNode.GetComponent<Node>().isOccupied.Value = true;
                AssignOwnerClientRpc(currentNode.GetComponent<Node>());

                currentNode.GetComponent<Node>().occupyingObject = this.gameObject;

                Vector3 destinationPos = currentNode.transform.position + new Vector3(0, .45f, 0.1f);

                MoveToNodeServerRpc(destinationPos);

                numMoves--;
                GameObject go = Instantiate(trail, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 0.5f), this.transform.rotation);
                //go.GetComponent<NetworkObject>().Spawn();
                StartCoroutine(StopParticleSystem(go, 1));

                if (numMoves <= 0)
                {
                    hasMoved.Value = true;
                    canShoot.Value = true;
                    //zoomButton.interactable = false;
                    GyroControls.Instance.gyroControl = true;
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void MoveToNodeServerRpc(Vector3 destination)
    {
        soundEffects.PlayOneShot(moving);
        StartCoroutine(MoveTowardsNode(destination));

    }

    IEnumerator MoveTowardsNode(Vector3 destinationPos)
    {
        canInteract.Value = false;
        // Set the tank's speed
        float speed = 2f;
        while (transform.position != destinationPos)
        {
            // Move the tank towards the clicked node
            transform.position = Vector3.MoveTowards(transform.position, destinationPos, speed * Time.deltaTime);
            yield return null;
        }

        // Set the tank's position to the final destination
        transform.position = destinationPos;
        if (transform.position == destinationPos)
        {
            soundEffects.Stop();
            canInteract.Value = true;
            Debug.Log("stignah");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TankShootServerRpc(NetworkBehaviourReference nodeToShoot)
    {
        bool canTakeFullDmg = false;
        if (nodeToShoot.TryGet<Node>(out Node node))
        {
            ServerCallingServerRpc(node);
            GameObject go = Instantiate(explosion, new Vector3(node.transform.position.x, 0, node.transform.position.z), Quaternion.Euler(new Vector3(-90, 0, 0)));
            go.GetComponent<NetworkObject>().Spawn();
            StartCoroutine(StopParticleSystem(go, 1));

            if (node.isOccupied.Value)
            {
                canTakeFullDmg = true;
                node.occupyingObject.GetComponent<TankScript>().tankHealth.Value--;
                canShoot.Value = false;
            }
            else
            {
                Debug.Log("Should destroy particle and node");
                node.DestroyNodeServerRpc(node);
                canShoot.Value = false;
            }
            if (!canTakeFullDmg)
            {
                float sphereRadius = 1f;
                Collider[] nearbyColliders = Physics.OverlapSphere(node.transform.position, sphereRadius);
                foreach (Collider collider in nearbyColliders)
                {
                    if (collider.CompareTag("Node") && collider.GetComponent<Node>().isOccupied.Value)
                    {
                        //collider.gameObject.GetComponentInChildren<Renderer>().materials[0].color = Color.blue;
                        collider.GetComponent<Node>().occupyingObject.GetComponent<TankScript>().tankHealth.Value -= 0.5f;
                    }
                }
            }
            soundEffects.PlayOneShot(shooting);
            CameraZoomBehavServerRpc();
        }
    }

    IEnumerator StopParticleSystem(GameObject particleSystem, float time)
    {
        Debug.Log("PARTICLE DESTROY");
        yield return new WaitForSeconds(time);
        Destroy(particleSystem);
    }

    [ServerRpc(RequireOwnership = false)]
    void DontScanMeServerRpc(NetworkBehaviourReference noda)
    {
        DontScanMeClientRpc(noda);
    }

    [ClientRpc]
    void DontScanMeClientRpc(NetworkBehaviourReference noda)
    {
        if (noda.TryGet<Node>(out Node node))
        {
            if (node.occupyingObject != null && node.occupyingObject != this.gameObject)
                StartCoroutine(Warning.Instance.displayTime(3));

        }
    }
    [ServerRpc(RequireOwnership = false)]
    void ServerCallingServerRpc(NetworkBehaviourReference objectHit)
    {
        NodeScanClientRpc(objectHit);
    }

    [ClientRpc]
    public void NodeScanClientRpc(NetworkBehaviourReference objectHit)
    {
        Debug.Log("SCANING");
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
                        Renderer renderer1 = hit.collider.gameObject.GetComponentInChildren<Renderer>();
                        Material[] materials = renderer1.materials;
                        Node node = hit.collider.gameObject.GetComponent<Node>();
                        Color[] originalColors = new Color[materials.Length];
                        for (var m = 0; m < renderer1.materials.Length; m++)
                        {
                            if (!node.isOccupied.Value)
                            {
                                originalColors[m] = materials[m].color;
                                renderer1.materials[m].color = Color.red;
                            }
                            else
                            {
                                originalColors[m] = materials[m].color;
                                renderer1.materials[m].color = Color.blue;
                                if (IsHost && IsServer && node.isOccupiedByClient.Value || !IsHost && node.isOccupiedByHost.Value)
                                {
                                    continue;
                                }
                               else
                                    StartCoroutine(Warning.Instance.displayTime(3));
                                                 
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

    IEnumerator CanPlaceTank()
    {
        yield return new WaitForSeconds(0.5f);
        canBePlaced = true;
    }

    IEnumerator TurnChange()
    {
        yield return new WaitForSeconds(1f);
        ServerScript.instance.playerTurn.Value = !ServerScript.instance.playerTurn.Value;
        canShoot.Value = false;
        hasMoved.Value = false;
        numMoves = 2;
    }

}
