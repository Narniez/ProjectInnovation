using UnityEngine;
using System;

public class GyroControls : MonoBehaviour
{
    public static event Action<GameObject> ObjectClicked;

    private Camera mainCamera;

    Vector3 oldPos;
    [SerializeField] private float movementSpeed = 1f;
    void Start()
    {
        mainCamera = Camera.main;
        Input.gyro.enabled = true;
    }

    void Update()
    {
        Debug.Log(Input.gyro.attitude.y);
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.transform.gameObject;
                Renderer renderer = clickedObject.GetComponent<Renderer>();

                if (renderer != null)
                {
                    renderer.material.color = UnityEngine.Random.ColorHSV();
                    ObjectClicked?.Invoke(clickedObject);
                }
            }
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.transform.gameObject;
                Renderer renderer = clickedObject.GetComponent<Renderer>();

                if (renderer != null)
                {
                    renderer.material.color = UnityEngine.Random.ColorHSV();
                    ObjectClicked?.Invoke(clickedObject);
                }
            }
        }
    }

    private void LateUpdate()
    {
        GyrohilikopterControl();
    }

    void GyrohilikopterControl()
    {

        // Update the camera's position based on the gyroscope data
        Vector3 newPosition = transform.position;


        if (Input.gyro.attitude.x > 0.2f)
        {
            newPosition.z += movementSpeed * Time.deltaTime;
        }
        if (Input.gyro.attitude.x < -0.2f)
        {
            newPosition.z -= movementSpeed * Time.deltaTime;
        } 
        if (Input.gyro.attitude.y > 0.2f)
        {
            newPosition.x -= movementSpeed * Time.deltaTime;
        }
        if (Input.gyro.attitude.y < -0.2f)
        {
            newPosition.x += movementSpeed * Time.deltaTime;
        }
        transform.position = newPosition;
    }
}