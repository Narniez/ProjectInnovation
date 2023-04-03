using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Node : MonoBehaviour
{ 
    public Vector3 position;
    public bool damaged;
    public int row;
    public int column;
    public List<Node> neighbours = new List<Node>();

    public UnityEvent OnClick;

    public Node(Vector3 position, bool damaged) { 
        this.position = position;
        this.damaged = damaged;
    }

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

    private void OnMouseDown()
    {
        if(OnClick != null)
        {
            OnClick.Invoke();
        }
    }
}
