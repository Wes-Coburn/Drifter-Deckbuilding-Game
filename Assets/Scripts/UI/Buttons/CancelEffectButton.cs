using UnityEngine;
using UnityEngine.EventSystems;

public class CancelEffectButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        UIManager.Instance.SetCancelEffectButton(false);
        EffectManager.Instance.AbortEffectGroup(true);
    }
}
