using UnityEngine;
using UnityEngine.EventSystems;

public class CreditsSceneButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        if (SceneLoader.SceneIsLoading) return;
        SceneLoader.LoadScene(SceneLoader.Scene.CreditsScene);
    }
}
