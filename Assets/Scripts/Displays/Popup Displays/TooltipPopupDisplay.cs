using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipPopupDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField][TextArea] private string tooltipText;
    [SerializeField] private Vector2 tooltipPosition;
    [SerializeField] private bool isZoomCardTooltip;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (isZoomCardTooltip && !CardZoom.ZoomCardIsCentered) return;
        if (DragDrop.DraggingCard != null || DragDrop.ArrowIsDragging) return;
        FunctionTimer.Create(() =>
        Managers.U_MAN.CreateTooltipPopup(tooltipPosition, tooltipText), 0.5f, UIManager.TOOLTIP_TIMER);
    }

    public void OnPointerExit(PointerEventData pointerEventData) => Managers.U_MAN.DestroyTooltipPopup();
}
