using UnityEngine;

public class ExplicitLanguagePopupDisplay : MonoBehaviour
{
    private GameManager gMan;
    private void Start() => gMan = GameManager.Instance;
    private void NewGame(bool hideExplicitLanguage)
    {
        gMan.HideExplicitLanguage = hideExplicitLanguage;
        gMan.SavePlayerPreferences();
        gMan.NewGame();
    }
    private void DestroySelf() =>
        UIManager.Instance.DestroyExplicitLanguagePopup();

    public void CloseButton_OnClick() =>
        DestroySelf();

    public void ShowButton_OnClick()
    {
        DestroySelf();
        NewGame(false);
    }
    public void HideButton_OnClick()
    {
        DestroySelf();
        NewGame(true);
    }
}
