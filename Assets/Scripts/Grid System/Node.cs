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

    //public void DestroyNode(Node node)
    //{
    //    node.isDestroyed = true;
    //    node.isWalkable = false;
    //    if (node.occupyingObject != null)
    //    {
    //        //node.occupyingObject.GetComponent<TankScript>().TakeHealth(node);
    //        node.occupyingObject = null;
    //        return;
    //    }
    //    MeshFilter meshFilter = node.GetComponent<MeshFilter>();
    //    MeshRenderer meshRendered = node.GetComponent<MeshRenderer>();
    //    Vector3 nodePosition = node.transform.position + new Vector3(5f, -1f, 0.0f);
    //    Mesh destroyedMesh = Instantiate(destroyedNodeMesh);

    //    meshFilter.mesh = destroyedMesh;
    //    meshRendered.material = destroyedNodeMaterial;
    //}

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
    {
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < nodeColors.Length; i++)
        {
            node.gameObject.GetComponent<Renderer>().materials[i].color = nodeColors[i];
        }
    }

    private void OnMouseDown()
    {
        if (OnClick != null)
        {
            Debug.Log("Called OnClick Method in Node class");
            OnClick.Invoke();
        }
    }
}
