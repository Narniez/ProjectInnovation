using UnityEngine;
using System;

public class GyroControls : MonoBehaviour
{
    public static GyroControls Instance;
    public bool gyroControl;

    [SerializeField] private float movementSpeed = 1f;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Input.gyro.enabled = true;
    }

    private void LateUpdate()
    {
        if (!gyroControl) return;
        GyrohilikopterControl();
    }

    void GyrohilikopterControl()
    {

        // Update the camera's position based on the gyroscope data
        Vector3 newPosition = transform.position;

        if (Input.gyro.attitude.x > 0.2f)
        {
            newPosition.x -= movementSpeed * Time.deltaTime;
        }
        if (Input.gyro.attitude.x < -0.2f)
        {
            newPosition.x += movementSpeed * Time.deltaTime;
        } 
        if (Input.gyro.attitude.y > 0.2f)
        {
            newPosition.z -= movementSpeed * Time.deltaTime;
        }
        if (Input.gyro.attitude.y < -0.2f)
        {
            newPosition.z += movementSpeed * Time.deltaTime;
        }
        transform.position = newPosition;
    }
}