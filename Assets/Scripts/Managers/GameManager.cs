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
    public const int ACTIONS_PER_TURN = 3;
    public const int MAXIMUM_ACTIONS = 5;

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
    EventManager eventManager;

    /******
     * *****
     * ****** START
     * *****
     *****/
    private void Start()
    {
        playerManager = PlayerManager.Instance;
        enemyManager = EnemyManager.Instance;
        cardManager = CardManager.Instance;
        UIManager = UIManager.Instance;
        eventManager = EventManager.Instance;
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
        enemyManager.EnemyHero = enemyHero;

        cardManager.UpdateDeck(PLAYER);
        cardManager.UpdateDeck(ENEMY);

        playerManager.PlayerHealth = STARTING_HEALTH;
        playerManager.PlayerActionsLeft = STARTING_ACTIONS;
        enemyManager.EnemyHealth = STARTING_HEALTH;

        cardManager.PlayerHero.GetComponent<HeroDisplay>().HeroScript = playerManager.PlayerHero;
        cardManager.EnemyHero.GetComponent<HeroDisplay>().HeroScript = enemyManager.EnemyHero;

        //FunctionTimer.Create(() => cm.DrawHand(PLAYER, PLAYER_HAND_SIZE), 1f);
        //FunctionTimer.Create(() => cm.DrawHand(ENEMY, ENEMY_HAND_SIZE), 1f);
        //FunctionTimer.Create(() => StartTurn(PLAYER), PLAYER_HAND_SIZE);

        void FinishEvent() => eventManager.FinishDelayedAction();
        void CMDrawCard(string hero)
        {
            CardManager cm = CardManager.Instance;
            cm.DrawCard(hero);
            FinishEvent();
        }
        void GMStartTurn(string hero)
        {
            StartTurn(hero);
            FinishEvent();
        }

        for (int i = 0; i < PLAYER_HAND_SIZE; i++)
        {
            eventManager.NewDelayedAction(() => CMDrawCard(PLAYER), 1f);
        }
        for (int i = 0; i < ENEMY_HAND_SIZE; i++)
        {
            eventManager.NewDelayedAction(() => CMDrawCard(ENEMY), 1f);
        }
        eventManager.NewDelayedAction(() => GMStartTurn(PLAYER), 1f);
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

            int refo = enemyManager.CurrentReinforcements;
            List<int> refoSched = enemyManager.ReinforcementSchedule;

            void FinishEvent() => eventManager.FinishDelayedAction();
            void CMDrawCard(string hero)
            {
                cardManager.DrawCard(hero);
                FinishEvent();
            }
            void CMPlayCard(GameObject card)
            {
                cardManager.PlayCard(card);
                FinishEvent();
            }
            void CMBeginAttack()
            {
                foreach (GameObject enemyHero in cardManager.EnemyZoneCards)
                {
                    if (!enemyHero.GetComponent<FollowerCardDisplay>().IsExhausted)
                    {
                        eventManager.NewDelayedAction(() => CMEnemyAttack(enemyHero), 1f);
                    }
                }
                // END TURN
                eventManager.NewDelayedAction(() => GMEndTurn(ENEMY), 1f);
                FinishEvent();
            }
            void CMEnemyAttack(GameObject hero)
            {
                if (cardManager.PlayerZoneCards.Count > 0)
                {
                    cardManager.Attack(hero, cardManager.PlayerZoneCards[0]);
                }
                else cardManager.Attack(hero, cardManager.PlayerHero);
                FinishEvent();
            }
            void GMEndTurn(string hero)
            {
                EndTurn(hero);
                FinishEvent();
            }

            // DELAYED ACTIONS
            for (int i = 0; i < refoSched[refo]; i++)
            {
                eventManager.NewDelayedAction(() => CMDrawCard(ENEMY), 1f);
            }
            for (int i = 0; i < refoSched[refo]; i++)
            {
                eventManager.NewDelayedAction(() => CMPlayCard(CardManager.Instance.EnemyHandCards[0]), 2f);
            }
            if ((refo + 1) < refoSched.Count) enemyManager.CurrentReinforcements++;
            else enemyManager.CurrentReinforcements = 0;

            CardManager.Instance.EnemyHero.GetComponent<EnemyHeroDisplay>().NextReinforcements =
                refoSched[enemyManager.CurrentReinforcements];

            eventManager.NewDelayedAction(() => CMBeginAttack(), 1f);
        }
    }

    public void EndTurn(string player)
    {
        CardManager.Instance.RemoveTemporaryEffects(PLAYER);
        CardManager.Instance.RemoveTemporaryEffects(ENEMY);
        CardManager.Instance.RemoveGiveNextEffects();
        if (player == ENEMY) StartTurn(PLAYER);
        else if (player == PLAYER) StartTurn(ENEMY);
    }
}
