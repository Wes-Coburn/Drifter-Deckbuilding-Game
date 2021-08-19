using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPopupButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        UIManager.Instance.CreateMenuPopup();
    }
}
