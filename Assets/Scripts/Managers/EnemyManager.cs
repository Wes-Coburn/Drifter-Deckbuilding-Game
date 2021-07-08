using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static EnemyManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        EnemyDeck = new List<int>();
        while (EnemyDeck.Count < 30) EnemyDeck.Add(2); // FOR TESTING ONLY
    }

    /* PLAYER_HERO */
    public Hero EnemyHero { get; private set; } // UNUSED/TESTING

    /* ENEMY_DECK */
    public List<int> EnemyDeck { get; private set; }
    public List<CardDisplay> EnemyDeck2 { get; private set; }

    /* IS_MY_TURN */
    public bool IsMyTurn { get; set; }

    /* HEALTH */
    private int enemyHealth;
    public int EnemyHealth
    {
        get => enemyHealth;
        set
        {
            enemyHealth = value;
            UIManager.Instance.UpdateEnemyHealth(EnemyHealth);
        }
    }
    
    /* ACTIONS_LEFT */
    private int enemyActionsLeft;
    public int EnemyActionsLeft
    {
        get => enemyActionsLeft;
        set
        {
            enemyActionsLeft = value;
            if (enemyActionsLeft > GameManager.MAXIMUM_ACTIONS) enemyActionsLeft = GameManager.MAXIMUM_ACTIONS;
            UIManager.Instance.UpdateEnemyActionsLeft(EnemyActionsLeft);
        }
    }
}
