using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TankScript : NetworkBehaviour
{
    // Start is called before the first frame update
    public Grid grid;

    public GameObject controlsPanel;
    private GameManager gameManager;
    bool tankChosen = false;
    public bool tankPlaced = false;
    Ray ray;
    RaycastHit hit;

    Button rightButton;
    Button leftButton;
    Button upButton;
    Button downButton;

    private Node currentNode;
    private Vector3 targetPosition;
    private int numMoves;
    private bool canMove;
    private bool canMoveOnPhone;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GyroControls.ObjectClicked += OnObjectClicked;

        controlsPanel.SetActive(false);
    }

    void OnObjectClicked(GameObject clickedObject)
    {
        if (tankChosen)
        {
            SetPosition(clickedObject.transform.position + new Vector3(-0.5f, 0f, 0.5f));
            currentNode = clickedObject.GetComponent<Node>();
            currentNode.GetComponent<Renderer>().material.color = Color.yellow;
            tankPlaced = true;
            controlsPanel.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        tankPlaced = !ServerScript.instance.playerTurn.Value;


        if (Input.GetKeyDown(KeyCode.P))
        {
            canMove = true;
        }
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0) && hit.collider.gameObject.CompareTag("tank1"))
            {
                Debug.Log("Selected: " + hit.collider.gameObject.name);
                tankChosen = true;
            }
            if (canMove && canMoveOnPhone)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                    TankMove(hit);

            }
            if (canMove && Input.GetMouseButtonDown(0))
            {
                TankMove(hit);
            }
        }
    }


    public void SetPosition(Vector3 newVec)
    {
        transform.localPosition = new Vector3(newVec.x, 0, newVec.z);
        GyroControls.ObjectClicked -= OnObjectClicked;
    }
    public void TankMove(RaycastHit hit)
    {
        if (numMoves > 0)
        {
            // Get a list of the neighbours of the current node
            List<Node> neighbours = currentNode.GetNeighbours();

            //Loop through each neighbouring node and check if it's next to the current node and not diagonal to it
            foreach (Node node in neighbours)
            {
                // If the node is next to the current node, move to it
                if (node.isWalkable && hit.transform.position == node.position && (node.row == currentNode.row || node.column == currentNode.column) && Mathf.Abs(node.row - currentNode.row) + Mathf.Abs(node.column - currentNode.column) == 1)
                {
                    currentNode.GetComponent<Renderer>().materials[1].color = Color.white;
                    currentNode = node;
                    currentNode.GetComponent<Renderer>().materials[1].color = Color.yellow;
                    transform.position = currentNode.gameObject.transform.position + new Vector3(-0.5f, 0f, 0.5f);
                    currentNode.occupyingObject = gameObject;

                    // Decrement the number of moves remaining
                    numMoves--;

                    // If there are no more moves remaining, disable the controls panel
                    if (numMoves == 0)
                    {
                        break;
                    }

                    break;
                }
            }
        }
    }
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
