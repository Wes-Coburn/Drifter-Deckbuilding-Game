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

    private UIManager uMan;
    private PlayerManager pMan;
    private EnemyManager enMan;
    private AudioManager auMan;

    [Header("PREFABS")]
    [SerializeField] private GameObject unitCardPrefab;
    [SerializeField] private GameObject actionCardPrefab;
    [SerializeField] private GameObject newCardPopupPrefab;

    [Header("PLAYER UNITS")]
    [SerializeField] private UnitCard[] playerStartUnits;
    [SerializeField] private List<UnitCard> playerRecruitUnits;

    [Header("TRIGGER")]
    [SerializeField] private CardAbility triggerKeyword;

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
    public const string TRIGGER_REVENGE = "Revenge";
    public const string TRIGGER_SPARK = "Spark";

    public static List<string> EvergreenTriggers = new List<string>
    {
        TRIGGER_DEATHBLOW,
        TRIGGER_INFILTRATE,
        TRIGGER_RESEARCH,
        TRIGGER_REVENGE,
        TRIGGER_SPARK,
    };

    public GameObject UnitCardPrefab { get => unitCardPrefab; }
    public GameObject ActionCardPrefab { get => actionCardPrefab; }
    public GameObject NewCardPopup { get; private set; }
    public UnitCard[] PlayerStartUnits { get => playerStartUnits; }
    public List<UnitCard> PlayerRecruitUnits { get => playerRecruitUnits; }
    public CardAbility TriggerKeyword { get => triggerKeyword; }


    private void Start()
    {
        uMan = UIManager.Instance;
        pMan = PlayerManager.Instance;
        enMan = EnemyManager.Instance;
        auMan = AudioManager.Instance;
    }

    /******
     * *****
     * ****** ADD/REMOVE_CARD
     * *****
     *****/
    public void AddPlayerCard(Card card, string hero, bool isStartingCard = true)
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
        if (!isStartingCard)
        {
            uMan.DestroyNewCardPopup();
            uMan.CreateNewCardPopup(cardInstance);
        }
    }
    public void RemovePlayerCard(Card card)
    {
        PlayerManager.Instance.PlayerDeckList.Remove(card);
    }
    
    /******
     * *****
     * ****** SHUFFLE_DECK
     * *****
     *****/
    public void ShuffleDeck(List<Card> deck)
    {
        deck.Shuffle();
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
    public static bool GetAbility(GameObject card, string ability)
    {
        if (card == null)
        {
            Debug.LogError("CARD IS NULL!");
            return false;
        }

        UnitCardDisplay ucd = card.GetComponent<UnitCardDisplay>();
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
            Debug.LogError("CARD IS NULL!");
            return false;
        }

        UnitCardDisplay ucd = unitCard.GetComponent<UnitCardDisplay>();
        bool effectFound = false;
        foreach (CardAbility ca in ucd.CurrentAbilities)
            if (ca is TriggeredAbility tra)
                if (tra.AbilityTrigger.AbilityName == triggerName)
                {
                    Debug.LogWarning("TRIGGER! <" + triggerName + ">");
                    EffectManager.Instance.StartEffectGroupList(tra.EffectGroupList, unitCard, triggerName);
                    effectFound = true;
                }
        return effectFound;
    }
    public bool TriggerPlayedUnits(string triggerName)
    {
        bool triggerFound = false;
        foreach (GameObject unit in CombatManager.Instance.PlayerZoneCards)
        {
            if (TriggerUnitAbility(unit, triggerName))
                triggerFound = true;
        }
        return triggerFound;
    }
}
