using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    public void OnClick()
    {
        if (PlayerManager.Instance.IsMyTurn && !UIManager.Instance.PlayerIsTargetting)
        {
            GameManager.Instance.EndTurn(GameManagerData.PLAYER);
        }
    }
}
