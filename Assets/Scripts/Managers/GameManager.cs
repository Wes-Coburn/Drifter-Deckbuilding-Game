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
        FunctionTimer.Create(() => CardManager.Instance.DrawHand(GameManager.ENEMY), 1f);
        FunctionTimer.Create(() => StartTurn(PLAYER), 3f);
    }
    public void EndGame(bool playerWins)
    {
        // VICTORY or DEFEAT animation
        if (playerWins) Debug.LogWarning("PLAYER WINS!");
        else Debug.LogWarning("ENEMY WINS!");
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

            // Timed Actions
            FunctionTimer.Create(() => cardManager.DrawCard(activePlayer), 1f);
            FunctionTimer.Create(() => cardManager.PlayCard(null, ENEMY), 2f);

            // ENEMY ATTACK
            float delay = 4f;
            
            void EnemyAttack(GameObject enemyHero)
            {
                if (cardManager.PlayerZoneCards.Count > 0)
                {
                    cardManager.Attack(enemyHero, cardManager.PlayerZoneCards[0]);
                }
                else cardManager.Attack(enemyHero, cardManager.PlayerChampion);
            }
            void EndTurnDelay(float delay) => FunctionTimer.Create(() => EndTurn(ENEMY), delay);

            foreach (GameObject enemyHero in cardManager.EnemyZoneCards)
            {
                if (cardManager.CanAttack(enemyHero))
                {
                    FunctionTimer.Create(() => EnemyAttack(enemyHero), delay);
                    delay += 2f;
                }
            }
            EndTurnDelay(delay);
        }
    }

    public void EndTurn(string player)
    {
        CardManager.Instance.RemoveTemporaryStats(player);
        CardManager.Instance.RemoveTemporaryAbilities(player);
        StartTurn(PLAYER);
    }
}
