using UnityEngine;

public class ExplicitLanguagePopupDisplay : MonoBehaviour
{
    private void SetPrefs(bool hideExplicitLanguage)
    {
        ManagerHandler.G_MAN.HideExplicitLanguage = hideExplicitLanguage;
        ManagerHandler.G_MAN.SavePlayerPreferences();
        ManagerHandler.U_MAN.CreateTutorialPopup();
    }
    private void DestroySelf() =>
        ManagerHandler.U_MAN.DestroyInteractablePopup(gameObject);

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
