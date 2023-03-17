using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private string[] sounds;
    [SerializeField] private Sound[] sounds2;
    public void PlaySound(int soundID)
    {
        if (sounds2.Length < 1) Managers.AU_MAN.StartStopSound(sounds[soundID]);
        else Managers.AU_MAN.StartStopSound(null, sounds2[soundID]);
    }
}
