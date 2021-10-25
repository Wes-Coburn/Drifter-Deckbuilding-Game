using UnityEngine;
using UnityEngine.EventSystems;

public class HeroPowerButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        if (!PlayerManager.Instance.IsMyTurn || EffectManager.Instance.EffectsResolving) return; // TESTING
        PlayerManager.Instance.UseHeroPower();
        FunctionTimer.StopTimer(PowerZoom.ABILITY_POPUP_TIMER);
        FunctionTimer.StopTimer(PowerZoom.POWER_POPUP_TIMER);
        GetComponent<PowerZoom>().DestroyPowerPopup();
    }
}
