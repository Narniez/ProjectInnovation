using UnityEngine;
using UnityEngine.EventSystems;

public class GyroControls : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
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
                    renderer.material.color = Random.ColorHSV();
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
                    renderer.material.color = Random.ColorHSV();
                }
            }
        }
    }
}