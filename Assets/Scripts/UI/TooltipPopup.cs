using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TooltipPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const string TOOLTIP_TIMER = "TooltipTimer";
    private GameObject tooltipPopup;
    [SerializeField] private GameObject tooltipPopupPrefab;
    [SerializeField] [TextArea] private string tooltipText;
    [SerializeField] private Vector2 tooltipPosition;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (DragDrop.DraggingCard != null || DragDrop.ArrowIsDragging) return;
        FunctionTimer.Create(() => ShowTooltip(), 0.5f, TOOLTIP_TIMER);
        void ShowTooltip()
        {
            DestroyToolTip();
            tooltipPopup = Instantiate(tooltipPopupPrefab, 
                UIManager.Instance.CurrentWorldSpace.transform);
            DisplayTooltipPopup();
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        FunctionTimer.StopTimer(TOOLTIP_TIMER);
        DestroyToolTip();
    }

    private void DisplayTooltipPopup()
    {
        tooltipPopup.transform.localPosition = tooltipPosition;
        tooltipPopup.GetComponentInChildren<TextMeshPro>().SetText(tooltipText);
    }

    private void DestroyToolTip()
    {
        if (tooltipPopup != null)
        {
            Destroy(tooltipPopup);
            tooltipPopup = null;
        }
    }
}
