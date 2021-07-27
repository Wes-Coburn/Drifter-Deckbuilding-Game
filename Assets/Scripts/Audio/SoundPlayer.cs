using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private string[] sounds;
    [SerializeField] private Sound[] sounds2;
    public void PlaySound(int soundID)
    {
        if (sounds2.Length < 1)
            AudioManager.Instance.StartStopSound(sounds[soundID]);
        else
            AudioManager.Instance.StartStopSound(null, sounds2[soundID]);
    }
}
