using UnityEngine;

public class ExplicitLanguageButton : MonoBehaviour
{
    public void ShowExplicit()
    {
        DestroySelf();
        NewGame(false);
    }
    public void HideExplicit()
    {
        DestroySelf();
        NewGame(true);
    }

    private void NewGame(bool hideExplicitLanguage)
    {
        GameManager.Instance.NewGame(hideExplicitLanguage);
    }
    private void DestroySelf()
    {
        UIManager.Instance.DestroyExplicitLanguagePopup();
    }
}
