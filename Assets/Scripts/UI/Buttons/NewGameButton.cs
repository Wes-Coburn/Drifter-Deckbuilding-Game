using UnityEngine;

public class NewGameButton : MonoBehaviour
{
    public void OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;

        //UIManager.Instance.ShakeCamera(EZCameraShake.CameraShakePresets.Bump); // TESTING
        AnimationManager.Instance.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress); // TESTING

        if (Managers.G_MAN.CheckSave())
        {
            Debug.LogWarning("SAVED GAME WARNING POPUP HERE!");
        }
        int hideLanguagePref = PlayerPrefs.GetInt(GameManager.HIDE_EXPLICIT_LANGUAGE, 2);
        if (hideLanguagePref == 2) Managers.U_MAN.CreateExplicitLanguagePopup();
        else Managers.U_MAN.CreateTutorialPopup(); // TESTING
    }
}
