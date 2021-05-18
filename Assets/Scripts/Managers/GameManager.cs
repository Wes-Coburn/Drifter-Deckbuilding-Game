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
    private int playerHealth;
    public int PlayerHealth
    {
        get => playerHealth;
        set
        {
            playerHealth = value;
            UIManager.UpdatePlayerHealth(PlayerHealth);
        }
    }
    private int enemyHealth;
    public int EnemyHealth
    {
        get => enemyHealth;
        set
        {
            enemyHealth = value;
            UIManager.UpdateEnemyHealth(EnemyHealth);
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
    }

    /******
     * *****
     * ****** START/END_GAME
     * *****
     *****/

    public void StartGame()
    {
        PlayerActionsLeft = STARTING_ACTIONS;
        EnemyActionsLeft = STARTING_ACTIONS;
        EnemyHealth = 0;
        PlayerHealth = 0;

        cardManager.DrawHand(PLAYER);
        StartTurn(PLAYER);
    }
    public void EndGame()
    {
        // VICTORY or DEFEAT animation
    }

    /******
     * *****
     * ****** START/END_TURN
     * *****
     *****/
    private void StartTurn(string activePlayer)
    {
        if (activePlayer == PLAYER)
        {
            playerManager.IsMyTurn = true;
            enemyManager.IsMyTurn = false;
            UIManager.UpdateEndTurnButton(playerManager.IsMyTurn);
            PlayerActionsLeft += ACTIONS_PER_TURN;
            cardManager.DrawCard(activePlayer);
            cardManager.RefreshCards(activePlayer);
        }
        else if (activePlayer == ENEMY)
        {
            playerManager.IsMyTurn = false;
            enemyManager.IsMyTurn = true;
            UIManager.UpdateEndTurnButton(playerManager.IsMyTurn);
            EnemyActionsLeft += ACTIONS_PER_TURN;
            cardManager.DrawCard(activePlayer);
            cardManager.RefreshCards(activePlayer);

            FunctionTimer.Create(() => CardManager.Instance.PlayCard(null, ENEMY), 1.5f);
            FunctionTimer.Create(() => EndTurn(ENEMY), 4f);
        }
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
