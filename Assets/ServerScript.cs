using Unity.Netcode;
using UnityEngine;

public class ServerScript : NetworkBehaviour
{
    public bool startHost = false;
    public static ServerScript instance;
    public GameObject prefab;
    public NetworkVariable<bool> playerTurn =
        new(false, writePerm: NetworkVariableWritePermission.Server,
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

        if (Input.GetKeyDown(KeyCode.H) || startHost)
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
        }
    }

   public void StartHost()
    {
        startHost = true;
    }
}
