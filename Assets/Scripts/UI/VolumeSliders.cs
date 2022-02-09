using UnityEngine;
using UnityEngine.UI;

public class VolumeSliders : MonoBehaviour
{
    [SerializeField] private GameObject musicSlider;
    [SerializeField] private GameObject sfxSlider;  

    private AudioManager auMan;
    private void Start()
    {
        auMan = AudioManager.Instance;
        SetMusicSlider();
        SetSFXSlider();
    }
    public void MusicVolume_OnSlide(float volume)
    {
        if (auMan == null) return;
        auMan.MusicVolume = volume;
    }
    public void MusicVolume_OnPlus()
    {
        auMan.MusicVolume += 0.2f;
        SetMusicSlider();
    }
    public void MusicVolume_OnMinus()
    {
        auMan.MusicVolume -= 0.2f;
        SetMusicSlider();
    }
    private void SetMusicSlider() => musicSlider.GetComponent<Slider>().SetValueWithoutNotify(auMan.MusicVolume);
    public void SFXVolume_OnSlide(float volume)
    {
        if (auMan == null) return;
        auMan.SFXVolume = volume;
    }
    public void SFXVolume_OnPlus()
    {
        auMan.SFXVolume += 0.2f;
        SetSFXSlider();
    }
    public void SFXVolume_OnMinus()
    {
        auMan.SFXVolume -= 0.2f;
        SetSFXSlider();
    }
    private void SetSFXSlider() => sfxSlider.GetComponent<Slider>().SetValueWithoutNotify(auMan.SFXVolume);

}
