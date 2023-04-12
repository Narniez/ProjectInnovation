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

    [ServerRpc(RequireOwnership = false)]
    public void ScanNodeServerRpc(NetworkBehaviourReference nodeToScan)
    {
        if (nodeToScan.TryGet<Node>(out Node node))
        {
            node.isScanned = true;
            nodeColors = new Color[node.gameObject.GetComponent<Renderer>().materials.Length]; // initialize the nodeColors array with the same length as the array of materials

            for (int i = 0; i < node.gameObject.GetComponent<Renderer>().materials.Length; i++)
            {
                nodeColors[i] = node.gameObject.GetComponent<Renderer>().materials[i].color;
                node.gameObject.GetComponent<Renderer>().materials[i].color = Color.red;

            }
            StartCoroutine(ResetColorCoroutine(this));
            if (node.occupyingObject != null && node.occupyingObject.CompareTag("tank1"))
            {
                node.gameObject.GetComponent<Renderer>().materials[1].color = Color.blue;
                Debug.Log("Player found on row " + this.row + " column: " + node.column);
            }
        }
    }

    private IEnumerator ResetColorCoroutine(Node node)
    {
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < nodeColors.Length; i++)
        {
            node.gameObject.GetComponent<Renderer>().materials[i].color = nodeColors[i];
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
