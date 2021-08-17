using UnityEngine;

public class TurnPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject playerTurnText;
    [SerializeField] private GameObject enemyTurnText;

    public void DisplayTurnPopup(bool isPlayerTurn)
    {
        playerTurnText.SetActive(isPlayerTurn);
        enemyTurnText.SetActive(!isPlayerTurn);
    }
}
