using System.Runtime.InteropServices.ComTypes;
using Unity.Netcode;
using UnityEngine;

public class CameraBehaviour : NetworkBehaviour
{
    public static CameraBehaviour instance;

    public Camera cam;

    public float zoomSpeed;

    //ZoomIn 
    public bool zoom;
    //ZoomOut

    /// <summary>
    /// WAGA BUGAAA
    /// </summary>
    public GameObject tank;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        cam = GetComponent<Camera>();
        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            if (IsHost) tank = GameObject.FindGameObjectWithTag("tank1");
            else tank = GameObject.FindGameObjectWithTag("tank2");
        };
    }

    public void CameraChange()
    {
        if (cam != null)
        {
            zoom = !zoom;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tank == null) return;
        if (zoom) { 
            ZoomIn(20); 
            this.transform.position = new Vector3(tank.transform.position.x, this.transform.position.y, tank.transform.position.z);
        }
        else { 
            ZoomOut(60);
            this.transform.position = new Vector3(5, 11.5f, 5.6f);
        }
    }

    void ZoomOut(float a)
    {
        if (cam.fieldOfView != a)
        {
            if (cam.fieldOfView < a)
            {
                cam.fieldOfView += zoomSpeed;
                ZoomOut(a);
                if (cam.fieldOfView > a)
                    cam.fieldOfView = a;
            }
        }
    }

    void ZoomIn(float a)
    {
        if (cam.fieldOfView != a)
        {
            if (cam.fieldOfView > a)
            {
                cam.fieldOfView -= zoomSpeed;
                ZoomOut(a);
                if (cam.fieldOfView < a)
                    cam.fieldOfView = a;
            }
        }
    }
}
