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
    private CardManager caMan;

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
                for (int i = 0; i < GameManager.ENEMY_START_UNITS; i++)
                    caMan.AddCard(unit, GameManager.ENEMY);
            caMan.ShuffleDeck(GameManager.ENEMY, false);
            ReinforcementSchedule =
                EnemyHero.Reinforcements[ReinforcementGroup].ReinforcementSchedule.Schedule;
            CurrentReinforcements = 0;
            NextReinforcements = ReinforcementSchedule[CurrentReinforcements];
            refoSched = ReinforcementSchedule; // TESTING
        }
    }
    public int EnemyHealth
    {
        get => enemyHealth;
        set
        {
            enemyHealth = value;
            coMan.EnemyHero.GetComponent<HeroDisplay>().HeroHealth = enemyHealth;
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
        caMan = CardManager.Instance;
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
        evMan.NewDelayedAction(() => BeginAttacks(), 0);
    }

    private void BeginAttacks()
    {
        foreach (GameObject ally in coMan.EnemyZoneCards)
        {
            UnitCardDisplay ucd = coMan.GetUnitDisplay(ally);
            if (!ucd.IsExhausted && ucd.CurrentPower > 0 && ucd.CurrentHealth > 0)
                evMan.NewDelayedAction(() => ResolveAttack(ally), 1);
        }
        evMan.NewDelayedAction(() => UpdateReinforcements(), 1);
        evMan.NewDelayedAction(() =>
        GameManager.Instance.EndCombatTurn(GameManager.ENEMY), 1);
    }

    private void ResolveAttack(GameObject ally)
    {
        if (ally == null)
        {
            Debug.LogWarning("ENEMY UNIT IS NULL!");
            return;
        }

        UnitCardDisplay ucd = coMan.GetUnitDisplay(ally);
        if (ucd.CurrentPower < 1 || ucd.CurrentHealth < 1) return;

        GameObject defender = FindDefender(ally);
        coMan.Attack(ally, defender);
    }
    
    private int TotalEnemyPower()
    {
        int totalPower = 0;
        foreach (GameObject enemy in coMan.EnemyZoneCards)
            totalPower += coMan.GetUnitDisplay(enemy).CurrentPower;
        return totalPower;
    }
    
    private int TotalAllyPower()
    {
        int totalPower = 0;
        foreach (GameObject ally in coMan.PlayerZoneCards)
        {
            UnitCardDisplay ucd = coMan.GetUnitDisplay(ally);
            if (!ucd.IsExhausted) totalPower += ucd.CurrentPower;
        }
        return totalPower;
    }

    private GameObject FindDefender(GameObject enemy)
    {
        GameObject defender = null;
        int playerHealth = PlayerManager.Instance.PlayerHealth;

        // If player hero is <LETHALLY THREATENED>
        if (TotalEnemyPower() >= playerHealth) return coMan.PlayerHero;

        // If enemy hero is NOT <MILDLY THREATENED>
        if (EnemyHealth > MaxEnemyHealth * 0.5f && TotalAllyPower() < EnemyHealth * 0.55f)
        {
            // If player hero IS <THREATENED>
            if (TotalEnemyPower() >= playerHealth * 0.75f ||
                CardManager.GetTrigger(enemy, CardManager.TRIGGER_INFILTRATE))
                return coMan.PlayerHero;
        }

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

            priority += power + (power/3) - health/2;
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
        if (defender == null) return coMan.PlayerHero;
        return defender;
    }

    public void UseHeroPower()
    {
        List<EffectGroup> groupList = EnemyHero.EnemyHeroPower.EffectGroupList;
        GameObject heroPower = coMan.EnemyHero.GetComponent<EnemyHeroDisplay>().HeroPower;
        EffectManager.Instance.StartEffectGroupList(groupList, heroPower);

        Sound[] soundList;
        soundList = EnemyHero.EnemyHeroPower.PowerSounds;
        foreach (Sound s in soundList) AudioManager.Instance.StartStopSound(null, s);

        AnimationManager.Instance.TriggerHeroPower(heroPower);
    }

    public void UpdateReinforcements()
    {
        int cardsToDraw = NextReinforcements;
        if (cardsToDraw < 1)
        {
            evMan.NewDelayedAction(() => NextReinforcementsState(), 1, true);
            return;
        }
        int overMaxCards = GameManager.MAX_HAND_SIZE + 1;
        if (cardsToDraw + handSize > overMaxCards)
            cardsToDraw = overMaxCards - handSize;
        float delay = 2 + ((3 - cardsToDraw) * 0.5f);
        if (delay < 0) delay = 0;

        evMan.NewDelayedAction(() => NextReinforcementsState(), delay, true);
        for (int i = 0; i < cardsToDraw; i++)
            evMan.NewDelayedAction(() => coMan.DrawCard(GameManager.ENEMY), 0.5f, true);
        evMan.NewDelayedAction(() => anMan.ReinforcementsState(), 0, true);
    }

    private void NextReinforcementsState()
    {
        anMan.NextReinforcementsState();
        if (++CurrentReinforcements > refoSched.Count - 1) CurrentReinforcements = 0;
        NextReinforcements = refoSched[CurrentReinforcements];
    }
}
