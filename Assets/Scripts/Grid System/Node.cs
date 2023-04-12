using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Node : NetworkBehaviour
{
    public Vector3 position;
    public bool isPlayerTurn = true;
    public bool damaged;
    public bool isDestroyed = false;
    public bool isScanned = false;
    private Color[] nodeColors;
    public bool isWalkable = true;
    public NetworkVariable<bool> isOccupied = new(false, readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner);
    public GameObject destroyedObjectPrefab;
    public int row;
    public int column;
    public List<Node> neighbours = new List<Node>();

    public Mesh destroyedNodeMesh;
    public Material destroyedNodeMaterial;

    public UnityEvent OnClick;

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


    public void DestroyNode(Node node)
    {
        node.isDestroyed = true;
        node.isWalkable = false;

        if (node.isOccupied.Value == false)
        {
            // Instantiate destroyed node prefab
            GameObject destroyedNode = Instantiate(destroyedObjectPrefab, node.transform.position, node.transform.rotation);
            destroyedNode.transform.SetParent(node.transform.parent);
            node.GetComponent<NetworkObject>().Despawn();
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
