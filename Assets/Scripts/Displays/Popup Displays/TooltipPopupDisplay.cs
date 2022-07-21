using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TooltipPopupDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const string TOOLTIP_TIMER = "TooltipTimer";
    private GameObject tooltipPopup;
    private UIManager uMan;

    [SerializeField] [TextArea] private string tooltipText;
    [SerializeField] private Vector2 tooltipPosition;
    [SerializeField] private bool isZoomCardTooltip;

    private void Start() => uMan = UIManager.Instance;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (isZoomCardTooltip && !CardZoom.ZoomCardIsCentered) return; // TESTING
        if (DragDrop.DraggingCard != null || DragDrop.ArrowIsDragging) return;
        FunctionTimer.Create(() => ShowTooltip(), 0.5f, TOOLTIP_TIMER);
        void ShowTooltip()
        {
            DestroyToolTip();
            tooltipPopup = Instantiate(uMan.TooltipPopupPrefab,
                uMan.CurrentWorldSpace.transform);
            DisplayTooltipPopup();
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData) => DestroyToolTip();

    private void DisplayTooltipPopup()
    {
        tooltipPopup.transform.localPosition = tooltipPosition;
        tooltipPopup.GetComponentInChildren<TextMeshPro>().SetText(tooltipText);
    }

    public void DestroyToolTip()
    {
        FunctionTimer.StopTimer(TOOLTIP_TIMER);

        if (tooltipPopup != null)
        {
            Destroy(tooltipPopup);
            tooltipPopup = null;
        }
    }
}
