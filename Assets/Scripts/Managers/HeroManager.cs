using System.Collections.Generic;
using UnityEngine;

public abstract class HeroManager : MonoBehaviour
{
    protected Hero heroScript;
    protected bool isMyTurn;
    protected int turnNumber;
    private GameObject heroObject;
    private int currentHealth;
    private int damageTakenTurn;
    private int energyPerTurn;
    private int currentEnergy;

    private int exploitsPlayed;
    private int inventionsPlayed;
    private int schemesPlayed;
    private int extractionsPlayed;

    protected CombatManager coMan;
    private EventManager evMan;
    private CardManager caMan;

    // Hero
    public abstract Hero HeroScript { get; set; }
    public abstract bool IsMyTurn { get; set; }
    public abstract int TurnNumber { get; set; }

    // Zones
    public Vector2 HandStart { get; private set; }
    public GameObject HeroObject
    {
        get => heroObject;
        set
        {
            heroObject = value;
            HandStart = HandZone.transform.position;
        }
    }
    public GameObject HandZone { get; set; }
    public GameObject PlayZone { get; set; }
    public GameObject ActionZone { get; set; }
    public GameObject DiscardZone { get; set; }

    // Lists
    public List<GameObject> HandZoneCards { get; private set; }
    public List<GameObject> PlayZoneCards { get; private set; }
    public List<GameObject> ActionZoneCards { get; private set; }
    public List<Card> DiscardZoneCards { get; private set; }

    // Decks
    public List<Card> DeckList { get; private set; }
    public List<Card> CurrentDeck { get; private set; }

    // Effect Lists
    public List<GiveNextUnitEffect> GiveNextEffects { get; private set; }
    public List<ChangeCostEffect> ChangeNextCostEffects { get; private set; }
    public List<ModifyNextEffect> ModifyNextEffects { get; private set; }

