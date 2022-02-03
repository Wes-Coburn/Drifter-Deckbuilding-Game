using UnityEngine;
using UnityEngine.EventSystems;

public class EndTurnButton : MonoBehaviour, IPointerClickHandler
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
        if (!PlayerManager.Instance.IsMyTurn || EffectManager.Instance.EffectsResolving ||
            EventManager.Instance.ActionsDelayed) return;
        GameManager.Instance.EndCombatTurn(GameManager.PLAYER);
        GetComponentInParent<SoundPlayer>().PlaySound(0);
    }


}
