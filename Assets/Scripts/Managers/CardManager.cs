using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static CardManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private PlayerManager pMan;
    private EnemyManager enMan;
    private AudioManager auMan;

    [Header("PREFABS")]
    [SerializeField] private GameObject unitCardPrefab;
    [SerializeField] private GameObject actionCardPrefab;
    [Header("PLAYER START UNITS")]
    [SerializeField] private UnitCard[] playerStartUnits;

    // Static Abilities
    public const string ABILITY_BLITZ = "Blitz";
    public const string ABILITY_FORCEFIELD = "Forcefield";
    public const string ABILITY_RANGED = "Ranged";
    public const string ABILITY_STEALTH = "Stealth";
    // Keyword Abilities
    public const string ABILITY_MARKED = "Marked";
    // Ability Triggers
    public const string TRIGGER_DEATHBLOW = "Deathblow";
    public const string TRIGGER_INFILTRATE = "Infiltrate";
    public const string TRIGGER_PLAY = "Play";
    public const string TRIGGER_RESEARCH = "Research";
    public const string TRIGGER_RETALIATE = "Retaliate";
    public const string TRIGGER_REVENGE = "Revenge";
    public const string TRIGGER_SPARK = "Spark";
    
    public const string TURN_START = "Turn Start";
    public const string TURN_END = "Turn End";
    
    private readonly string[] AbilityKeywords = new string[]
    {
        // Static
        ABILITY_BLITZ,
        ABILITY_FORCEFIELD,
        ABILITY_RANGED,
        ABILITY_STEALTH,
        // Keyword
        ABILITY_MARKED,
        "Mark",
        "Stun",
        "Exhausted",
        "Refreshed",
        "Refresh",
        // Triggers
        TRIGGER_DEATHBLOW,
        TRIGGER_INFILTRATE,
        TRIGGER_PLAY,
        TRIGGER_RESEARCH,
        TRIGGER_RETALIATE,
        TRIGGER_REVENGE,
        TRIGGER_SPARK,
        TURN_START,
        TURN_END
    };

    private readonly string[] CardTypes = new string[]
    {
        "Intel",
        "Rune"
    };

    public GameObject UnitCardPrefab { get => unitCardPrefab; }
    public GameObject ActionCardPrefab { get => actionCardPrefab; }
    public UnitCard[] PlayerStartUnits { get => playerStartUnits; }

    public List<UnitCard> PlayerRecruitMages { get; private set; }
    public List<UnitCard> PlayerRecruitRogues { get; private set; }
    public List<UnitCard> PlayerRecruitTechs { get; private set; }
    public List<UnitCard> PlayerRecruitWarriors { get; private set; }
    public List<UnitCard> PlayerRecruitUnits
    {
        get
        {
            List<UnitCard> returnList = new List<UnitCard>();
            List<List<UnitCard>> recruitLists = new List<List<UnitCard>>
            {
                PlayerRecruitMages,
                PlayerRecruitRogues,
                PlayerRecruitTechs,
                PlayerRecruitWarriors,
            };

            while (returnList.Count < 4)
            {
                foreach (List<UnitCard> list in recruitLists)
                {
                    foreach (UnitCard uc in list)
                    {
                        if (uc == null) break;
                        int index = pMan.PlayerDeckList.FindIndex(x => x.CardName == uc.CardName);
                        if (index == -1)
                        {
                            returnList.Add(uc);
                            if (returnList.Count > 3) return returnList; // TESTING
                            break;
                        }
                    }
                }
            }
            return returnList;
        }
    }

    private void Start()
    {
        pMan = PlayerManager.Instance;
        enMan = EnemyManager.Instance;
        auMan = AudioManager.Instance;
        PlayerRecruitMages = new List<UnitCard>();
        PlayerRecruitRogues = new List<UnitCard>();
        PlayerRecruitTechs = new List<UnitCard>();
        PlayerRecruitWarriors = new List<UnitCard>();
    }

    public string FilterKeywords(string text)
    {
        string newText = text;
        foreach (string s in AbilityKeywords)
            newText = newText.Replace(s, "<b><color=\"yellow\">" + s + "</b></color>");
        foreach (string s in CardTypes)
            newText = newText.Replace(s, "<b><color=\"blue\">" + s + "</b></color>");
        return newText;
    }

    public void LoadNewRecruits()
    {
        const string MAGE = "Mage";
        const string ROGUE = "Rogue";
        const string TECH = "Tech";
        const string WARRIOR = "Warrior";

        UnitCard[] allRecruits = Resources.LoadAll<UnitCard>("Recruit Units");
        foreach (UnitCard unitCard in allRecruits)
        {
            switch (unitCard.CardType)
            {
                case MAGE:
                    PlayerRecruitMages.Add(unitCard);
                    break;
                case ROGUE:
                    PlayerRecruitRogues.Add(unitCard);
                    break;
                case TECH:
                    PlayerRecruitTechs.Add(unitCard);
                    break;
                case WARRIOR:
                    PlayerRecruitWarriors.Add(unitCard);
                    break;
                default:
                    Debug.LogError("CARD TYPE NOT FOUND FOR <" + unitCard.CardName + ">");
                    return;
            }
        }
    }

    public Card[] ChooseCards()
    {
        Card[] allChooseCards = Resources.LoadAll<Card>("Combat Rewards");
        if (allChooseCards.Length < 1)
        {
            Debug.LogError("NO CARDS FOUND!");
            return null;
        }

        allChooseCards.Shuffle();
        Card[] chooseCards = new Card[3];
        int index = 0;
        foreach (Card card in allChooseCards)
        {
            if (pMan.PlayerDeckList.FindIndex(x => x.CardName == card.CardName) == -1)
            {
                chooseCards[index++] = card;
                if (index == 3) break;
            }
        }
        return chooseCards;
    }

    public void ShuffleRecruits()
    {
        List<List<UnitCard>> recruitLists = new List<List<UnitCard>>
        {
            PlayerRecruitMages,
            PlayerRecruitRogues,
            PlayerRecruitTechs,
            PlayerRecruitWarriors,
        };

        foreach (List<UnitCard> list in recruitLists)
            list.Shuffle();
    }

    /******
     * *****
     * ****** ADD/REMOVE_CARD
     * *****
     *****/
    public void AddCard(Card card, string hero)
    {
        List<Card> deck;
        Card cardInstance;
        if (hero == GameManager.PLAYER) deck = PlayerManager.Instance.PlayerDeckList;
        else if (hero == GameManager.ENEMY) deck = EnemyManager.Instance.EnemyDeckList;
        else
        {
            Debug.LogError("HERO NOT FOUND!");
            return;
        }
        if (card is UnitCard) cardInstance = ScriptableObject.CreateInstance<UnitCard>();
        else if (card is ActionCard) cardInstance = ScriptableObject.CreateInstance<ActionCard>();
        else
        {
            Debug.LogError("CARD TYPE NOT FOUND!");
            return;
        }
        cardInstance.LoadCard(card);
        deck.Add(cardInstance);
    }
    public void RemovePlayerCard(Card card) =>
        PlayerManager.Instance.PlayerDeckList.Remove(card);
    
    /******
     * *****
     * ****** SHUFFLE_DECK
     * *****
     *****/
    public void ShuffleDeck(string hero, bool playSound = true)
    {
        Debug.LogWarning("SHUFFLE <" + hero + "> DECK!");
        List<Card> deck;
        if (hero == GameManager.PLAYER)
            deck = pMan.CurrentPlayerDeck;
        else if (hero == GameManager.ENEMY)
        {
            deck = enMan.CurrentEnemyDeck;
            playSound = false;
        }
        else
        {
            Debug.LogError("INVALID HERO!");
            return;
        }
        deck.Shuffle();

        if (playSound)
            auMan.StartStopSound("SFX_ShuffleDeck");
    }

    /******
     * *****
     * ****** UPDATE_DECK
     * *****
     *****/
    public void UpdateDeck(string hero)
    {
        List<Card> deckList;
        List<Card> currentDeck;

        if (hero == GameManager.PLAYER)
        {
            deckList = pMan.PlayerDeckList;
            currentDeck = pMan.CurrentPlayerDeck;
        }
        else if (hero == GameManager.ENEMY)
        {
            deckList = enMan.EnemyDeckList;
            currentDeck = enMan.CurrentEnemyDeck;
        }
        else
        {
            Debug.LogError("HERO NOT FOUND!");
            return;
        }

        currentDeck.Clear();
        foreach (Card card in deckList)
            currentDeck.Add(card);
        currentDeck.Shuffle();
    }

    /******
     * *****
     * ****** CARD_ABILITIES
     * *****
     *****/
    public static bool GetAbility(GameObject unitCard, string ability)
    {
        if (unitCard == null)
        {
            Debug.LogError("CARD IS NULL!");
            return false;
        }

        if (!unitCard.TryGetComponent(out UnitCardDisplay ucd))
        {
            Debug.LogError("TARGET IS NOT UNIT CARD!");
            return false;
        }

        int abilityIndex = ucd.CurrentAbilities.FindIndex(x => x.AbilityName == ability);
        if (abilityIndex == -1) return false;
        else return true;
    }
    public static bool GetTrigger(GameObject card, string triggerName)
    {
        if (card == null)
        {
            Debug.LogError("CARD IS NULL!");
            return false;
        }

        foreach (CardAbility ca in card.GetComponent<UnitCardDisplay>().CurrentAbilities)
            if (ca is TriggeredAbility tra)
                if (tra.AbilityTrigger.AbilityName == triggerName) return true;
        return false;
    }
    public bool TriggerUnitAbility(GameObject unitCard, string triggerName)
    {
        if (unitCard == null)
        {
            Debug.LogWarning("CARD IS NULL!");
            return false;
        }
        if (!unitCard.TryGetComponent(out UnitCardDisplay ucd))
        {
            Debug.LogError("TARGET IS NOT UNIT CARD!");
            return false;
        }

        bool effectFound = false;
        foreach (CardAbility ca in ucd.CurrentAbilities)
            if (ca is TriggeredAbility tra)
                if (tra.AbilityTrigger.AbilityName == triggerName)
                {
                    Debug.Log("TRIGGER! <" + triggerName + ">");
                    EventManager.Instance.NewDelayedAction(() =>
                    EffectManager.Instance.StartEffectGroupList(tra.EffectGroupList, unitCard, triggerName), 0.5f, true);
                    effectFound = true;
                }
        if (effectFound) auMan.StartStopSound(null, ucd.CardScript.CardPlaySound); // TESTING
        return effectFound;
    }
    public bool TriggerPlayedUnits(string triggerName, string player)
    {
        List<GameObject> unitZoneCards;
        if (player == GameManager.PLAYER) unitZoneCards = CombatManager.Instance.PlayerZoneCards;
        else unitZoneCards = CombatManager.Instance.EnemyZoneCards;
        bool triggerFound = false;

        foreach (GameObject unit in unitZoneCards)
        {
            if (TriggerUnitAbility(unit, triggerName))
                triggerFound = true;
        }
        return triggerFound;
    }
}