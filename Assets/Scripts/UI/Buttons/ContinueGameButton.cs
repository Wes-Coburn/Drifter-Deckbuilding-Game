using UnityEngine;
using UnityEngine.UI;
using System;

public class ContinueGameButton : MonoBehaviour
{
    private GameManager gMan;
    private void Start()
    {
        gMan = GameManager.Instance;
        bool savedGame = gMan.CheckSave();
        GetComponent<Button>().interactable = savedGame;
    }

    public void OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        //UIManager.Instance.ShakeCamera(EZCameraShake.CameraShakePresets.Bump); // TESTING
        AnimationManager.Instance.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress); // TESTING

        SceneLoader.LoadAction += LoadGame;
        SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
    }

    private void LoadGame()
    {
        try { GameManager.Instance.LoadGame(); }
        catch (NullReferenceException)
        {
            UIManager.Instance.CreateFleetingInfoPopup
                ("Your save file is incompatible with this version!\nPlease start a new game.");
            return;
        }
    }
}
