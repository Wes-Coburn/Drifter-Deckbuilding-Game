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

        foreach (Sound sound in sounds) AddSoundSource(sound);
    }

    private void Start()
    {
        StartStopSound("Soundtrack_MenuScene", null, SoundType.Soundtrack);
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

    private void AddSoundSource(Sound sound)
    {
        sound.source = gameObject.AddComponent<AudioSource>();
        sound.source.clip = sound.clip;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;
    }

    public void StartStopSound (string name, Sound sound = null, SoundType soundType = SoundType.SFX, bool isEndSound = false)
    {
        Sound currentSound;
        if (sound == null)
        {
            currentSound = Array.Find(sounds, sound => sound.name == name);
            if (currentSound == null)
            {
                //Debug.LogWarning("SOUND <" + name + "> NOT FOUND!");
                return;
            }
        }
        else
        {
            AddSoundSource(sound);
            currentSound = sound;
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
                if (CurrentSoundscape == currentSound) return;
                if (CurrentSoundscape != null) CurrentSoundscape.source.Stop();
                CurrentSoundscape = currentSound;
                break;
            case SoundType.Soundtrack:
                if (CurrentSoundtrack == currentSound) return;
                if (CurrentSoundtrack != null) CurrentSoundtrack.source.Stop();
                CurrentSoundtrack = currentSound;
                break;
        }
        currentSound.source.Play();
    }
}
