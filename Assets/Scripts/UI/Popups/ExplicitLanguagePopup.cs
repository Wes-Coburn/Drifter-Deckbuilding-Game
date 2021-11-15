using UnityEngine;

public class ExplicitLanguagePopup : MonoBehaviour
{
    private void NewGame(bool hideExplicitLanguage) =>
        GameManager.Instance.NewGame(hideExplicitLanguage);
    private void DestroySelf() =>
        UIManager.Instance.DestroyExplicitLanguagePopup();

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
