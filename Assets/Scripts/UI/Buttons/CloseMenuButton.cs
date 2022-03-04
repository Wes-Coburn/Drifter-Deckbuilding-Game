using UnityEngine;
using UnityEngine.EventSystems;

public class CloseMenuButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        UIManager.Instance.DestroyMenuPopup();
        GameManager.Instance.SavePlayerPreferences();
    }
}
