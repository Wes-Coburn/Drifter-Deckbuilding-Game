using UnityEngine;
using UnityEngine.EventSystems;

public class EndTurnButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        if (!PlayerManager.Instance.IsMyTurn || EffectManager.Instance.EffectsResolving ||
            EventManager.Instance.ActionsDelayed) return; // TESTING
        GameManager.Instance.EndCombatTurn(GameManager.PLAYER);
        GetComponent<SoundPlayer>().PlaySound(0);
    }
}
