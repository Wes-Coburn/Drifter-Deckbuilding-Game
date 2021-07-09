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

    /* TEST_HEROES */
    [SerializeField] private Hero playerTestHero; // FOR TESTING ONLY
    [SerializeField] private Hero enemyTestHero; // FOR TESTING ONLY

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

        //PlayerManager.Instance.PlayerHero = playerTestHero; // TESTING
    }

    /******
     * *****
     * ****** NEW_GAME
     * *****
     *****/
    public void NewGame()
    {
        StartCombat(/*enemyTestHero*/);
    }

    /******
     * *****
     * ****** START/END_COMBAT
     * *****
     *****/
    private void StartCombat(/*Hero enemyHero*/)
    {
        PlayerManager pm = PlayerManager.Instance;
        EnemyManager em = EnemyManager.Instance;

        //em.EnemyHero = enemyHero; // TESTING

        pm.PlayerHealth = STARTING_HEALTH;
        pm.PlayerActionsLeft = STARTING_ACTIONS;
        em.EnemyHealth = STARTING_HEALTH;
        em.EnemyActionsLeft = STARTING_ACTIONS;

        FunctionTimer.Create(() => CardManager.Instance.DrawHand(PLAYER), 1f);
        FunctionTimer.Create(() => CardManager.Instance.DrawHand(ENEMY), 1f);
        FunctionTimer.Create(() => StartTurn(PLAYER), 3f);
    }
    public void EndCombat(bool playerWins)
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
    private void StartTurn(string player)
    {
        Debug.LogWarning("StartTurn(" + player + ")");
        if (player == PLAYER)
        {
            playerManager.IsMyTurn = true;
            enemyManager.IsMyTurn = false;
            UIManager.UpdateEndTurnButton(playerManager.IsMyTurn);
            PlayerManager.Instance.PlayerActionsLeft += ACTIONS_PER_TURN;
            cardManager.RefreshCards(player);
            FunctionTimer.Create(() => cardManager.DrawCard(player), 1f);
        }
        else if (player == ENEMY)
        {
            playerManager.IsMyTurn = false;
            enemyManager.IsMyTurn = true;
            UIManager.UpdateEndTurnButton(playerManager.IsMyTurn);
            EnemyManager.Instance.EnemyActionsLeft += ACTIONS_PER_TURN;
            cardManager.RefreshCards(player);

            // Timed Actions
            FunctionTimer.Create(() => cardManager.DrawCard(player), 1f);
            FunctionTimer.Create(() => cardManager.PlayCard(null, ENEMY), 2f);

            // ENEMY ATTACK
            float delay = 4f;
            void EnemyAttack(GameObject enemyHero)
            {
                if (cardManager.PlayerZoneCards.Count > 0)
                {
                    cardManager.Attack(enemyHero, cardManager.PlayerZoneCards[0]);
                }
                else cardManager.Attack(enemyHero, cardManager.PlayerHero);
            }
            void EndTurnDelay(float delay) => FunctionTimer.Create(() => EndTurn(ENEMY), delay);

            foreach (GameObject enemyHero in cardManager.EnemyZoneCards)
            {
                if (!enemyHero.GetComponent<FollowerCardDisplay>().IsExhausted)
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
        Debug.LogWarning("EndTurn(" + player + ")");
        CardManager.Instance.RemoveTemporaryEffects(PLAYER); // TESTING
        CardManager.Instance.RemoveTemporaryEffects(ENEMY); // TESTING
        if (player == ENEMY) StartTurn(PLAYER);
        else if (player == PLAYER) StartTurn(ENEMY);
    }
}
