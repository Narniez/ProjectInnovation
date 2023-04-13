using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Warning : MonoBehaviour
{
    public string warningText;
    public GameObject warningPrefab;

    GameObject generatedWarning;
    // Start is called before the first frame update
    void Start(){
        generatedWarning = Instantiate(warningPrefab);
        generatedWarning.transform.position = transform.position;
        generatedWarning.transform.localScale = transform.localScale/1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        generatedWarning.GetComponent<TextMeshProUGUI>().text = warningText;
        if (Input.GetKeyDown("w")){
            generatedWarning.transform.SetParent(transform);
        }
    }
}
