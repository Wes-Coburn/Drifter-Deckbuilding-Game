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
        StartStopSound("Soundtrack_TitleScene", null, SoundType.Soundtrack);
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

    public void StartStopSound (string sName, Sound sound = null, SoundType soundType = SoundType.SFX, bool isEndSound = false)
    {
        Sound currentSound = null;
        if (sound == null)
        {
            currentSound = Array.Find(sounds, x => x.name == sName);
            if (currentSound == null)
            {
                Debug.LogWarning("SOUND <" + sName + "> NOT FOUND!");
                return;
            }
        }
        else
        {
            /*
            foreach (Sound s in sounds)
            {
                if (s.ToString() == sound.ToString()) currentSound = s;
                break;
            }
            */
            currentSound = Array.Find(sounds, x => x.clip.ToString() == sound.clip.ToString());
            if (currentSound == null)
            {
                AddSoundSource(sound);
                currentSound = sound;
            }
        }

        if (isEndSound)
        {
            currentSound.source.Stop();
            return;
        }

        switch (soundType)
        {
            case SoundType.SFX:
                // blank
                break;
            case SoundType.Soundscape:
                if (CurrentSoundscape == currentSound) return;
                if (CurrentSoundscape != null) CurrentSoundscape.source.Stop();
                CurrentSoundscape = currentSound;
                CurrentSoundscape.source.loop = true;
                break;
            case SoundType.Soundtrack:
                if (CurrentSoundtrack == currentSound) return;
                if (CurrentSoundtrack != null) CurrentSoundtrack.source.Stop();
                CurrentSoundtrack = currentSound;
                CurrentSoundtrack.source.loop = true;
                break;
        }
        currentSound.source.Play();
    }
}
