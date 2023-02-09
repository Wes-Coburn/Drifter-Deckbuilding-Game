using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        if (SceneLoader.SceneIsLoading) return;

        Managers.G_MAN.EndGame();
        Managers.U_MAN.DestroyMenuPopup();
    }
}
