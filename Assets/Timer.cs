using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public Image uiText;

    public float duration;
    public float time;
    private void Start()
    {
        uiText.fillAmount = duration;
        time = duration;
    }
    private void FixedUpdate()
    {
        if (time >= 0) 
        {
            time -= Time.deltaTime;
            uiText.fillAmount -= 1.0f / duration * Time.deltaTime;
            timerText.text = time.ToString("00:00");
            //ServerScript.instance.playerTurn.Value = !ServerScript.instance.playerTurn.Value;
        }
    }
}
