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
    public const int STARTING_HEALTH = 10;
    public const int STARTING_ACTIONS = 0;
    public const int STARTING_HAND_SIZE = 3;
    public const int ACTIONS_PER_TURN = 2;
    public const int MAXIMUM_ACTIONS = 6;
    public const string PLAYER = "Player";
    public const string ENEMY = "Enemy";

    /* MANAGERS */
    private PlayerManager playerManager;
    private EnemyManager enemyManager;
    private CardManager cardManager;
    private UIManager UIManager;

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
        PlayerManager.Instance.PlayerHealth = STARTING_HEALTH;
        EnemyManager.Instance.EnemyHealth = STARTING_HEALTH;
        PlayerManager.Instance.PlayerActionsLeft = STARTING_ACTIONS;
        EnemyManager.Instance.EnemyActionsLeft = STARTING_ACTIONS;

        FunctionTimer.Create(() => CardManager.Instance.DrawHand(GameManager.PLAYER), 1f);
        FunctionTimer.Create(() => CardManager.Instance.DrawHand(GameManager.ENEMY), 1f); // FOR TESTING ONLY
        FunctionTimer.Create(() => StartTurn(PLAYER), 3f);
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
            PlayerManager.Instance.PlayerActionsLeft += ACTIONS_PER_TURN;
            cardManager.RefreshCards(activePlayer);
            FunctionTimer.Create(() => cardManager.DrawCard(activePlayer), 1f);
        }
        else if (activePlayer == ENEMY)
        {
            playerManager.IsMyTurn = false;
            enemyManager.IsMyTurn = true;
            UIManager.UpdateEndTurnButton(playerManager.IsMyTurn);
            EnemyManager.Instance.EnemyActionsLeft += ACTIONS_PER_TURN;
            cardManager.RefreshCards(activePlayer);
            FunctionTimer.Create(() => cardManager.DrawCard(activePlayer), 1f);
            FunctionTimer.Create(() => CardManager.Instance.PlayCard(null, ENEMY), 2f);
            FunctionTimer.Create(() => EndTurn(ENEMY), 4f);
        }
    }

    public void EndTurn(string player)
    {
        // end of turn effects
        if (player == PLAYER) StartTurn(ENEMY);
        else if (player == ENEMY) StartTurn(PLAYER);
    }
}
