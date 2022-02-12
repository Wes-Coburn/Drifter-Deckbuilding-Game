using UnityEngine;
using UnityEngine.EventSystems;

public class HeroPowerButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool isUltimate;
    public bool IsUltimate { get => isUltimate; }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        if (!PlayerManager.Instance.IsMyTurn || EffectManager.Instance.EffectsResolving ||
            EventManager.Instance.ActionsDelayed) return; // TESTING
        PlayerManager.Instance.UseHeroPower(isUltimate);
        PowerZoom pz = GetComponent<PowerZoom>();
        pz.DestroyPowerPopup();
        pz.DestroyAbilityPopup();
    }
}
