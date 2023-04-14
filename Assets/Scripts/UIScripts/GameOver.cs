using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOver : MonoBehaviour
{
    public GameObject[] ranking = new GameObject[3];
    public string[] winners = new string[3];


    List<Image> podiumImages = new List<Image>();
    // List<string> podiumText = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < ranking.Length; i++){
            podiumImages.Add(ranking[i].transform.GetChild(0).GetComponent<Image>());
            // podiumText.Add(ranking[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
        }

        for (int i = 0; i < winners.Length; i++){
            if(!string.IsNullOrEmpty(winners[i])){
                podiumImages[i].enabled = true;
                // podiumText[i] = winners[i];
                ranking[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = winners[i];
            }else{
                podiumImages[i].enabled = false;
                // podiumText[i] = "";                
                ranking[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = " ";
            }
        }
    }
}
