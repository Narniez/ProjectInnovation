using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hearts : MonoBehaviour
{

    public GameObject heart;
    public int maxHealth;
    public float currentHealth;

    public List<GameObject> heartImages = new List<GameObject>();


    // Start is called before the first frame update
    void Start(){
        for (int i = 0; i < maxHealth; i++){
            GameObject generatedHeart = Instantiate(heart);
            generatedHeart.transform.position = new Vector3(transform.position.x + i*80, transform.position.y);
            generatedHeart.transform.rotation = Quaternion.identity;
            generatedHeart.transform.localScale = transform.localScale/1.5f;

            generatedHeart.transform.SetParent(transform);
            generatedHeart.SetActive(false);
            heartImages.Add(generatedHeart);
        }
        
    }

    // Update is called once per frame
    void Update(){
        if (currentHealth <= maxHealth){
            for (int i = 0; i < maxHealth; i++){
                if (i < currentHealth){
                    heartImages[i].SetActive(true);
                } else {
                    heartImages[i].SetActive(false);
                }
            }
            
        }
    }
}
