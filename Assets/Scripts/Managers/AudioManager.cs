using System;
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    /* SINGELTON PATTERN */
    public static AudioManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
        }
    }

    public Sound[] sounds;
    public void StartStopSound (string name, bool start = true)
    {
        Debug.Log("Play sound: " + name);
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound: " + s.name + " not found!");
            return;
        }

        if (start) s.source.Play();
        else s.source.Stop();
    }
}
