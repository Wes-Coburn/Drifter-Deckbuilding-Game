using UnityEngine;

public class GameManager : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    /* GAME_MANAGER_DATA */
    private const int STARTING_ACTIONS = GameManagerData.STARTING_ACTIONS;
    private const string PLAYER = GameManagerData.PLAYER;
    private const string ENEMY = GameManagerData.ENEMY;

    /* MANAGERS */
    private PlayerManager playerManager;
    private CardManager cardManager;
    private UIManager UIManager;
    
    /* REPUTATION */
    private int playerReputation;
    public int PlayerReputation
    {
        get => playerReputation;
        set
        {
            playerReputation = value;
            UIManager.UpdatePlayerReputation(PlayerReputation);
        }
    }
    private int enemyReputation;
    public int EnemyReputation
    {
        get => enemyReputation;
        set
        {
            enemyReputation = value;
            UIManager.UpdateEnemyReputation(EnemyReputation);
        }
    }

    /* ACTIONS_LEFT */
    private int playerActionsLeft;
    public int PlayerActionsLeft
    {
        get => playerActionsLeft;
        set
        {
            playerActionsLeft = value;
            UIManager.UpdatePlayerActionsLeft(PlayerActionsLeft);
        }
    }
    private int enemyActionsLeft;
    public int EnemyActionsLeft
    {
        get => enemyActionsLeft;
        set
        {
            enemyActionsLeft = value;
            UIManager.UpdateEnemyActionsLeft(EnemyActionsLeft);
        }
    }

    /******
     * *****
     * ****** START
     * *****
     *****/
    void Start()
    {
        playerManager = PlayerManager.Instance;
        UIManager = UIManager.Instance;
        cardManager = CardManager.Instance;

        PlayerActionsLeft = STARTING_ACTIONS;
        EnemyActionsLeft = STARTING_ACTIONS;
        EnemyReputation = 0;
        PlayerReputation = 0;

        StartGame(); // TESTING
    }

    public void UpdateActionsLeft () // REMOVE THIS EVENTUALLY
    {
        PlayerActionsLeft = playerActionsLeft;
        EnemyActionsLeft = enemyActionsLeft;
    }

    /******
     * *****
     * ****** START/END_GAME
     * *****
     *****/

    public void StartGame()
    {
        cardManager.DrawHand();
    }
    public void EndGame()
    {
        // blank
    }
    
    
    /******
     * *****
     * ****** START/END_TURN
     * *****
     *****/
    public void NextTurn()
    {
        playerManager.IsMyTurn = !playerManager.IsMyTurn;
        UIManager.UpdateEndTurnButton(playerManager.IsMyTurn);
        if (playerManager.IsMyTurn) StartTurn();
        else EndTurn();
    }
    private void StartTurn()
    {
        Debug.Log("[START_TURN() in GameManager]!!!");
        PlayerActionsLeft = STARTING_ACTIONS;
        cardManager.RefreshCards(PLAYER);
    }
    public void EndTurn()
    {
        Debug.Log("[END_TURN() in GameManager!!!");
        EnemyActionsLeft = STARTING_ACTIONS;
        cardManager.RefreshCards(ENEMY);
    }
}
