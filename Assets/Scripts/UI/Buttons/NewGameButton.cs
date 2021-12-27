using UnityEngine;

public class NewGameButton : MonoBehaviour
{
    public void OnClick()
    {
        if (SceneLoader.SceneIsLoading) return; // TESTING

        if (GameManager.Instance.LoadGame(true))
        {
            Debug.LogWarning("SAVED GAME WARNING POPUP HERE!");
        }
        UIManager.Instance.CreateExplicitLanguagePopup();
    }
}
