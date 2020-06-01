using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitychanAudio : MonoBehaviour
{
    public AudioSource chanAudioSource;
    public AudioClip[] chanAudio;
    
    void Start()
    {
        InvokeRepeating("PlayAudio", 15f, 15f);
    }
        
    void PlayAudio()
    {
        chanAudioSource.clip = chanAudio[Random.Range(0, chanAudio.Length)];
        chanAudioSource.Play();

    }
}
