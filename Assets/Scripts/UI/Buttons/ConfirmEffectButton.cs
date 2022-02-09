using UnityEngine;
using UnityEngine.EventSystems;

public class ConfirmEffectButton : MonoBehaviour, IPointerClickHandler
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) UserClick();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        UserClick();
    }

    private void UserClick()
    {
        GetComponent<SoundPlayer>().PlaySound(0);
        UIManager.Instance.SetConfirmEffectButton(false);
        EffectManager.Instance.ConfirmTargetEffect();
    }
}
