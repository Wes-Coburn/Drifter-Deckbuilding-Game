using UnityEngine;
using UnityEngine.EventSystems;

public class CloseExplicitLanguageButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        UIManager.Instance.DestroyExplicitLanguagePopup();
    }
}
