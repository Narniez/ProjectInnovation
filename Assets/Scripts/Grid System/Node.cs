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

    [ServerRpc(RequireOwnership = false)]
    public void DestroyNodeServerRpc(NetworkBehaviourReference node)
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

}
