using System;
using UnityEngine;
using UnityEngine.UI;

public class ContinueGameButton : MonoBehaviour
{
    private void Start() => GetComponent<Button>().interactable = GameLoader.CheckSave();

    public void OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        //UIManager.Instance.ShakeCamera(EZCameraShake.CameraShakePresets.Bump); // TESTING
        Managers.AN_MAN.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress);

        SceneLoader.LoadAction += LoadGame;
        SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
    }

    private void LoadGame()
    {
        try { GameLoader.LoadGame(); }
        catch (NullReferenceException)
        {
            Managers.U_MAN.CreateFleetingInfoPopup
                ("Your save file is incompatible with this version!\nPlease start a new game.");
            return;
        }
    }
}
