using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private string[] sounds;
    public void PlaySound(int soundID) => AudioManager.Instance.StartStopSound(sounds[soundID]);
}
