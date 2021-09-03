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
    
    private CombatManager coMan;
    private EventManager evMan;
    private EnemyHero enemyHero;
    private int enemyHealth;

    public bool IsMyTurn { get; set; }
    public List<Card> EnemyDeckList { get; private set; }
    public List<Card> CurrentEnemyDeck { get; private set; }
    public List<int> ReinforcementSchedule { get; private set; }
    public int ReinforcementGroup { get; set; }
    public int CurrentReinforcements { get; set; }
    public EnemyHero EnemyHero
    {
        get => enemyHero;
        set
        {
            enemyHero = value;
            if (EnemyDeckList == null || CurrentEnemyDeck == null) return; // TESTING
            EnemyDeckList.Clear();
            if (value == null)
            {
                CurrentEnemyDeck.Clear();
                return;
            }
            foreach (UnitCard unit in enemyHero.Reinforcements[ReinforcementGroup].ReinforcementUnits)
                for (int i = 0; i < GameManager.ENEMY_START_FOLLOWERS; i++)
                    CardManager.Instance.AddCard(unit, GameManager.ENEMY);
            ReinforcementSchedule = EnemyHero.Reinforcements[ReinforcementGroup].ReinforcementSchedule;
            CurrentReinforcements = 0;
        }
    }
    public int EnemyHealth
    {
        get => enemyHealth;
        set
        {
            enemyHealth = value;
            CombatManager.Instance.EnemyHero.GetComponent<HeroDisplay>().HeroHealth = enemyHealth;
        }
    }

    private void Start()
    {
        coMan = CombatManager.Instance;
        evMan = EventManager.Instance;
    }

    public void StartCombat()
    {
        EnemyDeckList = new List<Card>();
        CurrentEnemyDeck = new List<Card>();
        ReinforcementSchedule = new List<int>();
        ReinforcementGroup = 0; // FOR TESTING ONLY
    }

    public void StartEnemyTurn()
    {
        int refo = CurrentReinforcements;
        List<int> refoSched = ReinforcementSchedule;

        float refoDelay = 1;
        if (refoSched[refo] > 0)
        {
            evMan.NewDelayedAction(() => Reinforcements(), 1);
            refoDelay = 4;
        }

        if ((refo + 1) < refoSched.Count) CurrentReinforcements++;
        else CurrentReinforcements = 0;

        evMan.NewDelayedAction(() => NextReinforcements(), refoDelay);
        for (int i = 0; i < refoSched[refo]; i++)
            evMan.NewDelayedAction(() => coMan.DrawCard(GameManager.ENEMY), 1);
        for (int i = 0; i < refoSched[refo]; i++)
            evMan.NewDelayedAction(() => coMan.PlayCard(coMan.EnemyHandCards[0]), 2);
        evMan.NewDelayedAction(() => BeginAttack(), 1);

        void Reinforcements()
        {
            AnimationManager.Instance.ReinforcementsState(coMan.EnemyHero);
            AudioManager.Instance.StartStopSound("SFX_Reinforcements");
        }
        void NextReinforcements()
        {
            coMan.EnemyHero.GetComponent<EnemyHeroDisplay>().NextReinforcements = 
                refoSched[CurrentReinforcements];
        }
        void BeginAttack()
        {
            foreach (GameObject enemyUnit in coMan.EnemyZoneCards)
            {
                UnitCardDisplay ucd = enemyUnit.GetComponent<UnitCardDisplay>();
                if (!ucd.IsExhausted && ucd.CurrentPower > 0)
                    evMan.NewDelayedAction(() => FinishAttack(enemyUnit), 1);
            }
            // END TURN
            evMan.NewDelayedAction(() => 
            GameManager.Instance.EndTurn(GameManager.ENEMY), 2);
        }
        void FinishAttack(GameObject enemyUnit)
        {
            bool isPlayed = coMan.EnemyZoneCards.Contains(enemyUnit);
            if (!isPlayed) return;
            if (coMan.PlayerZoneCards.Count > 0)
            {
                foreach (GameObject playerUnit in coMan.PlayerZoneCards)
                    if (!CardManager.GetAbility(playerUnit, "Stealth"))
                    {
                        coMan.Attack(enemyUnit, playerUnit);
                        return;
                    }
            }
            coMan.Attack(enemyUnit, coMan.PlayerHero);
        }
    }
}
