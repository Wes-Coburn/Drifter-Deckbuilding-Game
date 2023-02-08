using System.Collections.Generic;
using UnityEngine;

public abstract class HeroManager : MonoBehaviour
{
    protected Hero heroScript;
    private GameObject heroObject;
    protected bool isMyTurn;
    protected int turnNumber;
    private int currentHealth;
    private int damageTakenTurn;
    private int energyPerTurn;
    private int currentEnergy;

    private int exploitsPlayed;
    private int inventionsPlayed;
    private int schemesPlayed;
    private int extractionsPlayed;

    public abstract string HERO_TAG { get; }
    public abstract string CARD_TAG { get; }
    public abstract string HAND_ZONE_TAG { get; }
    public abstract string PLAY_ZONE_TAG { get; }
    public abstract string ACTION_ZONE_TAG { get; }
    public abstract string DISCARD_ZONE_TAG { get; }
    public abstract string HERO_POWER_TAG { get; }
    public abstract string HERO_ULTIMATE_TAG { get; }

    // Hero
    public abstract Hero HeroScript { get; set; }
    public GameObject HeroObject
    {
        get => heroObject;
        set
        {
            heroObject = value;
            HandStart = HandZone.transform.position;
        }
    }

    public abstract bool IsMyTurn { get; set; }
    public abstract int TurnNumber { get; set; }

    // Zone Objects
    public Vector2 HandStart { get; private set; }
    
    public GameObject HandZone { get; set; }
    public GameObject PlayZone { get; set; }
    public GameObject ActionZone { get; set; }
    public GameObject DiscardZone { get; set; }

    // Zone Lists
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
    public bool IsWounded() { return damageTakenTurn >= GameManager.WOUNDED_VALUE; }
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

    // Energy
    public int EnergyPerTurn
    {
        get => energyPerTurn;
        set
        {
            energyPerTurn = value;
            if (energyPerTurn > MaxEnergyPerTurn) energyPerTurn = MaxEnergyPerTurn;
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

                ManagerHandler.EV_MAN.NewDelayedAction(() => DrawUltimate(), 0, true);
                void DrawUltimate()
                {
                    Card card = ManagerHandler.CA_MAN.NewCardInstance(ManagerHandler.CA_MAN.Exploit_Ultimate);
                    ManagerHandler.CA_MAN.DrawCard(this, card);
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

                ManagerHandler.EV_MAN.NewDelayedAction(() => DrawUltimate(), 0, true);
                void DrawUltimate()
                {
                    Card card = ManagerHandler.CA_MAN.NewCardInstance(ManagerHandler.CA_MAN.Invention_Ultimate);
                    ManagerHandler.CA_MAN.DrawCard(this, card);
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

                ManagerHandler.EV_MAN.NewDelayedAction(() => DrawUltimate(), 0, true);
                void DrawUltimate()
                {
                    Card card = ManagerHandler.CA_MAN.NewCardInstance(ManagerHandler.CA_MAN.Scheme_Ultimate);
                    ManagerHandler.CA_MAN.DrawCard(this, card);
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

                ManagerHandler.EV_MAN.NewDelayedAction(() => DrawUltimate(), 0, true);
                void DrawUltimate()
                {
                    Card card = ManagerHandler.CA_MAN.NewCardInstance(ManagerHandler.CA_MAN.Extraction_Ultimate);
                    ManagerHandler.CA_MAN.DrawCard(this, card);
                }
            }
            else if (value > 3) Debug.LogError("VALUE > 3!");
        }
    }

    protected virtual void Start()
    {
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

    public static HeroManager GetSourceHero(GameObject sourceObject)
    {
        if (sourceObject == null)
        {
            Debug.LogError("SOURCE IS NULL!");
            return null;
        }

        foreach (HeroManager hMan in new List<HeroManager>() { PlayerManager.Instance, EnemyManager.Instance })
        {
            if (compare(hMan.HERO_TAG) || compare(hMan.CARD_TAG) ||
                compare(hMan.HERO_POWER_TAG) || compare(hMan.HERO_ULTIMATE_TAG))
                return hMan;

            bool compare(string tag) => sourceObject.CompareTag(tag);
        }
        
        Debug.LogError("INVALID TAG!");
        return null;
    }

    public static HeroManager GetSourceHero(GameObject sourceObject, out HeroManager enemyHero)
    {
        HeroManager sourcehero = GetSourceHero(sourceObject);
        enemyHero = sourcehero == PlayerManager.Instance ? EnemyManager.Instance : PlayerManager.Instance;
        return sourcehero;
    }
}
