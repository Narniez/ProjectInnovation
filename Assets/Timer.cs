using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : NetworkBehaviour
{
    public static Timer Instance;
    public TextMeshProUGUI timerText;
    public Image uiText;
    public float duration;
    public float time = 20;
    public float actualTime = 20;
    bool hasReset = false;
    string playerText = "Player 1's turn ";
    bool canStart = false;

    int clientCounter = 0;
    public GameObject tank;
    public GameObject tank2;
    TankScript tankS;
    TankScript tank2S;
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        uiText.fillAmount = duration;
        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            clientCounter++;
            Debug.Log("A client joined");

            tank = GameObject.FindGameObjectWithTag("tank1");
            tankS = tank.GetComponent<TankScript>();

            if (clientCounter == 2)
            {
                tank2 = GameObject.FindGameObjectWithTag("tank2");
                tank2S = tank2.GetComponent<TankScript>();
            }
        };
    }

    private void FixedUpdate()
    {
        Debug.Log(playerText);
        if (clientCounter == 2 && tankS.tankPlaced.Value && tank2S.tankPlaced.Value)
        {
            StartTimerServerRpc(playerText);
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void StartTimerServerRpc(string timeText)
    {
        StartTimerClientRpc(timeText);
    }

    public void ResetTimer()
    {
        Debug.Log("RESET TIMER");
        duration = 20;
        time = duration;
        uiText.fillAmount = duration;
    }

    [ClientRpc]
    private void StartTimerClientRpc(string timeText)
    {
        time -= Time.fixedDeltaTime;
        uiText.fillAmount -= 1.0f / duration * Time.fixedDeltaTime;
        timerText.text = timeText + time.ToString("00:00");
        if (time <= 0)
        {
            // Change turn and start timer again
            if (ServerScript.instance.playerTurn.Value)
            {
                playerText = "Player 2's turn";
                ResetTimer();
            }
            else
                playerText = "Player 1's turn";
            ServerScript.instance.playerTurn.Value = !ServerScript.instance.playerTurn.Value;
            ResetTimer();
        }
    }
}
