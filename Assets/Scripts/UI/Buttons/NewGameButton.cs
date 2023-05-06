using UnityEngine;

public class NewGameButton : MonoBehaviour
{
    public void OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;

        //UIManager.Instance.ShakeCamera(EZCameraShake.CameraShakePresets.Bump);
        Managers.AN_MAN.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress);

        if (GameLoader.CheckSave())
        {
            Debug.LogWarning("SAVED GAME WARNING POPUP HERE!");
        }

        int hideLanguagePref = PlayerPrefs.GetInt(GameLoader.HIDE_EXPLICIT_LANGUAGE, 2);
        if (hideLanguagePref == 2) Managers.U_MAN.CreateExplicitLanguagePopup();
        else Managers.U_MAN.CreateTutorialPopup();
    }
}
