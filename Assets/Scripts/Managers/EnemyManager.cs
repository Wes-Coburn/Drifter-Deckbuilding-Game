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
    
    private CardManager cMan;
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
            CardManager.Instance.EnemyHero.GetComponent<HeroDisplay>().HeroHealth = enemyHealth;
        }
    }

    private void Start()
    {
        cMan = CardManager.Instance;
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

        float refoDelay = 1f;
        if (refoSched[refo] > 0)
        {
            evMan.NewDelayedAction(() => Reinforcements(), 1f);
            refoDelay = 4f;
        }

        if ((refo + 1) < refoSched.Count) CurrentReinforcements++;
        else CurrentReinforcements = 0;

        evMan.NewDelayedAction(() => NextReinforcements(), refoDelay);
        for (int i = 0; i < refoSched[refo]; i++)
            evMan.NewDelayedAction(() => cMan.DrawCard(GameManager.ENEMY), 1f);
        for (int i = 0; i < refoSched[refo]; i++)
            evMan.NewDelayedAction(() => cMan.PlayCard(cMan.EnemyHandCards[0]), 2f);
        evMan.NewDelayedAction(() => CMBeginAttack(), 1f);

        void Reinforcements()
        {
            AnimationManager.Instance.ReinforcementsState(cMan.EnemyHero);
            AudioManager.Instance.StartStopSound("SFX_Reinforcements");
        }
        void NextReinforcements()
        {
            cMan.EnemyHero.GetComponent<EnemyHeroDisplay>().NextReinforcements = 
                refoSched[CurrentReinforcements];
        }
        void CMBeginAttack()
        {
            foreach (GameObject enemyUnit in cMan.EnemyZoneCards)
            {
                UnitCardDisplay ucd = enemyUnit.GetComponent<UnitCardDisplay>();
                if (!ucd.IsExhausted && ucd.CurrentPower > 0)
                    evMan.NewDelayedAction(() => CMAttack(enemyUnit), 1f);
            }
            // END TURN
            evMan.NewDelayedAction(() => 
            GameManager.Instance.EndTurn(GameManager.ENEMY), 2f);
        }
        void CMAttack(GameObject enemyUnit)
        {
            bool isPlayed = cMan.EnemyZoneCards.Contains(enemyUnit);
            if (!isPlayed) return;
            if (cMan.PlayerZoneCards.Count > 0)
            {
                foreach (GameObject playerUnit in cMan.PlayerZoneCards)
                    if (!CardManager.GetAbility(playerUnit, "Stealth"))
                    {
                        cMan.Attack(enemyUnit, playerUnit);
                        return;
                    }
            }
            cMan.Attack(enemyUnit, cMan.PlayerHero);
        }
    }
}
