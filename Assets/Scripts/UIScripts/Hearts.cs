using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Hearts : MonoBehaviour
{

    public GameObject heart;
    public int maxHealth;
    [Range(0,3)]
    public float currentHealth;
    public GameObject tank;
    List<GameObject> heartImages = new List<GameObject>();
    Color colorActive = new Color(1f,1f,1f);
    Color colorInactive = new Color(0.5f,0.5f,0.5f);

    // Start is called before the first frame update
    void Start(){
        for (int i = 0; i < maxHealth; i++){
            GameObject generatedHeart = Instantiate(heart);
            generatedHeart.transform.position = new Vector3(transform.position.x + i*80, transform.position.y);
            generatedHeart.transform.localScale = transform.localScale/1.5f;

            generatedHeart.transform.SetParent(transform);
            heartImages.Add(generatedHeart);
        }
    }

    // Update is called once per frame
    void Update(){
        if (currentHealth <= maxHealth){
            for (int i = 0; i < maxHealth; i++){
                if (i < currentHealth)
                {
                    heartImages[i].transform.GetChild(0).GetComponent<Image>().color = colorActive;
                    heartImages[i].transform.GetChild(1).GetComponent<Image>().color = colorActive;
                    
                    if(!(currentHealth % 1 == 0) && (currentHealth-i < 1)){
                        heartImages[i].transform.GetChild(1).GetComponent<Image>().color = colorInactive;
                    }

                } else {
                    
                    heartImages[i].transform.GetChild(0).GetComponent<Image>().color = colorInactive;
                    heartImages[i].transform.GetChild(1).GetComponent<Image>().color = colorInactive;
                }
            }
        }
    }
}
