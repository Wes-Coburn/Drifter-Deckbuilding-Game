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
    private const int ACTIONS_PER_TURN = GameManagerData.ACTIONS_PER_TURN;
    private const int MAXIMUM_ACTIONS = GameManagerData.MAXIMUM_ACTIONS;
    private const string PLAYER = GameManagerData.PLAYER;
    private const string ENEMY = GameManagerData.ENEMY;

    /* MANAGERS */
    private PlayerManager playerManager;
    private EnemyManager enemyManager;
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
            UIManager.UpdatePlayerHealth(PlayerReputation);
        }
    }
    private int enemyReputation;
    public int EnemyReputation
    {
        get => enemyReputation;
        set
        {
            enemyReputation = value;
            UIManager.UpdateEnemyHealth(EnemyReputation);
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
            if (playerActionsLeft > MAXIMUM_ACTIONS) playerActionsLeft = MAXIMUM_ACTIONS;
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
            if (enemyActionsLeft > MAXIMUM_ACTIONS) enemyActionsLeft = MAXIMUM_ACTIONS;
            UIManager.UpdateEnemyActionsLeft(EnemyActionsLeft);
        }
    }

    /******
     * *****
     * ****** START
     * *****
     *****/
    private void Start()
    {
        playerManager = PlayerManager.Instance;
        enemyManager = EnemyManager.Instance;
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
        UIManager.Instance.UpdatePlayerHealth(10); // Unnecessary
        UIManager.Instance.UpdateEnemyHealth(10); // Unnecessary

        cardManager.DrawHand(PLAYER);
        cardManager.DrawHand(ENEMY);
        StartTurn(PLAYER);
    }
    public void EndGame()
    {
        // game end animation
    }
    
    
    /******
     * *****
     * ****** START/END_TURN
     * *****
     *****/
    private void StartTurn(string player)
    {
        if (player == PLAYER)
        {
            playerManager.IsMyTurn = true;
            enemyManager.IsMyTurn = false;
            PlayerActionsLeft += ACTIONS_PER_TURN;
        }
        else if (player == ENEMY)
        {
            playerManager.IsMyTurn = false;
            enemyManager.IsMyTurn = true;
            EnemyActionsLeft += ACTIONS_PER_TURN;
        }

        cardManager.RefreshCards(player);
        UIManager.UpdateEndTurnButton(playerManager.IsMyTurn);
        cardManager.DrawCard(player);
    }
    public void EndTurn(string player)
    {
        // end of turn effects
        string otherPlayer = null;
        if (player == PLAYER)
        {
            otherPlayer = ENEMY;
        }
        else if (player == ENEMY)
        {
            otherPlayer = PLAYER;
        }
        StartTurn(otherPlayer);
    }
}
