using Unity.Netcode;
using UnityEngine;
using TMPro;
public class TextChange : NetworkBehaviour
{
    public TextMeshProUGUI onScreenInstructions;
    public GameObject tank;
    public GameObject tank2;
    public GameObject zoomButton;
    TankScript tankS;
    TankScript tank2S;
    int clientCounter=0;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        zoomButton = GameObject.FindGameObjectWithTag("ZoomB");
        tank = GameObject.FindGameObjectWithTag("tank1");
        tankS = tank.GetComponent<TankScript>();

        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            clientCounter++;
            Debug.Log("A client joined");
            if (!IsHost)
            {
                tank2 = GameObject.FindGameObjectWithTag("tank2");
                tank2S = tank2.GetComponent<TankScript>();
            }
            if (clientCounter == 2)
            {
                zoomButton = GameObject.FindGameObjectWithTag("ZoomB");
                tank2 = GameObject.FindGameObjectWithTag("tank2");
                tank2S = tank2.GetComponent<TankScript>();
            }
        };
    }
    private void Update()
    {
        //TextPerAction();
        if (tank != null)
        {
            if (clientCounter <= 1)
                return;
            if (!ServerScript.instance.playerTurn.Value)
            {
                if (!tankS.tankPlaced.Value)
                {
                    onScreenInstructions.text = "Place your tank";
                }
                else
                {
                    if (clientCounter == 2 && IsHost && tankS.tankPlaced.Value && tankS.tankHealth.Value > 0)
                    {
                        Debug.Log("VLIZAM EY TUKA");
                        zoomButton.SetActive(true);
                        onScreenInstructions.text = "You can now move 2 times";
                    }
                    if (IsHost && tankS.canShoot.Value)
                    {
                        zoomButton.SetActive(false);
                        onScreenInstructions.text = "Time to shoot - select a tile to shoot and scan!";
                    }
                    if (IsHost && tankS.tankHealth.Value <= 0)
                    {
                        onScreenInstructions.text = "You have been destroyed!";
                    }
                    if (IsHost && clientCounter == 2 && tank2S.tankHealth.Value <= 0)
                    {
                        onScreenInstructions.text = "You won!";
                    }
                }
            }
            else
            {
                if (IsHost && clientCounter == 2 && tank2S.tankHealth.Value > 0 && tankS.tankHealth.Value > 0)
                {
                    onScreenInstructions.text = "Waiting for other player to finish turn";
                }
                if (!IsHost && clientCounter == 1)
                {
                    if (!tank2S.tankPlaced.Value)
                    {
                        onScreenInstructions.text = "Place your tank";
                    }
                    else
                    {
                        if (tank2S.tankHealth.Value <= 0)
                        {
                            onScreenInstructions.text = "You have been destroyed!";
                        }
                        if (tankS.tankHealth.Value <= 0)
                        {
                            onScreenInstructions.text = "You won!";
                        }
                        if (!tank2S.hasMoved.Value && tank2S.tankHealth.Value > 0) {
                            zoomButton.SetActive(true);
                            onScreenInstructions.text = "You can now move 2 times";
                        } else if (tank2S.canShoot.Value) {
                            zoomButton.SetActive(false);
                            onScreenInstructions.text = "Time to shoot - select a tile to shoot and scan!";
                        }
                        else if (tank2S.tankHealth.Value > 0 && tankS.tankHealth.Value > 0)
                        {
                            onScreenInstructions.text = "Waiting for other player to finish turn";
                        }
                    }
                }
            }
        }
    }
}
