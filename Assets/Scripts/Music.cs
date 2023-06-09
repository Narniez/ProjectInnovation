using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{

    public AudioSource musicSource;
    public AudioClip musicStart;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);    
    }

    void Start()
    {
        musicSource.PlayOneShot(musicStart);
        musicSource.PlayScheduled(AudioSettings.dspTime + musicStart.length);
    }
}
