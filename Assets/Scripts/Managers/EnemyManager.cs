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
    private AnimationManager anMan;

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
            ReinforcementSchedule =
                EnemyHero.Reinforcements[ReinforcementGroup].ReinforcementSchedule;
            CurrentReinforcements = 0;
            NextReinforcements = ReinforcementSchedule[CurrentReinforcements]; 
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
        anMan = AnimationManager.Instance;
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
        if (NextReinforcements > 0) 
            evMan.NewDelayedAction(() => anMan.ReinforcementsState(), 1);
        if ((refo + 1) < refoSched.Count) CurrentReinforcements++;
        else CurrentReinforcements = 0;
        int handSize = coMan.EnemyHandCards.Count;
        // Draw Cards
        int cardsToDraw = NextReinforcements;
        int overMaxCards = GameManager.MAX_HAND_SIZE + 1;
        if (cardsToDraw + handSize > overMaxCards)
            cardsToDraw = overMaxCards - handSize;
        for (int i = 0; i < cardsToDraw; i++)
            evMan.NewDelayedAction(() => coMan.DrawCard(GameManager.ENEMY), 1);
        // Play Cards
        int cardsToPlay = handSize + NextReinforcements;
        int maxHand = GameManager.MAX_HAND_SIZE;
        int overMaxUnits = GameManager.MAX_UNITS_PLAYED + 1;
        int playedUnits = coMan.EnemyZoneCards.Count;
        if (cardsToPlay > maxHand) cardsToPlay = maxHand;
        if (cardsToPlay + playedUnits > overMaxUnits) 
            cardsToPlay = overMaxUnits - playedUnits;
        for (int i = 0; i < cardsToPlay; i++)
            evMan.NewDelayedAction(() =>
            coMan.PlayCard(coMan.EnemyHandCards[0]), 2);
        evMan.NewDelayedAction(() => BeginAttacks(), 1);

        void UpdateReinforcements()
        {
            anMan.NextReinforcementsState();
            NextReinforcements = refoSched[CurrentReinforcements];
        }
        void BeginAttacks()
        {
            foreach (GameObject enemyUnit in coMan.EnemyZoneCards)
            {
                UnitCardDisplay ucd = coMan.GetUnitDisplay(enemyUnit);
                if (!ucd.IsExhausted && ucd.CurrentPower > 0 && ucd.CurrentHealth > 0)
                    evMan.NewDelayedAction(() => ResolveAttack(enemyUnit), 1);
            }
            evMan.NewDelayedAction(() => UpdateReinforcements(), 2);
            evMan.NewDelayedAction(() => 
            GameManager.Instance.EndTurn(GameManager.ENEMY), 2);
        }
        void ResolveAttack(GameObject enemyUnit)
        {
            if (!coMan.EnemyZoneCards.Contains(enemyUnit)) return;
            UnitCardDisplay ucd = coMan.GetUnitDisplay(enemyUnit);
            if (ucd.CurrentPower < 1 || ucd.CurrentHealth < 1) return;

            if (CardManager.GetTrigger(enemyUnit, "Infiltrate"))
            {
                coMan.Attack(enemyUnit, coMan.PlayerHero);
                return;
            }
            else if (coMan.PlayerZoneCards.Count > 0)
            {
                foreach (GameObject playerUnit in coMan.PlayerZoneCards)
                    if (coMan.GetUnitDisplay(playerUnit).CurrentHealth > 0 &&
                        !CardManager.GetAbility(playerUnit, "Stealth"))
                    {
                        coMan.Attack(enemyUnit, playerUnit);
                        return;
                    }
            }
            coMan.Attack(enemyUnit, coMan.PlayerHero);
        }
    }
}
