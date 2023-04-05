using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerList : MonoBehaviour
{
    public string[] names;
    public GameObject player;

    [Space(20)]
    public Sprite spriteExplosion;
    public Sprite spriteX;

    List<GameObject> players = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < names.Length; i++){
            GameObject generatedPlayer = Instantiate(player);
            generatedPlayer.transform.position = new Vector3(transform.position.x, transform.position.y - i*60);
            // generatedHeart.transform.localScale = transform.localScale/1.5f;

            generatedPlayer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = names[i];
            generatedPlayer.transform.SetParent(transform);
            players.Add(generatedPlayer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space")){
                StartCoroutine(WaitTime(5f));
            for (int i = 0; i < players.Count; i ++){
                players[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontStyle |= FontStyles.Strikethrough | FontStyles.Italic;

                players[i].GetComponent<Image>().sprite = spriteExplosion;
                players[i].GetComponent<Image>().sprite = spriteX;
            }
        }
    }

    private IEnumerator WaitTime(float time)
    {
        Debug.Log("Should wait: " + time + " seconds");
        yield return new WaitForSeconds(time);
    }
    
}
