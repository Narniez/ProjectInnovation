using UnityEngine;
using TMPro;
using System.Collections;
using Unity.Netcode;
public class Warning : NetworkBehaviour
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
        generatedWarning.GetComponent<TextMeshProUGUI>().text = warningText;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator displayTime(float time)
    {
        if (IsOwner) yield break;
        generatedWarning.SetActive(true);
        generatedWarning.transform.SetParent(transform);
        yield return new WaitForSeconds(time);
        generatedWarning.SetActive(false);
    }
}
