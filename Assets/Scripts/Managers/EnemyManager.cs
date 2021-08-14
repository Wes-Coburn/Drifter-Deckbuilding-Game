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
    
    private CardManager cm;
    private EventManager em;

    /* IS_MY_TURN */
    public bool IsMyTurn { get; set; }

    /* ENEMY_HERO */
    public EnemyHero EnemyHero
    {
        get => enemyHero;
        set
        {
            enemyHero = value;
            EnemyDeckList.Clear();
            foreach (UnitCard unit in enemyHero.Reinforcements[ReinforcementGroup].ReinforcementUnits)
                for (int i = 0; i < GameManager.ENEMY_START_FOLLOWERS; i++)
                    CardManager.Instance.AddCard(unit, GameManager.ENEMY);
            ReinforcementSchedule = EnemyHero.Reinforcements[ReinforcementGroup].ReinforcementSchedule;
            CurrentReinforcements = 0;
        }
    }
    private EnemyHero enemyHero;

    /* ENEMY_DECK */
    public List<Card> EnemyDeckList { get; private set; }
    public List<Card> CurrentEnemyDeck { get; private set; }
    public List<int> ReinforcementSchedule { get; private set; }
    public int ReinforcementGroup { get; set; }
    public int CurrentReinforcements { get; set; }

    /* ENEMY_HEALTH */
    private int enemyHealth;
    public int EnemyHealth
    {
        get => enemyHealth;
        set
        {
            enemyHealth = value;
            CardManager.Instance.EnemyHero.GetComponent<HeroDisplay>().HeroHealth = enemyHealth;
        }
    }

    /******
     * *****
     * ****** START_COMBAT
     * *****
     *****/
    public void StartCombat()
    {
        EnemyDeckList = new List<Card>();
        CurrentEnemyDeck = new List<Card>();
        ReinforcementSchedule = new List<int>();
        ReinforcementGroup = 0; // FOR TESTING ONLY
    }

    /******
     * *****
     * ****** START_ENEMY_TURN
     * *****
     *****/
    public void StartEnemyTurn()
    {
        cm = CardManager.Instance;
        em = EventManager.Instance;

        int refo = CurrentReinforcements;
        List<int> refoSched = ReinforcementSchedule;

        // DELAYED ACTIONS
        for (int i = 0; i < refoSched[refo]; i++)
            em.NewDelayedAction(() => cm.DrawCard(GameManager.ENEMY), 1f);
        for (int i = 0; i < refoSched[refo]; i++)
            em.NewDelayedAction(() => cm.PlayCard(cm.EnemyHandCards[0]), 2f);
        
        if ((refo + 1) < refoSched.Count) CurrentReinforcements++;
        else CurrentReinforcements = 0;

        cm.EnemyHero.GetComponent<EnemyHeroDisplay>().NextReinforcements =
            refoSched[CurrentReinforcements];
        em.NewDelayedAction(() => CMBeginAttack(), 1f);

        void CMBeginAttack()
        {
            foreach (GameObject enemyUnit in cm.EnemyZoneCards)
            {
                UnitCardDisplay ucd = enemyUnit.GetComponent<UnitCardDisplay>();
                if (!ucd.IsExhausted && ucd.CurrentPower > 0)
                    em.NewDelayedAction(() => CMAttack(enemyUnit), 1f);
            }
            // END TURN
            em.NewDelayedAction(() => GameManager.Instance.EndTurn(GameManager.ENEMY), 2f);
        }

        void CMAttack(GameObject enemyUnit)
        {
            bool isPlayed = cm.EnemyZoneCards.Contains(enemyUnit);
            if (!isPlayed) return;
            if (cm.PlayerZoneCards.Count > 0)
            {
                foreach (GameObject playerUnit in cm.PlayerZoneCards)
                    if (!CardManager.GetAbility(playerUnit, "Stealth"))
                    {
                        cm.Attack(enemyUnit, playerUnit);
                        return;
                    }
            }
            cm.Attack(enemyUnit, cm.PlayerHero);
        }
    }
}
