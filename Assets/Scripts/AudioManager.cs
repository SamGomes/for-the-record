using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager{

    private AudioSource themeIntro;
    private AudioSource themeLoop;

    public void PlayInfinitClip(GameObject invoker, AudioClip intro, AudioClip loop)
    {
        themeIntro = invoker.AddComponent<AudioSource>();
        themeLoop = invoker.AddComponent<AudioSource>();
        
        //play theme song
        themeIntro.clip = Resources.Load<AudioClip>("Audio/theme/themeIntro");
        themeLoop.clip = Resources.Load<AudioClip>("Audio/theme/themeLoop");
        themeIntro.Play();
        themeLoop.PlayDelayed(themeIntro.clip.length);
        themeLoop.loop = true;
    }
}
