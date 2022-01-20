using UnityEngine;
using UnityEngine.EventSystems;

public class HeroPowerButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        if (!PlayerManager.Instance.IsMyTurn || EffectManager.Instance.EffectsResolving) return; // TESTING
        PlayerManager.Instance.UseHeroPower();
        PowerZoom pz = GetComponent<PowerZoom>();
        pz.DestroyPowerPopup();
        pz.DestroyAbilityPopup();
    }
}
