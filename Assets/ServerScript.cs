using Unity.Netcode;
using UnityEngine;

public class ServerScript : NetworkBehaviour
{
    public static ServerScript instance;
    public GameObject prefab;
    public NetworkVariable<bool> playerTurn =
        new(writePerm: NetworkVariableWritePermission.Owner,
        readPerm: NetworkVariableReadPermission.Everyone);

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.H))
            NetworkManager.Singleton.StartHost();
        if (Input.GetKeyDown(KeyCode.C))
            NetworkManager.Singleton.StartClient();

        if (Input.GetKeyDown(KeyCode.Space) && IsServer)
        {
            GameObject go = Instantiate(prefab);
            go.GetComponent<NetworkObject>().Spawn();
            if (Input.GetKeyDown(KeyCode.L))
            {
                go.GetComponent<NetworkObject>().Despawn();
            }
            EndTurn();
        }
        if (playerTurn.Value)
        {
            // Allow Player 1 to make moves
            //..
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
                EndTurn();
            // Wait for Player 2's turn
        }

    }


    public void EndTurn()
    {
        // Switch turn to other player
        playerTurn.Value = !playerTurn.Value;
    }


}