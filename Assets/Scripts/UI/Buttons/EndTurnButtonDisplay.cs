using UnityEngine;

public class EndTurnButtonDisplay : MonoBehaviour
{
    [SerializeField] private GameObject playerTurnSide, enemyTurnSide;

    public GameObject PlayerTurnSide => playerTurnSide;
    public GameObject EnemyTurnSide => enemyTurnSide;
}
