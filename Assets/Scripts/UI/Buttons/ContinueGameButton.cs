using UnityEngine;
using UnityEngine.UI;
using System;

public class ContinueGameButton : MonoBehaviour
{
    private void Start()
    {
        bool savedGame = GameManager.Instance.CheckSave(); // TESTING
        GetComponent<Button>().interactable = savedGame;
    }

    public void OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        //UIManager.Instance.ShakeCamera(EZCameraShake.CameraShakePresets.Bump); // TESTING

        SceneLoader.LoadAction += LoadGame;
        SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
    }

    private void LoadGame()
    {
        try { GameManager.Instance.LoadGame(); }
        catch (NullReferenceException)
        {
            UIManager.Instance.CreateFleetingInfoPopup
                ("Your save file is incompatible with this version!\nPlease start a new game.", true);
            return;
        }
    }
}
