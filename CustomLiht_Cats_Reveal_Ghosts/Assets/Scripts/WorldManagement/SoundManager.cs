using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    [SerializeField] [Range(0.0f, 1.0f)] private float volume = 0.5f;
    [SerializeField] private AudioClip sound = null;
    [SerializeField] private AudioClip secondSound = null;
    private AudioSource usedSound = new AudioSource();

    private void Awake()
    {
        if (secondSound == null)
        {
            usedSound = gameObject.AddComponent<AudioSource>();
            usedSound.clip = sound;
        }
        else
        {
            int rng = Random.Range(0, 100);
            if (rng < 2)
            {
                usedSound = gameObject.AddComponent<AudioSource>() as AudioSource;
                usedSound.clip = secondSound;
            }
            else
            {
                usedSound = gameObject.AddComponent<AudioSource>();
                usedSound.clip = sound;
            }
        }
        
        usedSound.loop = true;
        usedSound.volume = volume;
        PlaySound();
    }

    private void PlaySound()
    {
        if (usedSound == null)
        {
            Debug.Log("WHat is happeneing here");
            return;
        }
        usedSound.Play();
    }    
}
