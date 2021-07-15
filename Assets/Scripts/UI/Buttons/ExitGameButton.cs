using UnityEngine;
using UnityEngine.EventSystems;

public class ExitGameButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        Application.Quit();
    }
}
