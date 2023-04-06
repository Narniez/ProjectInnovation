using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class TankScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject controlsPanel;
    private GameManager gameManager;
    public static bool tankChosen = false;
    bool tankPlaced = false;
    Ray ray;
    RaycastHit hit;
    public int numMoves = 2;

     Button rightButton;
     Button leftButton;
     Button upButton;
     Button downButton;
    public bool canMove = false;
    public bool canMoveOnPhone = false;
    private Node currentNode;
    private Vector3 targetPosition;


    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GyroControls.ObjectClicked += OnObjectClicked;

        //upButton = controlsPanel.transform.Find("Up").GetComponent<Button>();
        //rightButton = GameObject.Find("Controls/Right").GetComponent<Button>();
        //leftButton = GameObject.Find("Controls/Left").GetComponent<Button>();
        //downButton = GameObject.Find("Controls/Down").GetComponent<Button>();

        //upButton.onClick.AddListener(() => Swipe(Direction.Up));
        //downButton.onClick.AddListener(() => Swipe(Direction.Down));
        //rightButton.onClick.AddListener(() => Swipe(Direction.Right));
        //leftButton.onClick.AddListener(() => Swipe(Direction.Left));
        //controlsPanel.SetActive(false);
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
                if(Input.GetTouch(0).phase == TouchPhase.Began)
                TankMove(hit);

            }
            if (canMove && Input.GetMouseButtonDown(0))
            {
                TankMove(hit);
            }
        }
    }
    public void Swipe(Direction direction)
    {
        // Get a list of the neighbours of the current node
        List<Node> neighbours = currentNode.GetNeighbours();

        //Initialise a variable to keep track of the node we want to move to
        Node neighbourNode = null;


        //Loop thrrough each neighbouring node and update the current node based on the direction
        foreach (Node node in neighbours)
        {
            switch (direction)
            {
                case Direction.Up:
                    if (node.row == currentNode.row + 1 && node.column == currentNode.column && node.isWalkable)
                    {
                        neighbourNode = node;
                    }
                    break;

                case Direction.Down:
                    if (node.row == currentNode.row - 1 && node.column == currentNode.column && node.isWalkable)
                    {
                        neighbourNode = node;
                    }
                    break;

                case Direction.Right:
                    if (node.column == currentNode.column + 1 && node.row == currentNode.row && node.isWalkable)
                    {
                        neighbourNode = node;
                    }
                    break;

                case Direction.Left:
                    if (node.column == currentNode.column - 1 && node.row == currentNode.row && node.isWalkable)
                    {
                        neighbourNode = node;
                    }
                    break;
            }

            if (neighbourNode != null)
            {
                break;
            }
        }

        if (neighbourNode != null)
        {
            currentNode = neighbourNode;
            currentNode.GetComponent<Renderer>().materials[3].color = Color.yellow;
            transform.position = currentNode.gameObject.transform.position + new Vector3(-0.5f, 0f, 0.5f);
            currentNode.occupyingObject = gameObject;
        }
        else
        {
            Debug.Log("No more nodes this way :(");
        }
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

    public void SetPosition(Vector3 newVec)
    {
        transform.localPosition = new Vector3(newVec.x, 0, newVec.z);
        GyroControls.ObjectClicked -= OnObjectClicked;
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
