using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject musicSlider;
    [SerializeField] private GameObject sfxSlider;
    [SerializeField] private GameObject explicitLanguageToggle;
    [SerializeField] private GameObject gameInfoPopup;
    [SerializeField] private GameObject gameInfoButton;

    private AudioManager auMan;
    private GameManager gMan;

    private void Start()
    {
        auMan = AudioManager.Instance;
        gMan = GameManager.Instance;
        
        SetMusicSlider();
        SetSFXSlider();
        SetExplicitLanguageToggle();
        gameInfoPopup.SetActive(false);
    }
    private void SetSFXSlider() => sfxSlider.GetComponent<Slider>().SetValueWithoutNotify(auMan.SFXVolume);
    private void SetMusicSlider() => musicSlider.GetComponent<Slider>().SetValueWithoutNotify(auMan.MusicVolume);
    private void SetExplicitLanguageToggle() =>
        explicitLanguageToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!gMan.HideExplicitLanguage);
    public void ExplicitLanguage_OnToggle(bool showLanguage)
    {
        if (gMan == null)
        {
            Debug.LogWarning("GAME_MANAGER IS NULL!");
            return;
        }
        gMan.HideExplicitLanguage = !showLanguage;
    }
    public void MusicVolume_OnSlide(float volume)
    {
        if (auMan == null)
        {
            Debug.LogWarning("AUDIO_MANAGER IS NULL!");
            return;
        }
        auMan.MusicVolume = volume;
    }
    public void MusicVolume_OnPlus()
    {
        auMan.MusicVolume++;
        SetMusicSlider();
    }
    public void MusicVolume_OnMinus()
    {
        auMan.MusicVolume--;
        SetMusicSlider();
    }
    public void SFXVolume_OnSlide(float volume)
    {
        if (auMan == null) return;
        auMan.SFXVolume = volume;
    }
    public void SFXVolume_OnPlus()
    {
        auMan.SFXVolume++;
        SetSFXSlider();
    }
    public void SFXVolume_OnMinus()
    {
        auMan.SFXVolume--;
        SetSFXSlider();
    }
    public void GameInfoButton_OnClick()
    {
        gameInfoPopup.SetActive(!gameInfoPopup.activeSelf);
        TextMeshProUGUI tmpro = gameInfoButton.GetComponentInChildren<TextMeshProUGUI>();
        string text = gameInfoPopup.activeSelf ? "Back" : "How to Play";
        tmpro.SetText(text);
    }
}
