using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{ 
    public Vector3 position;
    public bool damaged;

    public Node(Vector3 position, bool damaged) { 
        this.position = position;
        this.damaged = damaged;
    }
}
