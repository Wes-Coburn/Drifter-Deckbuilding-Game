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
        EnemyDeckList = new List<Card>();
        CurrentEnemyDeck = new List<Card>();
        ReinforcementSchedule = new List<int>();
    }

    CardManager cardMan;
    EventManager eveMan;

    /* ENEMY_HERO */
    public EnemyHero EnemyHero
    {
        get => enemyHero;
        set
        {
            enemyHero = value;
            foreach (UnitCard follower in enemyHero.Reinforcements[0].ReinforcementUnits)
            {
                for (int i = 0; i < GameManager.ENEMY_START_FOLLOWERS; i++)
                {
                    CardManager.Instance.AddCard(follower, GameManager.ENEMY);
                }
            }
            ReinforcementSchedule = EnemyHero.Reinforcements[0].ReinforcementSchedule;
            CurrentReinforcements = 0;
        }
    }
    private EnemyHero enemyHero;

    /* ENEMY_DECK */
    public List<Card> EnemyDeckList { get; private set; }
    public List<Card> CurrentEnemyDeck { get; private set; }
    public List<int> ReinforcementSchedule { get; private set; }
    public int CurrentReinforcements { get; set; }

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

    /******
     * *****
     * ****** START_ENEMY_TURN
     * *****
     *****/
    public void StartEnemyTurn()
    {
        cardMan = CardManager.Instance;
        eveMan = EventManager.Instance;

        int refo = CurrentReinforcements;
        List<int> refoSched = ReinforcementSchedule;

        // DELAYED ACTIONS
        for (int i = 0; i < refoSched[refo]; i++)
            eveMan.NewDelayedAction(() => cardMan.DrawCard(GameManager.ENEMY), 1.5f);

        for (int i = 0; i < refoSched[refo]; i++)
            eveMan.NewDelayedAction(() => cardMan.PlayCard(cardMan.EnemyHandCards[0]), 1.5f);

        if ((refo + 1) < refoSched.Count) CurrentReinforcements++;
        else CurrentReinforcements = 0;

        cardMan.EnemyHero.GetComponent<EnemyHeroDisplay>().NextReinforcements =
            refoSched[CurrentReinforcements];

        eveMan.NewDelayedAction(() => CMBeginAttack(), 1f);

        void CMBeginAttack()
        {
            foreach (GameObject enemyUnit in cardMan.EnemyZoneCards)
            {
                UnitCardDisplay ucd = enemyUnit.GetComponent<UnitCardDisplay>();
                if (!ucd.IsExhausted && ucd.CurrentPower > 0)
                    eveMan.NewDelayedAction(() => CMAttack(enemyUnit), 1f);
            }
            // END TURN
            eveMan.NewDelayedAction(() => GameManager.Instance.EndTurn(GameManager.ENEMY), 1f);
        }

        void CMAttack(GameObject enemyUnit)
        {
            bool isPlayed = cardMan.EnemyZoneCards.Contains(enemyUnit);
            if (!isPlayed) return;
            if (cardMan.PlayerZoneCards.Count > 0)
            {
                foreach (GameObject playerUnit in cardMan.PlayerZoneCards)
                {
                    if (!CardManager.GetAbility(playerUnit, "Stealth"))
                    {
                        cardMan.Attack(enemyUnit, playerUnit);
                        return;
                    }
                }
            }
            cardMan.Attack(enemyUnit, cardMan.PlayerHero);
        }
    }
}
