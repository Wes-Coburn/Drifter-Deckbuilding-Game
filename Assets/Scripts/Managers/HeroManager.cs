using System.Collections.Generic;
using UnityEngine;

public abstract class HeroManager : MonoBehaviour
{
    protected Hero heroScript;
    private GameObject heroObject;
    protected bool isMyTurn;
    protected int turnNumber, currentHealth, damageTaken_ThisTurn, energyPerTurn, currentEnergy,
        exploitsPlayed, inventionsPlayed, schemesPlayed, extractionsPlayed;

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

    // Zone Objects
    public Vector2 HandStart { get; private set; }

    public GameObject HandZone { get; set; }
    public GameObject PlayZone { get; set; }
    public GameObject ActionZone { get; set; }

    // !!!
    public abstract bool IsMyTurn { get; set; } // << PlayerData! >>
    public abstract int TurnNumber { get; set; } // << PlayerData! >>

    public int TurnNumber_Direct { set => turnNumber = value; }

    // Zone Lists
    public List<GameObject> HandZoneCards { get; set; } // << PlayerData! >>
    public List<GameObject> PlayZoneCards { get; set; } // << PlayerData! >>
    public List<GameObject> ActionZoneCards { get; set; } // << PlayerData! >>
    public List<Card> DiscardZoneCards { get; set; } // << PlayerData! >>

    // Decks
    public List<Card> DeckList { get; set; }
    public List<Card> CurrentDeck { get; set; } // << PlayerData! >>

    // Effect Lists
    public List<GiveNextUnitEffect> GiveNextEffects { get; set; } // << PlayerData! >>
    public List<ChangeCostEffect> ChangeNextCostEffects { get; set; } // << PlayerData! >>
    public List<ModifyNextEffect> ModifyNextEffects { get; set; } // << PlayerData! >>

    // Counters
    public int AlliesDestroyed_ThisTurn { get; set; } // << PlayerData! >>

    // Health
    public bool IsWounded() { return damageTaken_ThisTurn >= GameManager.WOUNDED_VALUE; }
    public int CurrentHealth // << PlayerData! >>
    {
        get => currentHealth;
        set
        {
            currentHealth = value;
            HeroObject.GetComponent<HeroDisplay>().HeroHealth = currentHealth;
        }
    }
    public abstract int MaxHealth { get; }
    public int DamageTaken_ThisTurn // << PlayerData! >>
    {
        get => damageTaken_ThisTurn;
        set
        {
            bool wasWounded = damageTaken_ThisTurn >= GameManager.WOUNDED_VALUE;
            damageTaken_ThisTurn = value;
            bool isWounded = damageTaken_ThisTurn >= GameManager.WOUNDED_VALUE;
            HeroObject.GetComponent<HeroDisplay>().IsWounded = isWounded;

            if (!wasWounded && isWounded)
            {
                if (heroScript == null)
                {
                    Debug.LogError("HERO SCRIPT IS NULL!");
                    return;
                }

                List<GameObject> enemyZoneList = heroScript is PlayerHero ? 
                    Managers.EN_MAN.PlayZoneCards : Managers.P_MAN.PlayZoneCards;
                
                Managers.EF_MAN.TriggerModifiers_SpecialTrigger
                    (ModifierAbility.TriggerType.EnemyHeroWounded, enemyZoneList);
            }
        }
    }

    // Energy
    public int EnergyPerTurn // << PlayerData! >>
    {
        get => energyPerTurn;
        set
        {
            energyPerTurn = value > MaxEnergyPerTurn ? MaxEnergyPerTurn : value;
            HeroObject.GetComponent<HeroDisplay>().SetHeroEnergy(CurrentEnergy, energyPerTurn);
        }
    }
    public int MaxEnergyPerTurn => GameManager.MAXIMUM_ENERGY_PER_TURN;
    public int MaxEnergy => GameManager.MAXIMUM_ENERGY;
    public int CurrentEnergy // << PlayerData! >>
    {
        get => currentEnergy;
        set
        {
            currentEnergy = value > MaxEnergy ? MaxEnergy : value;
            HeroObject.GetComponent<HeroDisplay>().SetHeroEnergy(CurrentEnergy, energyPerTurn);

            if (currentEnergy < 0)
            {
                Debug.LogError("NEGATIVE ENERGY LEFT <" + currentEnergy + ">");
            }
        }
    }

    // Trackers
    public int ExploitsPlayed // << PlayerData! >>
    {
        get => exploitsPlayed;
        set
        {
            exploitsPlayed = value;
            if (value == 3)
            {
                exploitsPlayed = 0;
                Managers.EV_MAN.NewDelayedAction(() => DrawUltimate(), 0, true);

                void DrawUltimate()
                {
                    Card card = Managers.CA_MAN.NewCardInstance(Managers.CA_MAN.Exploit_Ultimate);
                    Managers.CA_MAN.DrawCard(this, card);
                }
            }
            else if (value > 3) Debug.LogError("VALUE > 3!");
        }
    }

    public int InventionsPlayed // << PlayerData! >>
    {
        get => inventionsPlayed;
        set
        {
            inventionsPlayed = value;
            if (value == 3)
            {
                inventionsPlayed = 0;
                Managers.EV_MAN.NewDelayedAction(() => DrawUltimate(), 0, true);
                
                void DrawUltimate()
                {
                    Card card = Managers.CA_MAN.NewCardInstance(Managers.CA_MAN.Invention_Ultimate);
                    Managers.CA_MAN.DrawCard(this, card);
                }
            }
            else if (value > 3) Debug.LogError("VALUE > 3!");
        }
    }

    public int SchemesPlayed // << PlayerData! >>
    {
        get => schemesPlayed;
        set
        {
            schemesPlayed = value;
            if (value == 3)
            {
                schemesPlayed = 0;
                Managers.EV_MAN.NewDelayedAction(() => DrawUltimate(), 0, true);
                
                void DrawUltimate()
                {
                    Card card = Managers.CA_MAN.NewCardInstance(Managers.CA_MAN.Scheme_Ultimate);
                    Managers.CA_MAN.DrawCard(this, card);
                }
            }
            else if (value > 3) Debug.LogError("VALUE > 3!");
        }
    }

    public int ExtractionsPlayed // << PlayerData! >>
    {
        get => extractionsPlayed;
        set
        {
            extractionsPlayed = value;
            if (value == 3)
            {
                extractionsPlayed = 0;
                Managers.EV_MAN.NewDelayedAction(() => DrawUltimate(), 0, true);
                
                void DrawUltimate()
                {
                    Card card = Managers.CA_MAN.NewCardInstance(Managers.CA_MAN.Extraction_Ultimate);
                    Managers.CA_MAN.DrawCard(this, card);
                }
            }
            else if (value > 3) Debug.LogError("VALUE > 3!");
        }
    }

    protected virtual void Start()
    {
        HandZoneCards = new();
        PlayZoneCards = new();
        ActionZoneCards = new();
        DiscardZoneCards = new();

        DeckList = new();
        CurrentDeck = new();

        GiveNextEffects = new();
        ChangeNextCostEffects = new();
        ModifyNextEffects = new();
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

        if (sourceObject.TryGetComponent(out ItemIcon _)) return PlayerManager.Instance;

        foreach (var hMan in new HeroManager[] { PlayerManager.Instance, EnemyManager.Instance })
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
        var sourcehero = GetSourceHero(sourceObject);
        enemyHero = sourcehero == PlayerManager.Instance ? EnemyManager.Instance : PlayerManager.Instance;
        return sourcehero;
    }
}
