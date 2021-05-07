using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    public void OnClick()
    {
        if (PlayerManager.Instance.IsMyTurn) GameManager.Instance.EndTurn(GameManagerData.PLAYER);
    }
}