    // Health
    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = value;
            HeroObject.GetComponent<HeroDisplay>().HeroHealth = currentHealth;
        }
    }
    public abstract int MaxHealth { get; }
    public int DamageTakenTurn
    {
        get => damageTakenTurn;
        set
        {
            bool wasWounded;
            if (damageTakenTurn >= GameManager.WOUNDED_VALUE) wasWounded = true;
            else wasWounded = false;

            damageTakenTurn = value;
            bool isWounded;
            if (damageTakenTurn >= GameManager.WOUNDED_VALUE) isWounded = true;
            else isWounded = false;
            HeroObject.GetComponent<HeroDisplay>().IsWounded = isWounded;

            if (!wasWounded && isWounded)
            {
                List<GameObject> enemyZoneList;
                if (heroScript is PlayerHero) enemyZoneList = EnemyManager.Instance.PlayZoneCards;
                else if (heroScript is EnemyHero) enemyZoneList = PlayerManager.Instance.PlayZoneCards;
                else
                {
                    if (heroScript == null) Debug.LogError("HERO SCRIPT IS NULL!");
                    else Debug.LogError("INVALID HERO TYPE!");
                    return;
                }

                EffectManager.Instance.TriggerModifiers_SpecialTrigger
                    (ModifierAbility.TriggerType.EnemyHeroWounded, enemyZoneList);
            }
        }
    }
    public bool IsWounded() { return damageTakenTurn >= GameManager.WOUNDED_VALUE; }

    // Energy
    public int EnergyPerTurn
    {
        get => energyPerTurn;
        set
        {
            energyPerTurn = value;
            if (energyPerTurn > MaxEnergyPerTurn)
                energyPerTurn = MaxEnergyPerTurn;
            HeroObject.GetComponent<HeroDisplay>().SetHeroEnergy(CurrentEnergy, energyPerTurn);
        }
    }
    public int MaxEnergyPerTurn => GameManager.MAXIMUM_ENERGY_PER_TURN;
    public int MaxEnergy => GameManager.MAXIMUM_ENERGY;
    public int CurrentEnergy
    {
        get => currentEnergy;
        set
        {
            currentEnergy = value;
            if (currentEnergy > MaxEnergy) currentEnergy = MaxEnergy;
            HeroObject.GetComponent<HeroDisplay>().SetHeroEnergy(CurrentEnergy, energyPerTurn);

            if (currentEnergy < 0)
            {
                Debug.LogError("NEGATIVE ENERGY LEFT <" + currentEnergy + ">");
            }
        }
    }

    // Trackers
    public int ExploitsPlayed
    {
        get => exploitsPlayed;
        set
        {
            exploitsPlayed = value;
            if (value == 3)
            {
                exploitsPlayed = 0;

                evMan.NewDelayedAction(() => DrawUltimate(), 0, true);
                void DrawUltimate()
                {
                    Card card = caMan.NewCardInstance(caMan.Exploit_Ultimate);
                    caMan.DrawCard(this, card);
                }
            }
            else if (value > 3) Debug.LogError("VALUE > 3!");
        }
    }

    public int InventionsPlayed
    {
        get => inventionsPlayed;
        set
        {
            inventionsPlayed = value;
            if (value == 3)
            {
                inventionsPlayed = 0;

                evMan.NewDelayedAction(() => DrawUltimate(), 0, true);
                void DrawUltimate()
                {
                    Card card = caMan.NewCardInstance(caMan.Invention_Ultimate);
                    caMan.DrawCard(this, card);
                }
            }
            else if (value > 3) Debug.LogError("VALUE > 3!");
        }
    }

    public int SchemesPlayed
    {
        get => schemesPlayed;
        set
        {
            schemesPlayed = value;
            if (value == 3)
            {
                schemesPlayed = 0;

                evMan.NewDelayedAction(() => DrawUltimate(), 0, true);
                void DrawUltimate()
                {
                    Card card = caMan.NewCardInstance(caMan.Scheme_Ultimate);
                    caMan.DrawCard(this, card);
                }
            }
            else if (value > 3) Debug.LogError("VALUE > 3!");
        }
    }

    public int ExtractionsPlayed
    {
        get => extractionsPlayed;
        set
        {
            extractionsPlayed = value;
            if (value == 3)
            {
                extractionsPlayed = 0;

                evMan.NewDelayedAction(() => DrawUltimate(), 0, true);
                void DrawUltimate()
                {
                    Card card = caMan.NewCardInstance(caMan.Extraction_Ultimate);
                    caMan.DrawCard(this, card);
                }
            }
            else if (value > 3) Debug.LogError("VALUE > 3!");
        }
    }

    protected virtual void Start()
    {
        coMan = CombatManager.Instance;
        caMan = CardManager.Instance;
        evMan = EventManager.Instance;

        HandZoneCards = new List<GameObject>();
        PlayZoneCards = new List<GameObject>();
        ActionZoneCards = new List<GameObject>();
        DiscardZoneCards = new List<Card>();

        DeckList = new List<Card>();
        CurrentDeck = new List<Card>();

        GiveNextEffects = new List<GiveNextUnitEffect>();
        ChangeNextCostEffects = new List<ChangeCostEffect>();
        ModifyNextEffects = new List<ModifyNextEffect>();
    }

    public virtual void ResetForCombat()
    {
        exploitsPlayed = 0;
        inventionsPlayed = 0;
        schemesPlayed = 0;
        extractionsPlayed = 0;

        HandZoneCards.Clear();
        PlayZoneCards.Clear();
        ActionZoneCards.Clear();
        DiscardZoneCards.Clear();

        foreach (Effect e in GiveNextEffects) Destroy(e);
        GiveNextEffects.Clear();
        foreach (Effect e in ChangeNextCostEffects) Destroy(e);
        ChangeNextCostEffects.Clear();
        foreach (Effect e in ModifyNextEffects) Destroy(e);
        ModifyNextEffects.Clear();
    }
}
