using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCloseDetection : NetworkBehaviour
{

    public static UnityAction checkDistance;

    public GameObject client1;
    public GameObject client2;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        client1 = GameObject.FindGameObjectWithTag("tank1");
        client2 = GameObject.FindGameObjectWithTag("tank2");
    }

    private void OnEnable()
    {
        checkDistance += RevealIfEnemyIsClose;
    }

    private void OnDisable()
    {
        checkDistance -= RevealIfEnemyIsClose;
    }

    void RevealIfEnemyIsClose()
    {
        if (client1 == null || client2 == null) return;

        if (client1 != null && client2 != null)
        {
            float distance = Vector3.Distance(client1.transform.position, client2.transform.position);
            ChangaVisibilityServerRpc(distance);
            Debug.Log("blizo sa");

        }
    }

    [ServerRpc(RequireOwnership = false)]
    void ChangaVisibilityServerRpc(float distance)
    {
        ChangaVisibilityClientRpc(distance);
    }

    private void Update()
    {
        
    }

    [ClientRpc]
    void ChangaVisibilityClientRpc(float distance)
    {
        if (distance <= 2.3f)
        {
            Camera.main.cullingMask |= (1 << 6);
            Camera.main.cullingMask |= (1 << 7);
        }
        else
        {
            if (IsClient && IsServer)
            {
                Camera.main.cullingMask &= ~(1 << 7);
                Camera.main.cullingMask |= (1 << 6);

                Debug.Log("Change the host's camera");
            }
            if (IsClient && !IsServer)
            {
                Camera.main.cullingMask &= ~(1 << 6);
                Camera.main.cullingMask |= (1 << 7);

                Debug.Log("Change the clients's camera");
            }
        }
    }
}
