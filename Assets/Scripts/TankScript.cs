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


    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GyroControls.ObjectClicked += OnObjectClicked;

        upButton = controlsPanel.transform.Find("Up").GetComponent<Button>();
        rightButton = GameObject.Find("Controls/Right").GetComponent<Button>();
        leftButton = GameObject.Find("Controls/Left").GetComponent<Button>();
        downButton = GameObject.Find("Controls/Down").GetComponent<Button>();

        upButton.onClick.AddListener(() => Swipe(Direction.Up));
        downButton.onClick.AddListener(() => Swipe(Direction.Down));
        rightButton.onClick.AddListener(() => Swipe(Direction.Right));
        leftButton.onClick.AddListener(() => Swipe(Direction.Left));
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
<<<<<<< HEAD
        if (!IsOwner) return;
        tankPlaced = !ServerScript.instance.playerTurn.Value;
=======
        if (Input.GetKeyDown(KeyCode.P))
        {
            canMove = true;
        }
>>>>>>> parent of cb854ee... Merge branch 'multitashak' into scanning
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0) && hit.collider.gameObject.CompareTag("tank1"))
            {
                Debug.Log("Selected: " + hit.collider.gameObject.name);
                tankChosen = true;
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
        }
        else
        {
            Debug.Log("No more nodes this way :(");
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
