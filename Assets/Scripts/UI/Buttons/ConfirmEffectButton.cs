using UnityEngine;
using UnityEngine.EventSystems;

public class ConfirmEffectButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        GetComponent<SoundPlayer>().PlaySound(0);
        UIManager.Instance.SetConfirmEffectButton(false);
        EffectManager.Instance.ConfirmTargetEffect(); // TESTING
    }
}
