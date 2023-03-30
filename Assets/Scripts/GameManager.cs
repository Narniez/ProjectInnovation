using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private bool tankPlaced = false;
    private bool playerTurn = true;
    public GameObject playerTank;
    private TankScript tankScript;


    void Start()
    {
        tankScript = playerTank.GetComponent<TankScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlaceTank(GameObject node)
    {
        tankScript = playerTank.GetComponent<TankScript>();

    }
}
