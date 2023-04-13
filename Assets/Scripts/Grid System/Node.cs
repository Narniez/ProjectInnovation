using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Node : NetworkBehaviour
{
    public Vector3 position;

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
                GameObject destroyedNode = Instantiate(destroyedObjectPrefab, nodee.transform.position, nodee.transform.rotation);
                destroyedNode.transform.SetParent(nodee.transform.parent);
                nodee.GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}
