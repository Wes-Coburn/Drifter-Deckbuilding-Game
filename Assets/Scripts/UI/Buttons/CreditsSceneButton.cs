using UnityEngine;
using UnityEngine.EventSystems;

public class CreditsSceneButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        if (SceneLoader.SceneIsLoading) return;

        //UIManager.Instance.ShakeCamera(EZCameraShake.CameraShakePresets.Bump); // TESTING
        SceneLoader.LoadScene(SceneLoader.Scene.CreditsScene);
    }
}
