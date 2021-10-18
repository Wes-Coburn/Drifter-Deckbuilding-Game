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
    private int nextReinforcements;

    public bool IsMyTurn { get; set; }
    public List<Card> EnemyDeckList { get; private set; }
    public List<Card> CurrentEnemyDeck { get; private set; }
    public List<int> ReinforcementSchedule { get; private set; }
    public int ReinforcementGroup { get; set; }
    public int CurrentReinforcements { get; set; }
    public int NextReinforcements
    {
        get => nextReinforcements;
        set
        {
            nextReinforcements = value;
            coMan.EnemyHero.GetComponent<EnemyHeroDisplay>().NextReinforcements = value;
        }
    }

    public EnemyHero EnemyHero
    {
        get => enemyHero;
        set
        {
            enemyHero = value;
            if (EnemyDeckList == null || CurrentEnemyDeck == null) return;
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
            NextReinforcements = ReinforcementSchedule[CurrentReinforcements]; // TESTING
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
        if (NextReinforcements > 0) // TESTING
        {
            evMan.NewDelayedAction(() => ShowReinforcements(), 1);
            refoDelay = 4;
        }

        if ((refo + 1) < refoSched.Count) CurrentReinforcements++;
        else CurrentReinforcements = 0;

        int handSize = coMan.EnemyHandCards.Count;
        // Draw Cards
        int cardsToDraw = NextReinforcements; // TESTING
        int overMaxCards = GameManager.MAX_HAND_SIZE + 1;
        if (cardsToDraw + handSize > overMaxCards)
            cardsToDraw = overMaxCards - handSize;
        evMan.NewDelayedAction(() => UpdateReinforcements(), refoDelay);
        for (int i = 0; i < cardsToDraw; i++)
            evMan.NewDelayedAction(() => coMan.DrawCard(GameManager.ENEMY), 1);

        // Play Cards
        int cardsToPlay = handSize + NextReinforcements; // TESTING
        int maxHand = GameManager.MAX_HAND_SIZE;
        int overMaxUnits = GameManager.MAX_UNITS_PLAYED + 1;
        int playedUnits = coMan.EnemyZoneCards.Count;
        if (cardsToPlay > maxHand) cardsToPlay = maxHand;
        if (cardsToPlay + playedUnits > overMaxUnits) 
            cardsToPlay = overMaxUnits - playedUnits;
        for (int i = 0; i < cardsToPlay; i++)
            evMan.NewDelayedAction(() => coMan.PlayCard(coMan.EnemyHandCards[0]), 2);

        evMan.NewDelayedAction(() => BeginAttack(), 1);

        void ShowReinforcements()
        {
            AnimationManager.Instance.ReinforcementsState(coMan.EnemyHero);
            AudioManager.Instance.StartStopSound("SFX_Reinforcements");
        }
        void UpdateReinforcements() =>
            NextReinforcements = refoSched[CurrentReinforcements]; // TESTING
        void BeginAttack()
        {
            foreach (GameObject enemyUnit in coMan.EnemyZoneCards)
            {
                UnitCardDisplay ucd = enemyUnit.GetComponent<UnitCardDisplay>();
                if (!ucd.IsExhausted && ucd.CurrentPower > 0)
                    evMan.NewDelayedAction(() => FinishAttack(enemyUnit), 1);
            }
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
