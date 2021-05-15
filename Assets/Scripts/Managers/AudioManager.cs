using System;
using UnityEngine;

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

    private void Start()
    {
        //StartStopSound("Soundtrack_MenuScene", SoundType.Soundtrack);
    }

    public Sound CurrentSoundscape { get; set; }
    public Sound CurrentSoundtrack { get; set; }
    public Sound[] sounds;
    public enum SoundType
    {
        SFX,
        Soundscape,
        Soundtrack
    }

    public void StartStopSound (string name, SoundType soundType, bool isEndSound = false)
    {
        Sound currentSound = Array.Find(sounds, sound => sound.name == name);
        if (currentSound == null)
        {
            Debug.LogError("Sound: " + name + " not found!");
            return;
        }

        if (isEndSound)
        {
            currentSound.source.Stop();
            return;
        }

        switch (soundType)
        {
            case SoundType.SFX:
                break;
            case SoundType.Soundscape:
                if (CurrentSoundscape == currentSound) return; // TESTING
                if (CurrentSoundscape != null) CurrentSoundscape.source.Stop();
                CurrentSoundscape = currentSound;
                break;
            case SoundType.Soundtrack:
                if (CurrentSoundtrack == currentSound) return; // TESTING
                if (CurrentSoundtrack != null) CurrentSoundtrack.source.Stop();
                CurrentSoundtrack = currentSound;
                break;
        }
        currentSound.source.Play();
    }
}
