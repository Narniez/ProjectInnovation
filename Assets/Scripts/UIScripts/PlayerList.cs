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
    float time;
    bool spacePressed = false;
    

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
            time = Time.realtimeSinceStartup;
            spacePressed = true;
        }

        if(spacePressed){
            for (int i = 0; i < players.Count; i ++){
                //The main part of the script starts from here
                // players[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontStyle |= FontStyles.Strikethrough | FontStyles.Italic;
                players[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "<s color=#FF0000>" + names[i] + "</s>";                
                players[i].GetComponent<Image>().sprite = spriteExplosion;

                
                if(Time.realtimeSinceStartup - time >= 1){
                    players[i].GetComponent<Image>().sprite = spriteX;

                    players[i].GetComponent<Image>().color = new Color(1f,1f,1f,0.5f);
                    players[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(1f,1f,1f,0.5f);
                }
            }
        }
    }
}
