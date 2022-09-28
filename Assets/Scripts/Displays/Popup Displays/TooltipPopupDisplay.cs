using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TooltipPopupDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] [TextArea] private string tooltipText;
    [SerializeField] private Vector2 tooltipPosition;
    [SerializeField] private bool isZoomCardTooltip;

    private UIManager uMan;

    private void Start() => uMan = UIManager.Instance;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (isZoomCardTooltip && !CardZoom.ZoomCardIsCentered) return; // TESTING
        if (DragDrop.DraggingCard != null || DragDrop.ArrowIsDragging) return;
        FunctionTimer.Create(() =>
        uMan.CreateTooltipPopup(tooltipPosition, tooltipText), 0.5f, UIManager.TOOLTIP_TIMER);
    }

    public void OnPointerExit(PointerEventData pointerEventData) => uMan.DestroyTooltipPopup();
}
