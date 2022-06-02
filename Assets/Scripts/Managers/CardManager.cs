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
    private EffectManager efMan;
    private EventManager evMan;

    [Header("PREFABS")]
    [SerializeField] private GameObject unitCardPrefab;
    [SerializeField] private GameObject actionCardPrefab;
    [Header("PLAYER START UNITS")]
    [SerializeField] private UnitCard[] playerStartUnits;
    [Header("TUTORIAL PLAYER UNITS")]
    [SerializeField] private UnitCard[] tutorialPlayerUnits;

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
    public const string TRIGGER_TRAP = "Trap";
    public const string TRIGGER_TURN_START = "Turn Start";
    public const string TRIGGER_TURN_END = "Turn End";

    // Unit Types
    public const string MAGE = "Mage";
    public const string MUTANT = "Mutant";
    public const string ROGUE = "Rogue";
    public const string TECH = "Tech";
    public const string WARRIOR = "Warrior";

    // Ability Keywords
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
        "Evade",
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
        TRIGGER_TRAP,
        TRIGGER_TURN_START,
        TRIGGER_TURN_END
    };

    // Card Types
    private readonly string[] CardTypes = new string[]
    {
        "Exploit",
        "Invention",
        "Scheme"
    };

    public GameObject UnitCardPrefab { get => unitCardPrefab; }
    public GameObject ActionCardPrefab { get => actionCardPrefab; }
    public UnitCard[] PlayerStartUnits { get => playerStartUnits; }
    public UnitCard[] TutorialPlayerUnits { get => tutorialPlayerUnits; }

    public List<UnitCard> PlayerRecruitMages { get; private set; }
    public List<UnitCard> PlayerRecruitMutants { get; private set; }
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
                PlayerRecruitMutants,
                PlayerRecruitRogues,
                PlayerRecruitTechs,
                PlayerRecruitWarriors,
            };

            while (returnList.Count < 8)
            {
                foreach (List<UnitCard> list in recruitLists)
                {
                    foreach (UnitCard uc in list)
                    {
                        if (uc == null) continue;
                        int index = pMan.PlayerDeckList.FindIndex(x => x.CardName == uc.CardName);
                        if (index == -1 && !returnList.Contains(uc))
                        {
                            returnList.Add(uc);
                            if (returnList.Count > 7) return returnList;
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
        efMan = EffectManager.Instance;
        evMan = EventManager.Instance;

        PlayerRecruitMages = new List<UnitCard>();
        PlayerRecruitMutants = new List<UnitCard>();
        PlayerRecruitRogues = new List<UnitCard>();
        PlayerRecruitTechs = new List<UnitCard>();
        PlayerRecruitWarriors = new List<UnitCard>();
    }

    public string FilterKeywords(string text)
    {
        foreach (string s in AbilityKeywords)
            text = text.Replace(s, "<b><color=\"yellow\">" + s + "</b></color>");
        foreach (string s in CardTypes)
            text = text.Replace(s, "<b><color=\"green\">" + s + "</b></color>");
        return text;
    }

    public string FilterCardTypes(string text)
    {
        text = ReplaceText(MAGE, "orange");
        text = ReplaceText(MUTANT, "green");
        text = ReplaceText(ROGUE, "purple");
        text = ReplaceText(TECH, "blue");
        text = ReplaceText(WARRIOR, "red");
        return text;

        string ReplaceText(string target, string color)
        {
            text = text.Replace(target, "<color=\"" + color + "\">" + target + "</color>");
            return text;
        }
    }

    public void LoadNewRecruits()
    {
        UnitCard[] allRecruits = Resources.LoadAll<UnitCard>("Recruit Units");
        foreach (UnitCard unitCard in allRecruits)
        {
            switch (unitCard.CardType)
            {
                case MAGE:
                    PlayerRecruitMages.Add(unitCard);
                    break;
                case MUTANT:
                    PlayerRecruitMutants.Add(unitCard);
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

    public enum ChooseCard
    {
        Action,
        Unit
    }
    public Card[] ChooseCards(ChooseCard chooseCard)
    {
        Card[] allChooseCards;
        string chooseCardType;

        switch (chooseCard)
        {
            case ChooseCard.Action:
                chooseCardType = "Combat Rewards";
                break;
            case ChooseCard.Unit:
                chooseCardType = "Recruit Units";
                break;
            default:
                Debug.LogError("INVALID TYPE!");
                return null;
        }

        allChooseCards = Resources.LoadAll<Card>(chooseCardType);
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
            int playerDeckIndex = pMan.PlayerDeckList.FindIndex
                (x => x.CardName == card.CardName);

            if (playerDeckIndex == -1)
            {
                card.CurrentEnergyCost = card.StartEnergyCost;
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
    public void AddCard(Card card, string hero, bool newCard = false)
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
        else if (card is ActionCard)
        {
            if (card is SkillCard) cardInstance = ScriptableObject.CreateInstance<SkillCard>();
            else cardInstance = ScriptableObject.CreateInstance<ActionCard>();
        }
        else
        {
            Debug.LogError("CARD TYPE NOT FOUND!");
            return;
        }
        cardInstance.LoadCard(card);
        deck.Add(cardInstance);
        if (hero == GameManager.PLAYER && newCard) UnitReputationChange(card, false); // TESTING
    }
    public void RemovePlayerCard(Card card)
    {
        PlayerManager.Instance.PlayerDeckList.Remove(card);
        UnitReputationChange(card, true); // TESTING
    }

    private void UnitReputationChange(Card card, bool isRemoval)
    {
        if (card is UnitCard) { } // TESTING
        else return;

        int change;
        if (isRemoval) change = -1;
        else change = 1;
        GameManager.ReputationType repType;

        switch(card.CardType)
        {
            case MAGE:
                repType = GameManager.ReputationType.Mages;
                break;
            case MUTANT:
                repType = GameManager.ReputationType.Mutants;
                break;
            case ROGUE:
                repType = GameManager.ReputationType.Rogues;
                break;
            case TECH:
                repType = GameManager.ReputationType.Techs;
                break;
            case WARRIOR:
                repType = GameManager.ReputationType.Warriors;
                break;
            default:
                Debug.LogError("INVALID REPUTATION TYPE!");
                return;
        }
        GameManager.Instance.ChangeReputation(repType, change);
    }
    
    /******
     * *****
     * ****** SHUFFLE_DECK
     * *****
     *****/
    public void ShuffleDeck(string hero, bool playSound = true)
    {
        Debug.Log("SHUFFLE <" + hero + "> DECK!");
        if (hero == GameManager.PLAYER)
        {
            pMan.CurrentPlayerDeck.Shuffle();
            pMan.CurrentPlayerSkillDeck.Shuffle();
        }
        else if (hero == GameManager.ENEMY)
        {
            enMan.CurrentEnemyDeck.Shuffle();
            playSound = false;
        }
        else
        {
            Debug.LogError("INVALID HERO!");
            return;
        }

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
        if (hero == GameManager.PLAYER)
        {
            pMan.CurrentPlayerDeck.Clear();
            pMan.CurrentPlayerSkillDeck.Clear();
            foreach (Card card in pMan.PlayerDeckList)
            {
                if (card is SkillCard sc)
                    pMan.CurrentPlayerSkillDeck.Add(sc);
                else pMan.CurrentPlayerDeck.Add(card);
            }
            pMan.CurrentPlayerSkillDeck.Shuffle();
            pMan.CurrentPlayerDeck.Shuffle();
        }
        else if (hero == GameManager.ENEMY)
        {
            enMan.CurrentEnemyDeck.Clear();
            foreach (Card card in enMan.EnemyDeckList)
                enMan.CurrentEnemyDeck.Add(card);
            enMan.CurrentEnemyDeck.Shuffle();
        }
        else
        {
            Debug.LogError("HERO NOT FOUND!");
            return;
        }
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
        List<TriggeredAbility> traAbilities = new List<TriggeredAbility>();
        List<TriggeredAbility> ctrlAbilities = new List<TriggeredAbility>();
        
        foreach (CardAbility ca in ucd.CurrentAbilities)
            if (ca is TriggeredAbility tra)
                if (tra.AbilityTrigger.AbilityName == triggerName)
                {
                    Debug.Log("TRIGGER! <" + triggerName + ">");
                    effectFound = true;
                    if (HasControlAbility(tra)) ctrlAbilities.Add(tra);
                    else traAbilities.Add(tra);
                }

        foreach (TriggeredAbility ctrlTra in ctrlAbilities)
        {
            evMan.NewDelayedAction(() =>
            TriggerAbility(ctrlTra), 0.5f, true);
        }

        foreach (TriggeredAbility traAbi in traAbilities)
        {
            evMan.NewDelayedAction(() =>
            TriggerAbility(traAbi), 0.5f, true);
        }

        return effectFound;

        bool HasControlAbility(TriggeredAbility tra)
        {
            foreach (EffectGroup eg in tra.EffectGroupList)
            {
                foreach (Effect e in eg.Effects)
                    if (e is ChangeControlEffect) return true;
            }
            return false;
        }

        void TriggerAbility(TriggeredAbility tra)
        {
            string triggerName = tra.AbilityTrigger.AbilityName;
            if (triggerName != TRIGGER_PLAY)
                auMan.StartStopSound(null, ucd.CardScript.CardPlaySound);
            efMan.StartEffectGroupList(tra.EffectGroupList, unitCard, triggerName);
        }
    }
    public void TriggerPlayedUnits(string triggerName, string player)
    {
        List<GameObject> unitZoneCards;
        if (player == GameManager.PLAYER) unitZoneCards = CombatManager.Instance.PlayerZoneCards;
        else unitZoneCards = CombatManager.Instance.EnemyZoneCards;

        foreach (GameObject unit in unitZoneCards)
            TriggerUnitAbility(unit, triggerName);

        // ENEMY HERO POWER
        if (player == GameManager.ENEMY && enMan.EnemyHero.EnemyHeroPower != null &&
            enMan.EnemyHero.EnemyHeroPower.PowerTrigger.AbilityName == triggerName)
            enMan.UseHeroPower();
    }

    public void TriggerTrapAbilities(GameObject trappedUnit)
    {
        List<GameObject> unitZoneCards;
        if (trappedUnit.CompareTag(CombatManager.PLAYER_CARD)) unitZoneCards = CombatManager.Instance.EnemyZoneCards;
        else unitZoneCards = CombatManager.Instance.PlayerZoneCards;

        foreach (GameObject unit in unitZoneCards)
        {
            UnitCardDisplay ucd = unit.GetComponent<UnitCardDisplay>();
            foreach (CardAbility ca in ucd.CurrentAbilities)
                if (ca is TrapAbility trapAbility)
                {
                    ucd.AbilityTriggerState(TRIGGER_TRAP);

                    foreach (Effect selfEffect in trapAbility.SelfEffects)
                        evMan.NewDelayedAction(() =>
                        TriggerTrapEffects(unit, selfEffect, unit), 0.5f, true);

                    foreach (Effect trapEffect in trapAbility.TrapEffects)
                        evMan.NewDelayedAction(() =>
                        TriggerTrapEffects(trappedUnit, trapEffect, unit), 0.5f, true);
                }
        }

        void TriggerTrapEffects(GameObject unit, Effect effect, GameObject source) =>
            efMan.ResolveEffect(new List<GameObject> { unit }, effect, 0, false, source);
    }
}