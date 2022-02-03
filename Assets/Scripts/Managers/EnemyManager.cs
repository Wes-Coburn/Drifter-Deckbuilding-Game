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

    private List<int> refoSched;
    private int handSize;

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
            if (EnemyHero.Reinforcements[ReinforcementGroup] == null)
            {
                Debug.LogError("REINFORCEMENTS NOT FOUND!");
                return;
            }
            foreach (UnitCard unit in enemyHero.Reinforcements[ReinforcementGroup].ReinforcementUnits)
                for (int i = 0; i < GameManager.ENEMY_START_Units; i++)
                    CardManager.Instance.AddCard(unit, GameManager.ENEMY);
            ReinforcementSchedule =
                EnemyHero.Reinforcements[ReinforcementGroup].ReinforcementSchedule.Schedule;
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
    public int MaxEnemyHealth
    {
        get
        {
            int bonusHealth = 0;
            if (enemyHero.IsBoss) bonusHealth = GameManager.BOSS_BONUS_HEALTH;
            return GameManager.ENEMY_STARTING_HEALTH + bonusHealth;
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
        refoSched = ReinforcementSchedule;
        handSize = coMan.EnemyHandCards.Count;
        int cardsToPlay = handSize;
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
    }

    private void BeginAttacks()
    {
        foreach (GameObject enemyUnit in coMan.EnemyZoneCards)
        {
            UnitCardDisplay ucd = coMan.GetUnitDisplay(enemyUnit);
            if (!ucd.IsExhausted && ucd.CurrentPower > 0 && ucd.CurrentHealth > 0)
                evMan.NewDelayedAction(() => ResolveAttack(enemyUnit), 1);
        }
        UpdateReinforcements();
        evMan.NewDelayedAction(() =>
        GameManager.Instance.EndCombatTurn(GameManager.ENEMY), 1);
    }

    private void ResolveAttack(GameObject enemyUnit)
    {
        if (enemyUnit == null)
        {
            Debug.LogWarning("ENEMY UNIT IS NULL!");
            return;
        }

        UnitCardDisplay ucd = coMan.GetUnitDisplay(enemyUnit);
        if (ucd.CurrentPower < 1 || ucd.CurrentHealth < 1) return;

        if (CardManager.GetTrigger(enemyUnit, CardManager.TRIGGER_INFILTRATE) ||
            CardManager.GetTrigger(enemyUnit, CardManager.TRIGGER_RETALIATE) ||
            TotalAllyPower() >= PlayerManager.Instance.PlayerHealth)
        {
            coMan.Attack(enemyUnit, coMan.PlayerHero);
            return;
        }
        else if (coMan.PlayerZoneCards.Count > 0)
        {
            GameObject playerUnit = FindDefender();
            if (playerUnit != null)
            {
                coMan.Attack(enemyUnit, playerUnit);
                return;
            }
        }
        coMan.Attack(enemyUnit, coMan.PlayerHero);
    }
    
    private int TotalAllyPower()
    {
        int totalPower = 0;
        foreach (GameObject ally in coMan.EnemyZoneCards)
            totalPower += coMan.GetUnitDisplay(ally).CurrentPower;
        return totalPower;
    }

    private GameObject FindDefender()
    {
        GameObject defender = null;
        List<GameObject> legalDefenders = new List<GameObject>();
        foreach (GameObject playerUnit in coMan.PlayerZoneCards)
            if (coMan.GetUnitDisplay(playerUnit).CurrentHealth > 0 &&
                !CardManager.GetAbility(playerUnit, CardManager.ABILITY_STEALTH))
                legalDefenders.Add(playerUnit);

        int highestPriority = -99;
        foreach (GameObject legalDefender in legalDefenders)
        {
            UnitCardDisplay ucd = coMan.GetUnitDisplay(legalDefender);
            int priority = 0;
            int power = ucd.CurrentPower;
            int health = ucd.CurrentHealth;

            priority += power - health/2;
            if (CardManager.GetAbility(legalDefender, CardManager.ABILITY_FORCEFIELD))
                priority -= 2;
            if (CardManager.GetAbility(legalDefender, CardManager.ABILITY_RANGED))
                priority += 2;

            if (priority > highestPriority)
            {
                highestPriority = priority;
                defender = legalDefender;
            }
        }
        return defender;
    }

    private void UpdateReinforcements()
    {
        if (CurrentReinforcements > refoSched.Count - 1)
            CurrentReinforcements = 0;

        int cardsToDraw = NextReinforcements;
        if (cardsToDraw < 1)
        {
            evMan.NewDelayedAction(() => NextReinforcementsState(), 1);
            return;
        }

        evMan.NewDelayedAction(() => anMan.ReinforcementsState(), 1);
        int overMaxCards = GameManager.MAX_HAND_SIZE + 1;
        if (cardsToDraw + handSize > overMaxCards)
            cardsToDraw = overMaxCards - handSize;
        for (int i = 0; i < cardsToDraw; i++)
            evMan.NewDelayedAction(() => coMan.DrawCard(GameManager.ENEMY), 0.5f);

        float delay = 2 + ((3 - cardsToDraw) * 0.5f);
        if (delay < 0) delay = 0;
        evMan.NewDelayedAction(() => NextReinforcementsState(), delay);
    }

    private void NextReinforcementsState()
    {
        anMan.NextReinforcementsState();
        NextReinforcements = refoSched[CurrentReinforcements++];
    }
}
