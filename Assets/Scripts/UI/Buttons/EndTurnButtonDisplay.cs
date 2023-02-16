using UnityEngine;

public class EndTurnButtonDisplay : MonoBehaviour
{
    [SerializeField] private GameObject playerTurnSide;
    [SerializeField] private GameObject enemyTurnSide;

    public GameObject PlayerTurnSide => playerTurnSide;
    public GameObject EnemyTurnSide => enemyTurnSide;
}
