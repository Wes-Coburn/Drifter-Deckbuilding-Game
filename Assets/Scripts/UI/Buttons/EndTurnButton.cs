using UnityEngine;
using UnityEngine.EventSystems;

public class EndTurnButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        if (!PlayerManager.Instance.IsMyTurn || EffectManager.Instance.EffectsResolving) return; // TESTING
        GameManager.Instance.EndTurn(GameManager.PLAYER);
        GetComponent<SoundPlayer>().PlaySound(0);
    }
}
