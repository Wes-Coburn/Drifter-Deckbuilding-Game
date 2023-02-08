using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject musicSlider;
    [SerializeField] private GameObject sfxSlider;
    [SerializeField] private GameObject explicitLanguageToggle;
    [SerializeField] private GameObject gameInfoPopup;
    [SerializeField] private GameObject gameInfoButton;

    private void Start()
    {
        SetMusicSlider();
        SetSFXSlider();
        SetExplicitLanguageToggle();
        gameInfoPopup.SetActive(false);
    }
    private void SetSFXSlider() => sfxSlider.GetComponent<Slider>().SetValueWithoutNotify(ManagerHandler.AU_MAN.SFXVolume);
    private void SetMusicSlider() => musicSlider.GetComponent<Slider>().SetValueWithoutNotify(ManagerHandler.AU_MAN.MusicVolume);
    private void SetExplicitLanguageToggle() =>
        explicitLanguageToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!ManagerHandler.G_MAN.HideExplicitLanguage);
    public void ExplicitLanguage_OnToggle(bool showLanguage)
    {
        if (ManagerHandler.G_MAN == null)
        {
            Debug.LogWarning("GAME_MANAGER IS NULL!");
            return;
        }
        ManagerHandler.G_MAN.HideExplicitLanguage = !showLanguage;
    }
    public void MusicVolume_OnSlide(float volume)
    {
        if (ManagerHandler.AU_MAN == null)
        {
            Debug.LogWarning("AUDIO_MANAGER IS NULL!");
            return;
        }
        ManagerHandler.AU_MAN.MusicVolume = volume;
    }
    public void MusicVolume_OnPlus()
    {
        ManagerHandler.AU_MAN.MusicVolume++;
        SetMusicSlider();
    }
    public void MusicVolume_OnMinus()
    {
        ManagerHandler.AU_MAN.MusicVolume--;
        SetMusicSlider();
    }
    public void SFXVolume_OnSlide(float volume)
    {
        if (ManagerHandler.AU_MAN == null) return;
        ManagerHandler.AU_MAN.SFXVolume = volume;
    }
    public void SFXVolume_OnPlus()
    {
        ManagerHandler.AU_MAN.SFXVolume++;
        SetSFXSlider();
    }
    public void SFXVolume_OnMinus()
    {
        ManagerHandler.AU_MAN.SFXVolume--;
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
