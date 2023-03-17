using UnityEngine;
using UnityEngine.EventSystems;

public class HeroPowerButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool isUltimate;
    public bool IsUltimate { get => isUltimate; }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        if (!Managers.P_MAN.IsMyTurn || Managers.EF_MAN.EffectsResolving || Managers.EV_MAN.ActionsDelayed) return;

        Managers.P_MAN.UseHeroPower(isUltimate);
        var pz = GetComponent<PowerZoom>();
        pz.DestroyPowerPopup();
        pz.DestroyAbilityPopup();
    }
}
