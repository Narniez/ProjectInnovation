using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using System.Collections.Generic;

public class Multiplayer : NetworkBehaviour
{
    public List<Vector2> pos;
    public GameObject dis;

    public NetworkVariable<bool> readyToPlay = new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Owner);

    public NetworkVariable<bool> everyoneReady = new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Server);

    //public NetworkVariable<float> timerBeforeStart = new NetworkVariable<float>(3, writePerm: NetworkVariableWritePermission.Server);


    public float timer;
    public GameObject textT;
    public GameObject timerText;
    public GameObject toggleCheck;

    private void Start()
    {
        //if (!IsOwner) return;

        if ((int)OwnerClientId == 0)
        {
            toggleCheck = GameObject.Find("ReadyCheckPlayer1");
            readyToPlay.Value = toggleCheck.GetComponent<Toggle>().isOn;
            this.transform.position = pos[0];
        }
        else
        {
            toggleCheck = GameObject.Find("ReadyCheckPlayer2");
            readyToPlay.Value = toggleCheck.GetComponent<Toggle>().isOn;
            this.transform.position = pos[1];
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        toggleCheck.GetComponent<Toggle>().interactable = true;
        if (everyoneReady.Value)
        {
            TimerChanger();
            textT.GetComponent<TextMeshProUGUI>().text = timer.ToString("0");

            if (timer < 0)
            {
                timer = 0;

                dis = GameObject.Find("Relay");
                if (dis != null && dis.gameObject.activeSelf)
                {
                    dis.SetActive(false);
                }
                textT.GetComponent<TextMeshProUGUI>().text = "";
                StartTheGame();
            }
        }
        else
        {
            readyToPlay.Value = toggleCheck.GetComponent<Toggle>().isOn;
            CheckIfReadyServerRpc();

            if (Input.GetKey(KeyCode.Space))
            {
                readyToPlay.Value = true;

                Debug.Log(readyToPlay.Value);
            }
        }
    }

    [ServerRpc]
    void CheckIfReadyServerRpc()
    {
        if (!IsServer) return;
        foreach (GameObject players in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (players.GetComponent<Multiplayer>().readyToPlay.Value == false) return;
        }
        foreach (GameObject players in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.GetComponent<Multiplayer>().everyoneReady.Value = true;
        }
    }

    //[ServerRpc]
    void StartTheGame()
    {
        if (!IsOwner) return;
        for (int i = 0; i < GameObject.Find("QuestionsHolder").transform.childCount; i++)
        {
            GameObject.Find("QuestionsHolder").transform.GetChild(i).gameObject.SetActive(true);
        }


        if (GameObject.Find("MultiplayerSceneHolder") != null)
            GameObject.Find("MultiplayerSceneHolder").SetActive(false);

        Debug.Log("IGRATA POCHA IURUUUUUUUUUSH!!!1");
    }

    void TimerChanger()
    {
        if (!IsOwner) return;
        if (everyoneReady.Value)
            timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Debug.Log("bla bla");
            return;
        }
    }
}