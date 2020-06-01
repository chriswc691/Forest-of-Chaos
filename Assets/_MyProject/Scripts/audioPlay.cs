using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioPlay : MonoBehaviour
{

    public AudioSource bgmAudioSource;
    
    void Start()
    {
        bgmAudioSource.PlayDelayed(2.5f);
    }

}
