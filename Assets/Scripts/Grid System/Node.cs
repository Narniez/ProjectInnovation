using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class Node : NetworkBehaviour
{
    public Vector3 position;

    public bool isPlayerTurn = true;
    public bool damaged;
    public bool isDestroyed = false;
    public bool isScanned = false;
    public bool isWalkable = true;
    public NetworkVariable<bool> isOccupied = new(false, readPerm: NetworkVariableReadPermission.Everyone, 
        writePerm: NetworkVariableWritePermission.Owner);
    public GameObject occupyingObject = null;
    public GameObject destroyedObjectPrefab;
    public int row;
    public int column;
    public List<Node> neighbours = new List<Node>();

    public Mesh destroyedNodeMesh;
    public Material destroyedNodeMaterial;

    public UnityEvent OnClick;

    private Color[] nodeColors;


    public List<Node> GetNeighbours()
    {
        return neighbours;
    }

    public void AddNeighbour(Node neighbour)
    {
        if (!neighbours.Contains(neighbour))
        {
            neighbours.Add(neighbour);
        }
    }

<<<<<<< HEAD
    [ServerRpc(RequireOwnership = false)]
    public void DestroyNodeServerRpc(NetworkBehaviourReference node)
=======

    public void DestroyNode(Node node)
    {
        node.isDestroyed = true;
        node.isWalkable = false;
        if (node.occupyingObject != null)
        {
            //node.occupyingObject.GetComponent<TankScript>().TakeHealth(node);
            node.occupyingObject = null;
            return;
        }

        // Instantiate destroyed node prefab
        GameObject destroyedNode = Instantiate(destroyedObjectPrefab, node.transform.position, node.transform.rotation);
        destroyedNode.transform.SetParent(node.transform.parent);
        node.GetComponent<NetworkObject>().Despawn();
    }

    public void ScanNode()
    {
        this.isScanned = true;
        nodeColors = new Color[this.gameObject.GetComponent<Renderer>().materials.Length]; // initialize the nodeColors array with the same length as the array of materials

        for (int i = 0; i < this.gameObject.GetComponent<Renderer>().materials.Length; i++)
        {
            nodeColors[i] = this.gameObject.GetComponent<Renderer>().materials[i].color;
            this.gameObject.GetComponent<Renderer>().materials[i].color = Color.red;

        }
        StartCoroutine(ResetColorCoroutine(this));
        if (this.occupyingObject != null && this.occupyingObject.CompareTag("tank1"))
        {
            this.gameObject.GetComponent<Renderer>().materials[1].color = Color.blue;
            Debug.Log("Player found on row " + this.row + " column: " + this.column);
        }

    }

    private IEnumerator ResetColorCoroutine(Node node)
>>>>>>> main
    {
        if (node.TryGet<Node>(out Node nodee))
        {
            nodee.isDestroyed = true;
            nodee.isWalkable = false;

            if (nodee.isOccupied.Value == false)
            {
                // Instantiate destroyed node prefab
                GameObject destroyedNode = Instantiate(destroyedObjectPrefab, nodee.transform.position, nodee.transform.rotation);
                destroyedNode.transform.SetParent(nodee.transform.parent);
                nodee.GetComponent<NetworkObject>().Despawn();
            }
        }
    }

    //private void OnMouseDown()
    //{
    //    if (OnClick != null)
    //    {
    //        Debug.Log("Called OnClick Method in Node class");
    //        OnClick?.Invoke();
    //    }
    //}

<<<<<<< HEAD
=======
    //private void OnMouseDown()
    //{
    //    if (OnClick != null)
    //    {
    //        Debug.Log("Called OnClick Method in Node class");
    //        OnClick?.Invoke();
    //    }
    //}

>>>>>>> main
}
