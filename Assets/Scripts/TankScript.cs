using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScript : MonoBehaviour
{
    // Start is called before the first frame update

    private GameManager clickedTile;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(Vector3 newVec)
    {
        transform.localPosition = new Vector3(newVec.x, 2, newVec.z);
    }
}
