using UnityEngine;

public class ExplicitLanguagePopupDisplay : MonoBehaviour
{
    private void SetPrefs(bool hideExplicitLanguage)
    {
        Managers.G_MAN.HideExplicitLanguage = hideExplicitLanguage;
        GameLoader.SavePlayerPreferences();
        Managers.U_MAN.CreateTutorialPopup();
    }
    private void DestroySelf() =>
        Managers.U_MAN.DestroyInteractablePopup(gameObject);

    public void CloseButton_OnClick() => DestroySelf();

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
