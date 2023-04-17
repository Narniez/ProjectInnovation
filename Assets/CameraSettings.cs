using Unity.Netcode;
using UnityEngine;

public class CameraSettings : NetworkBehaviour
{
    private GameObject[] cam;

    public override void OnNetworkSpawn()
    {
        cam = GameObject.FindGameObjectsWithTag("MainCamera");
        if (IsOwnedByServer)
        {
            this.gameObject.tag = "tank1";

            this.transform.position = new Vector3(0, .25f, 0);
            this.transform.rotation = Quaternion.Euler(-90, 180, 0);

            //this.gameObject.transform.position = new Vector3(0.5f, 0, 0.5f);
            foreach (Transform child in transform)
            {
                child.gameObject.layer = 6;
            }
            this.gameObject.layer = 6;
            Camera.main.cullingMask &= ~(1 << 7);
        }
        if (IsClient && !IsOwnedByServer)
        {
            this.gameObject.tag = "tank2";
            this.gameObject.transform.position = new Vector3(11, .25f, 11);
            this.transform.rotation = Quaternion.Euler(-90, 0, 0);

            foreach (Transform child in transform)
            {
                child.gameObject.layer = 7;
            }
            this.gameObject.layer = 7;
            if (IsOwner)
            {
                Camera.main.cullingMask &= ~(1 << 6);
                Camera.main.cullingMask |= (1 << 7);
            }
        }
    }
}
