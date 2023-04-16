using UnityEngine;
using TMPro;
using System.Collections;
public class Warning : MonoBehaviour
{
    public static Warning Instance;
    public string warningText;
    public GameObject warningPrefab;
    bool canShowText = false;

    GameObject generatedWarning;
    // Start is called before the first frame update

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
    void Start()
    {
        generatedWarning = Instantiate(warningPrefab);
        generatedWarning.transform.position = transform.position;
        generatedWarning.transform.localScale = transform.localScale / 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        generatedWarning.GetComponent<TextMeshProUGUI>().text = warningText;
        if (Input.GetKeyDown(KeyCode.W))
        {
            canShowText = true;
            if (canShowText)
            {
                StartCoroutine(displayTime(3));
            }
        }
    }

    public IEnumerator displayTime(float time)
    {
        generatedWarning.SetActive(true);
        generatedWarning.transform.SetParent(transform);
        yield return new WaitForSeconds(time);
        generatedWarning.SetActive(false);
        canShowText = false;
    }
}
