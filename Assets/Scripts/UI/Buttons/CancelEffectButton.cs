using UnityEngine;
using UnityEngine.EventSystems;

public class CancelEffectButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        GetComponent<SoundPlayer>().PlaySound(0);
        UIManager.Instance.SetCancelEffectButton(false);
        EffectManager.Instance.AbortEffectGroupList(true);
    }
}
