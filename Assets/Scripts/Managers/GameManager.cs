using System.Collections.Generic;
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
    public const int STARTING_HEALTH = 15;
    public const int STARTING_ACTIONS = 0;
    public const int ACTIONS_PER_TURN = 2;
    public const int MAXIMUM_ACTIONS = 6;

    public const string PLAYER = "Player";
    public const int PLAYER_HAND_SIZE = 4;
    public const int PLAYER_START_FOLLOWERS = 6;
    public const int PLAYER_START_SKILLS = 2;

    public const string ENEMY = "Enemy";
    public const int ENEMY_HAND_SIZE = 0;
    public const int ENEMY_START_FOLLOWERS = 8;
    public const int ENEMY_START_SKILLS = 2;

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
     * ****** NEW_GAME
     * *****
     *****/
    public void NewGame()
    {
        PlayerManager.Instance.PlayerHero = playerTestHero; // FOR TESTING ONLY
        StartCombat(enemyTestHero); // FOR TESTING ONLY
    }

    /******
     * *****
     * ****** START/END_COMBAT
     * *****
     *****/
    private void StartCombat(Hero enemyHero)
    {
        CardManager cm = CardManager.Instance;
        PlayerManager pm = PlayerManager.Instance;
        EnemyManager em = EnemyManager.Instance;
        em.EnemyHero = enemyHero;

        cm.UpdateDeck(PLAYER);
        cm.UpdateDeck(ENEMY);

        pm.PlayerHealth = STARTING_HEALTH;
        pm.PlayerActionsLeft = STARTING_ACTIONS;
        em.EnemyHealth = STARTING_HEALTH;

        CardManager.Instance.PlayerHero.GetComponent<HeroDisplay>().HeroScript = pm.PlayerHero;
        
        FunctionTimer.Create(() => CardManager.Instance.DrawHand(PLAYER, PLAYER_HAND_SIZE), 1f);
        FunctionTimer.Create(() => CardManager.Instance.DrawHand(ENEMY, ENEMY_HAND_SIZE), 1f);
        FunctionTimer.Create(() => StartTurn(PLAYER), PLAYER_HAND_SIZE);
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
        cardManager.RefreshFollowers(player);

        // PLAYER
        if (player == PLAYER)
        {
            playerManager.IsMyTurn = true;
            enemyManager.IsMyTurn = false;
            UIManager.UpdateEndTurnButton(true);

            PlayerManager.Instance.PlayerActionsLeft += ACTIONS_PER_TURN;
            playerManager.HeroPowerUsed = false;

            FunctionTimer.Create(() => cardManager.DrawCard(PLAYER), 1f);
        }

        // ENEMY
        else if (player == ENEMY)
        {
            playerManager.IsMyTurn = false;
            enemyManager.IsMyTurn = true;
            UIManager.UpdateEndTurnButton(false);

            // ENEMY REINFORCEMENTS
            int reinforcements = enemyManager.CurrentReinforcements;
            List<int> refoSched = enemyManager.ReinforcementSchedule;

            int delay;
            for (delay = 1; delay < (refoSched[reinforcements] + 1); delay++)
            {
                FunctionTimer.Create(() => cardManager.DrawCard(ENEMY), delay);
                FunctionTimer.Create(() => cardManager.PlayCard(CardManager.Instance.EnemyHandCards[0]), 2*delay);
            }
            if ((reinforcements + 1) < refoSched.Count) enemyManager.CurrentReinforcements++;
            else enemyManager.CurrentReinforcements = 0;

            // ENEMY ATTACK
            delay += 3;
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
                    delay += 2;
                }
            }
            EndTurnDelay(delay);
        }
    }

    public void EndTurn(string player)
    {
        CardManager.Instance.RemoveTemporaryEffects(PLAYER);
        CardManager.Instance.RemoveTemporaryEffects(ENEMY);
        CardManager.Instance.RemoveGiveNextEffects(); // TESTING
        if (player == ENEMY) StartTurn(PLAYER);
        else if (player == PLAYER) StartTurn(ENEMY);
    }
}
