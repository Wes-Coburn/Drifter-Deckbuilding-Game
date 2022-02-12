using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject musicSlider;
    [SerializeField] private GameObject sfxSlider;
    [SerializeField] private GameObject explicitLanguageToggle;

    private AudioManager auMan;
    private GameManager gMan;
    private void Start()
    {
        auMan = AudioManager.Instance;
        gMan = GameManager.Instance;
        SetMusicSlider();
        SetSFXSlider();
        SetExplicitLanguageToggle();
    }
    private void SetSFXSlider() => sfxSlider.GetComponent<Slider>().SetValueWithoutNotify(auMan.SFXVolume);
    private void SetMusicSlider() => musicSlider.GetComponent<Slider>().SetValueWithoutNotify(auMan.MusicVolume);
    private void SetExplicitLanguageToggle() =>
        explicitLanguageToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!gMan.HideExplicitLanguage);
    public void ExplicitLanguage_OnToggle(bool showLanguage)
    {
        if (gMan == null) return;
        gMan.HideExplicitLanguage = !showLanguage;
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
}
