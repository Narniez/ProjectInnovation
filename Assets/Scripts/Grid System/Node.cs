using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Node : MonoBehaviour
{
    public Vector3 position;
    public bool isPlayerTurn = true;
    public bool damaged;
    public bool isDestroyed = false;
    public bool isScanned = false;
    public bool isWalkable = true;
    public GameObject occupyingObject = null;
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
        if (node.occupyingObject != null)
        {
            //node.occupyingObject.GetComponent<TankScript>().TakeHealth(node);
            node.occupyingObject = null;
            return;
        }
        MeshFilter meshFilter = node.GetComponent<MeshFilter>();
        MeshRenderer meshRendered = node.GetComponent<MeshRenderer>();
        Vector3 nodePosition = node.transform.position + new Vector3(5f, -1f, 0.0f);
        Mesh destroyedMesh = Instantiate(destroyedNodeMesh);
        meshFilter.mesh = destroyedMesh;

        meshRendered.material = destroyedNodeMaterial;
    }

    private void OnMouseDown()
    {
        if (OnClick != null)
        {
            OnClick.Invoke();
        }
    }
}
