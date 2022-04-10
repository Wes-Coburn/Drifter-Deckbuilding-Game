using UnityEngine;

public class ExplicitLanguagePopupDisplay : MonoBehaviour
{
    private GameManager gMan;
    private UIManager uMan;
    private void Start()
    {
        gMan = GameManager.Instance;
        uMan = UIManager.Instance;
    }
    private void SetPrefs(bool hideExplicitLanguage)
    {
        gMan.HideExplicitLanguage = hideExplicitLanguage;
        gMan.SavePlayerPreferences();
        uMan.CreateTutorialPopup(); // TESTING
    }
    private void DestroySelf() =>
        uMan.DestroyExplicitLanguagePopup();

    public void CloseButton_OnClick() =>
        DestroySelf();

    public void ShowButton_OnClick()
    {
        DestroySelf();
        SetPrefs(false);
    }
    public void HideButton_OnClick()
    {
        DestroySelf();
        SetPrefs(true);
    }
}
