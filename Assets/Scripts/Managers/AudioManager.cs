using System.Collections.Generic;
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

    [SerializeField] private Sound[] sounds;

    private static readonly List<Sound> activeSounds = new List<Sound>();

    public Sound CurrentSoundscape { get; set; }
    public Sound CurrentSoundtrack { get; set; }
    public enum SoundType
    {
        SFX,
        Soundscape,
        Soundtrack
    }

    private Sound AddSoundSource(Sound sound)
    {
        Sound newSound = new Sound
        {
            source = gameObject.AddComponent<AudioSource>(),
            name = sound.name,
        };
        newSound.source.clip = sound.clip;
        newSound.source.volume = sound.volume;
        newSound.source.pitch = sound.pitch;
        activeSounds.Add(newSound);
        return newSound;
    }

    public void CleanAudioSources()
    {
        List<Sound> noSource = new List<Sound>();
        foreach (Sound s in activeSounds) if (s.source == null) noSource.Add(s);
        if (noSource.Count > 0) Debug.Log("CLEANING <" + noSource.Count + "> SOUNDS!");
        foreach (Sound s in noSource) activeSounds.Remove(s);
    }

    public void StopCurrentSoundscape()
    {
        if (CurrentSoundscape != null)
        {
            CurrentSoundscape.source.Stop();
            CurrentSoundscape = null;
        }
    }

    public void StartStopSound(string sName, Sound sound = null, 
        SoundType soundType = SoundType.SFX, bool isEndSound = false, bool isLooped = false)
    {
        int soundIndex;
        Sound currentSound = null;
        if (sound == null)
        {
            if (string.IsNullOrEmpty(sName)) return;
            soundIndex = activeSounds.FindIndex(x => x.name == sName);
            if (soundIndex == -1)
            {
                Debug.LogWarning("SOUND <" + sName + "> NOT FOUND!");
                return;
            }
            else currentSound = activeSounds[soundIndex];
        }
        else
        {
            soundIndex = activeSounds.FindIndex(x => x.source.clip == sound.clip);
            if (soundIndex != -1) currentSound = activeSounds[soundIndex];
            else currentSound = AddSoundSource(sound);
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
                if (CurrentSoundscape != null)
                {
                    if (CurrentSoundscape.source.clip == currentSound.source.clip) return;
                    CurrentSoundscape.source.Stop();
                }

                CurrentSoundscape = currentSound;
                CurrentSoundscape.source.loop = true;
                break;
            case SoundType.Soundtrack:
                //Debug.LogWarning("SOUNDTRACKS SILENCED!");
                //return; // FOR RECORDING ONLY
//#pragma warning disable CS0162 // Unreachable code detected
                if (CurrentSoundtrack != null)
//#pragma warning restore CS0162 // Unreachable code detected
                {
                    if (CurrentSoundtrack.source.clip == currentSound.source.clip) return;
                    CurrentSoundtrack.source.Stop();
                }

                CurrentSoundtrack = currentSound;
                CurrentSoundtrack.source.loop = true;
                break;
        }
        if (isLooped) currentSound.source.loop = true;
        currentSound.source.Play();
    }
}
