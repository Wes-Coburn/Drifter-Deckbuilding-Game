using System;
using UnityEngine;
using UnityEngine.UI;

public class ContinueGameButton : MonoBehaviour
{
    public void OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        //UIManager.Instance.ShakeCamera(EZCameraShake.CameraShakePresets.Bump); // TESTING
        Managers.AN_MAN.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress);

        var data = SaveLoad.LoadGame(SaveLoad.SaveType.Player) as PlayerData;
        string saveScene = data.SaveScene;
        
        if (!Enum.TryParse(saveScene, out SceneLoader.Scene loadScene))
        {
            Debug.LogError("INVALID SAVE SCENE!");
            return;
        }

        SceneLoader.LoadAction_Async += GameLoader.LoadGame_PlayerData_Async;
        SceneLoader.LoadScene(loadScene);
    }
}
