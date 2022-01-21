using UnityEngine;

public class ExplicitLanguagePopupDisplay : MonoBehaviour
{
    private void NewGame(bool hideExplicitLanguage) =>
        GameManager.Instance.NewGame(hideExplicitLanguage);
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
