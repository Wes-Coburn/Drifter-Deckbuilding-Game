using UnityEngine;
using UnityEngine.EventSystems;

public class WoundedTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject abilityPopupPrefab;
    [SerializeField] private CardAbility woundedAbility;
    [SerializeField] private Vector2 tooltipPosition;

    private const string WOUNDED_TOOLTIP_TIMER = "WoundedTooltipTimer";

    private GameObject woundedTooltip;

    private void CreateWoundedTooltip()
    {
        DestroyWoundedTooltip();
        woundedTooltip = Instantiate(abilityPopupPrefab, UIManager.Instance.CurrentCanvas.transform);
        woundedTooltip.GetComponent<AbilityPopupDisplay>().AbilityScript = woundedAbility;
        woundedTooltip.transform.localPosition = tooltipPosition;
        woundedTooltip.transform.localScale = new Vector2(3, 3);
    }

    private void DestroyWoundedTooltip()
    {
        FunctionTimer.StopTimer(WOUNDED_TOOLTIP_TIMER);

        if (woundedTooltip != null)
        {
            Destroy(woundedTooltip);
            woundedTooltip = null;
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        FunctionTimer.Create(() => CreateWoundedTooltip(), 1, WOUNDED_TOOLTIP_TIMER);
    }

    public void OnPointerExit(PointerEventData pointerEventData) => DestroyWoundedTooltip();
}
