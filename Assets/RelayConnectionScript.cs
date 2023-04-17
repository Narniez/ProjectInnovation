using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class RelayConnectionScript : MonoBehaviour
{
    public TextMeshProUGUI codeText;

    public TextMeshProUGUI[] playerNameText;

    public TMP_InputField codeInputField;
    public TMP_InputField nicknameInputField;

    public Button clienJoinButton;
    public string nickname;

    private const int _MaxPlayers = 2;

    private async void Start()
    {
        //if (PlayerPrefs.GetString("nickname") != null) { 
        //    nickname = PlayerPrefs.GetString("nickname");
        //    nicknameInputField.text = PlayerPrefs.GetString("nickname");
        //}

        
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("A player logged in! " + AuthenticationService.Instance.PlayerId);
            playerNameText[0].text = PlayerPrefs.GetString("nickname");
        };

        clienJoinButton.onClick.AddListener(JoinRelay);        
    }

    public async void CreateRelay()
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        try
        {
            //if (nicknameInputField.text.Length < 3) return;
            //else {
            //    nickname = nicknameInputField.text;
            //    PlayerPrefs.SetString("nickname", nickname);
            //}
            //Create a new relay with _MaxPlayers
            Allocation a = await RelayService.Instance.CreateAllocationAsync(_MaxPlayers);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
            codeText.text = "Code: " + joinCode;

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                a.RelayServer.IpV4,
                (ushort)a.RelayServer.Port,
                a.AllocationIdBytes,
                a.Key,
                a.ConnectionData
                );

            //NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            throw e;
        }
    }

    public async void JoinRelay()
    {
        try
        {
            JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(codeInputField.text);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                a.RelayServer.IpV4,
                (ushort)a.RelayServer.Port,
                a.AllocationIdBytes,
                a.Key,
                a.ConnectionData,
                a.HostConnectionData);
            //NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            throw e;
        }
    }
}
