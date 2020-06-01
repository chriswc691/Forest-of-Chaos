using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay_Menu : MonoBehaviour
{
    public AudioSource bgmAudioSource;

    void Start()
    {
        bgmAudioSource.PlayDelayed(7f);
    }
}
