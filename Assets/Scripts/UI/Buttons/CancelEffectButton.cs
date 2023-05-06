using UnityEngine;
using UnityEngine.EventSystems;

public class CancelEffectButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        GetComponent<SoundPlayer>().PlaySound(0);
        Managers.U_MAN.SetCancelEffectButton(false);
        Managers.EF_MAN.CancelEffectGroupList(true);
    }
}
