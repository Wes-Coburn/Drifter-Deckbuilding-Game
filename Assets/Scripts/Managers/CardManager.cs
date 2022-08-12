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
    private CombatManager coMan;

    [Header("PREFABS")]
    [SerializeField] private GameObject unitCardPrefab;
    [SerializeField] private GameObject actionCardPrefab;
    [SerializeField] private GameObject cardContainerPrefab;
    [SerializeField] private GameObject dragArrowPrefab;
    [Header("PLAYER START UNITS")]
    [SerializeField] private UnitCard[] playerStartUnits;
    [Header("TUTORIAL PLAYER UNITS")]
    [SerializeField] private UnitCard[] tutorialPlayerUnits;

    // Static Abilities
    public const string ABILITY_ARMORED = "Armored";
    public const string ABILITY_BLITZ = "Blitz";
    public const string ABILITY_FORCEFIELD = "Forcefield";
    public const string ABILITY_POISONOUS = "Poisonous";
    public const string ABILITY_RANGED = "Ranged";
    public const string ABILITY_STEALTH = "Stealth";
    public const string ABILITY_WARD = "Ward";
    // Status Abilities
    public const string ABILITY_MARKED = "Marked";
    public const string ABILITY_POISONED = "Poisoned";
    public const string ABILITY_WOUNDED = "Wounded";
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

    // Ability Keywords
    private static string[] AbilityKeywords = new string[]
    {
        // Static
        ABILITY_ARMORED,
        ABILITY_BLITZ,
        ABILITY_FORCEFIELD,
        ABILITY_POISONOUS,
        ABILITY_RANGED,
        ABILITY_STEALTH,
        ABILITY_WARD,
        // Keyword
        ABILITY_MARKED,
        "Mark",
        ABILITY_POISONED,
        "Poison",
        "Silence",
        "Stun",
        "Evade",
        "Exhausted",
        "Refreshed",
        "Refresh",
        ABILITY_WOUNDED,
        // Triggers
        TRIGGER_DEATHBLOW,
        TRIGGER_INFILTRATE,
        TRIGGER_PLAY,
        TRIGGER_RESEARCH,
        TRIGGER_RETALIATE,
        TRIGGER_REVENGE,
        TRIGGER_SPARK,
        "Traps",
        TRIGGER_TRAP,
        TRIGGER_TURN_START,
        TRIGGER_TURN_END
    };

    // Card Types
    private static string[] CardTypes = new string[]
    {
        "Exploit",
        "Invention",
        "Scheme"
    };

    // Unit Types
    public const string MAGE = "Mage";
    public const string MUTANT = "Mutant";
    public const string ROGUE = "Rogue";
    public const string TECH = "Tech";
    public const string WARRIOR = "Warrior";

    // Positive Abilities
    public static string[] PositiveAbilities = new string[]
    {
        ABILITY_ARMORED,
        ABILITY_BLITZ,
        ABILITY_FORCEFIELD,
        ABILITY_POISONOUS,
        ABILITY_RANGED,
        ABILITY_STEALTH
    };

    // Negative Abilities
    public static string[] NegativeAbilities = new string[]
    {
        ABILITY_MARKED,
        ABILITY_POISONED
    };

    public GameObject UnitCardPrefab { get => unitCardPrefab; }
    public GameObject ActionCardPrefab { get => actionCardPrefab; }
    public GameObject CardContainerPrefab { get => cardContainerPrefab; }
    public GameObject DragArrowPrefab { get => dragArrowPrefab; }

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
        coMan = CombatManager.Instance;

        PlayerRecruitMages = new List<UnitCard>();
        PlayerRecruitMutants = new List<UnitCard>();
        PlayerRecruitRogues = new List<UnitCard>();
        PlayerRecruitTechs = new List<UnitCard>();
        PlayerRecruitWarriors = new List<UnitCard>();
    }

    public Card NewCardInstance(Card card, bool isExactCopy = false)
    {
        Card cardScript = null;
        if (card is UnitCard) cardScript = ScriptableObject.CreateInstance<UnitCard>();
        else if (card is ActionCard) cardScript = ScriptableObject.CreateInstance<ActionCard>();

        if (isExactCopy) cardScript.CopyCard(card);
        else cardScript.LoadCard(card);
        return cardScript;
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
        Unit,
        Skill
    }
    public Card[] ChooseCards(ChooseCard chooseCard)
    {
        Card[] allChooseCards;
        string chooseCardType = "";

        switch (chooseCard)
        {
            case ChooseCard.Action:
                chooseCardType = "Combat Rewards";
                break;
            case ChooseCard.Unit:
                chooseCardType = "Recruit Units";
                break;
            case ChooseCard.Skill:
                break;
            default:
                Debug.LogError("INVALID TYPE!");
                return null;
        }

        if (chooseCard is ChooseCard.Skill)
            allChooseCards = pMan.PlayerHero.HeroSkills.ToArray();
        else allChooseCards = Resources.LoadAll<Card>(chooseCardType);

        if (allChooseCards.Length < 1)
        {
            Debug.LogError("NO CARDS FOUND!");
            return null;
        }

        // Rare Card Functionality
        // <Common> : <Rare> ::: <3> : <1>
        List<Card> cardPool = new List<Card>();
        foreach (Card card in allChooseCards)
        {
            cardPool.Add(card);
            if (!card.IsRare)
            {
                cardPool.Add(card);
                cardPool.Add(card);
            }
        }

        cardPool.Shuffle();
        Card[] chooseCards = new Card[3];
        int index = 0;

        // Limit Duplicates
        GetChooseCards(true);
        if (index < 3)
        {
            index = 0;
            GetChooseCards(false);
        }

        return chooseCards;

        void GetChooseCards(bool limitDuplicates)
        {
            foreach (Card card in cardPool)
            {
                bool isDuplicate = false;
                foreach (Card c in chooseCards)
                    if (c == card) isDuplicate = true;
                if (isDuplicate) continue;

                int copies = 0;
                if (limitDuplicates)
                {
                    foreach (Card playerCard in pMan.PlayerDeckList)
                    {
                        if (playerCard.CardName == card.CardName)
                            copies++;
                    }
                }

                if (!limitDuplicates || copies < 1 || (card is SkillCard && copies < 3))
                {
                    chooseCards[index++] = card;
                    if (index == 3) break;
                }
            }
        }
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
        if (hero == GameManager.PLAYER && newCard) UnitReputationChange(card, false);
    }
    public void RemovePlayerCard(Card card)
    {
        PlayerManager.Instance.PlayerDeckList.Remove(card);
        UnitReputationChange(card, true);
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
            pMan.CurrentPlayerDeck.Shuffle();
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
            foreach (Card card in pMan.PlayerDeckList)
                pMan.CurrentPlayerDeck.Add(card);
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
     * ****** GET_ABILITY
     * *****
     *****/
    public static bool GetAbility(GameObject unitCard, string ability)
    {
        if (unitCard == null)
        {
            Debug.LogError("CARD IS NULL!");
            return false;
        }

        if (!unitCard.TryGetComponent(out UnitCardDisplay ucd)) return false;

        int abilityIndex = ucd.CurrentAbilities.FindIndex(x => x.AbilityName == ability);
        if (abilityIndex == -1) return false;
        else return true;
    }
    /******
     * *****
     * ****** GET_TRIGGER
     * *****
     *****/
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
    /******
     * *****
     * ****** TRIGGER_UNIT_ABILITY
     * *****
     *****/
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
                    int additionalTriggers =
                        efMan.TriggerModifiers_TriggerAbility(triggerName, unitCard);
                    int totalTriggers = 1 + additionalTriggers;

                    List<TriggeredAbility> targetList;
                    if (HasControlAbility(tra)) targetList = ctrlAbilities;
                    else targetList = traAbilities;

                    for (int i = 0; i < totalTriggers; i++) targetList.Add(tra);
                }

        float delay = 0.25f;
        int currentAbility = 0;
        int totalAbilities = ctrlAbilities.Count + traAbilities.Count;

        foreach (TriggeredAbility ctrlTra in ctrlAbilities)
        {
            if (++currentAbility == totalAbilities) delay = 0;

            evMan.NewDelayedAction(() =>
            TriggerAbility(ctrlTra), delay, true);
        }
        foreach (TriggeredAbility traAbi in traAbilities)
        {
            if (++currentAbility == totalAbilities) delay = 0;

            evMan.NewDelayedAction(() =>
            TriggerAbility(traAbi), delay, true);
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

        void TriggerAbility(TriggeredAbility tra) =>
            efMan.StartEffectGroupList(tra.EffectGroupList, unitCard, triggerName);
    }
    /******
     * *****
     * ****** TRIGGER_PLAYED_UNITS
     * *****
     *****/
    public void TriggerPlayedUnits(string triggerName, string player)
    {
        List<GameObject> unitZoneCards;
        if (player == GameManager.PLAYER) unitZoneCards = CombatManager.Instance.PlayerZoneCards;
        else unitZoneCards = CombatManager.Instance.EnemyZoneCards;

        foreach (GameObject unit in unitZoneCards)
        {
            if (triggerName == TRIGGER_TURN_START)
            {
                int poisonCount = 0;
                UnitCardDisplay ucd = unit.GetComponent<UnitCardDisplay>();
                foreach (CardAbility ca in ucd.CurrentAbilities)
                    if (ca.AbilityName == ABILITY_POISONED) poisonCount++;

                for (int i = 0; i < poisonCount; i++)
                    evMan.NewDelayedAction(() =>
                    SchedulePoisonEffect(unit, ucd), 0, true);
            }

            if (!GetTrigger(unit, triggerName)) continue;

            evMan.NewDelayedAction(() =>
            TriggerUnitAbility(unit, triggerName), 0.5f, true);
        }

        if (player == GameManager.ENEMY)
        {
            EnemyHeroPower ehp = enMan.EnemyHero.EnemyHeroPower;
            if (ehp != null && ehp.PowerTrigger.AbilityName == triggerName)
                evMan.NewDelayedAction(() =>
                enMan.UseHeroPower(), 0.5f, true);
        }

        void SchedulePoisonEffect(GameObject unit, UnitCardDisplay ucd)
        {
            if (unit != null && ucd.CurrentHealth > 0)
                evMan.NewDelayedAction(() => PoisonEffect(unit, ucd), 0.5f, true);
        }
        void PoisonEffect(GameObject unit, UnitCardDisplay ucd)
        {
            ucd.AbilityTriggerState(ABILITY_POISONED);
            efMan.ResolveEffect(new List<GameObject> { unit },
                efMan.PoisonEffect, false, 0, out _, false);
        }
    }
    /******
     * *****
     * ****** TRIGGER_TRAP_ABILITIES
     * *****
     *****/
    public void TriggerTrapAbilities(GameObject trappedUnit)
    {
        if (trappedUnit == null ||
            GetAbility(trappedUnit, ABILITY_WARD)) return;

        List<GameObject> unitZoneCards;
        List<GameObject> resolveFirstTraps = new List<GameObject>();

        if (trappedUnit.CompareTag(CombatManager.PLAYER_CARD))
            unitZoneCards = coMan.EnemyZoneCards;
        else unitZoneCards = coMan.PlayerZoneCards;

        foreach (GameObject trap in unitZoneCards)
        {
            if (efMan.UnitsToDestroy.Contains(trap)) continue; // TESTING

            UnitCardDisplay ucd = trap.GetComponent<UnitCardDisplay>();
            foreach (CardAbility ca in ucd.CurrentAbilities)
                if (ca is TrapAbility trapAbility)
                {
                    if (trapAbility.ResolveLast) TriggerAllEffects(trap);
                    else resolveFirstTraps.Add(trap);
                }
        }

        foreach (GameObject trap in resolveFirstTraps)
            TriggerAllEffects(trap);

        void TriggerAllEffects(GameObject trap)
        {
            UnitCardDisplay ucd = trap.GetComponent<UnitCardDisplay>();

            foreach (CardAbility ca in ucd.CurrentAbilities)
                if (ca is TrapAbility trapAbility)
                {
                    ucd.AbilityTriggerState(TRIGGER_TRAP);
                    auMan.StartStopSound(null, ucd.UnitCard.CardPlaySound);

                    efMan.UnitsToDestroy.Add(trap);
                    evMan.NewDelayedAction(() => efMan.ClearDestroyedUnits(), 0, true); // TESTING

                    foreach (Effect selfEffect in trapAbility.SelfEffects)
                        evMan.NewDelayedAction(() =>
                        TriggerEffect(trap, selfEffect, false, trap), 0, true);

                    foreach (Effect trapEffect in trapAbility.TrapEffects)
                        evMan.NewDelayedAction(() =>
                        TriggerEffect(trappedUnit, trapEffect, true, trap), 0.5f, true);
                }
        }
        void TriggerEffect(GameObject unit, Effect effect, bool shootRay, GameObject source) =>
            efMan.ResolveEffect(new List<GameObject> { unit }, effect, shootRay, 0, out _, false, source);
    }
}