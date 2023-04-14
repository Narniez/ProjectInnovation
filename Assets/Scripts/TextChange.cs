using Unity.Netcode;
using UnityEngine;
using TMPro;
public class TextChange : NetworkBehaviour
{
    public TextMeshProUGUI onScreenInstructions;
    public GameObject tank;
    TankScript tankS;
    bool showText = true;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        tank = GameObject.FindGameObjectWithTag("tank1");
        tankS = tank.GetComponent<TankScript>();
        
    }
    private void Update()
    {
        if (tankS.tankPlaced.Value && !ServerScript.instance.playerTurn.Value && showText)
        {
            onScreenInstructions.text = "You can now move 2 times";
            showText = false;
        }
        if (tankS.canShoot.Value)
        {
            onScreenInstructions.text = "Time to shoot - select a tile to shoot and scan!";
        }
        if (ServerScript.instance.playerTurn.Value)
        {
            onScreenInstructions.text = "Waiting for other player to finish turn";
            showText = true;
        }
    }
}
