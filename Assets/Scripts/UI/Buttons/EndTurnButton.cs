using UnityEngine;
using UnityEngine.EventSystems;

public class EndTurnButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;

        if (PlayerManager.Instance.IsMyTurn && !UIManager.Instance.PlayerIsTargetting)
        {
            GameManager.Instance.EndTurn(GameManager.PLAYER);
            AudioManager.Instance.StartStopSound("SFX_EndTurn");
        }
    }
}
