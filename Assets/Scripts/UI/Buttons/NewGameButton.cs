using UnityEngine;

public class NewGameButton : MonoBehaviour
{
    private GameManager gMan;
    private void Start() => gMan = GameManager.Instance;
    public void OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;

        //UIManager.Instance.ShakeCamera(EZCameraShake.CameraShakePresets.Bump); // TESTING
        if (gMan.CheckSave())
        {
            Debug.LogWarning("SAVED GAME WARNING POPUP HERE!");
        }
        int hideLanguagePref = PlayerPrefs.GetInt(GameManager.HIDE_EXPLICIT_LANGUAGE, 2);        
        if (hideLanguagePref == 2) UIManager.Instance.CreateExplicitLanguagePopup();
        else gMan.NewGame();
    }
}
