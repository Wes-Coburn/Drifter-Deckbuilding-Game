using System.Collections.Generic;
using System.Linq;
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

    #region FIELDS
    private PlayerManager pMan;
    private EnemyManager enMan;
    private AudioManager auMan;
    private EffectManager efMan;
    private EventManager evMan;
    private CombatManager coMan;
    private UIManager uMan;
    private AnimationManager anMan;

    [Header("PREFABS")]
    [SerializeField] private GameObject unitCardPrefab;
    [SerializeField] private GameObject actionCardPrefab;
    [SerializeField] private GameObject cardContainerPrefab;
    [SerializeField] private GameObject dragArrowPrefab;
    [Header("PLAYER START UNITS")]
    [SerializeField] private UnitCard[] playerStartUnits;
    [Header("TUTORIAL PLAYER UNITS")]
    [SerializeField] private UnitCard[] tutorialPlayerUnits;
    [Header("ULTIMATE CREATED CARDS")]
    [SerializeField] private ActionCard exploit_Ultimate;
    [SerializeField] private ActionCard invention_Ultimate;
    [SerializeField] private ActionCard scheme_Ultimate;
    [SerializeField] private ActionCard extraction_Ultimate;

    private int lastCardIndex;
    private int lastContainerIndex;

    // Card Tags
    public const string PLAYER_CARD = "PlayerCard";
    public const string ENEMY_CARD = "EnemyCard";

    // Positive Keywords
    public const string ABILITY_ARMORED = "Armored";
    public const string ABILITY_BLITZ = "Blitz";
    public const string ABILITY_DEFENDER = "Defender";
    public const string ABILITY_FORCEFIELD = "Forcefield";
    public const string ABILITY_POISONOUS = "Poisonous";
    public const string ABILITY_RANGED = "Ranged";
    public const string ABILITY_REGENERATION = "Regeneration";
    public const string ABILITY_STEALTH = "Stealth";
    public const string ABILITY_WARD = "Ward";
    // Status Keywords
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

    // Card Types
    public const string EXPLOIT = "Exploit";
    public const string INVENTION = "Invention";
    public const string SCHEME = "Scheme";

    public const string EXTRACTION = "Extraction";

    // Ability Keywords
    private static string[] AbilityKeywords = new string[]
    {
        // Positive Keywords
        ABILITY_ARMORED,
        ABILITY_BLITZ,
        ABILITY_DEFENDER,
        ABILITY_FORCEFIELD,
        ABILITY_POISONOUS,
        ABILITY_RANGED,
        ABILITY_REGENERATION,
        ABILITY_STEALTH,
        ABILITY_WARD,
        // Status Keywords
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
        // Ability Triggers
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
        EXPLOIT + "s",
        EXPLOIT,
        INVENTION + "s",
        INVENTION,
        SCHEME + "s",
        SCHEME,
        EXTRACTION + "s",
        EXTRACTION
    };

    // Unit Types
    public const string MAGE = "Mage";
    public const string MUTANT = "Mutant";
    public const string ROGUE = "Rogue";
    public const string TECH = "Tech";
    public const string WARRIOR = "Warrior";

    // Generatable Keywords
    [Header("GENERATABLE KEYWORDS")]
    public List<StaticAbility> GeneratableKeywords;

    // Positive Abilities
    public static string[] PositiveAbilities = new string[]
    {
        ABILITY_ARMORED,
        ABILITY_BLITZ,
        ABILITY_DEFENDER,
        ABILITY_FORCEFIELD,
        ABILITY_POISONOUS,
        ABILITY_RANGED,
        ABILITY_REGENERATION,
        ABILITY_STEALTH,
        ABILITY_WARD,
    };

    // Negative Abilities
    public static string[] NegativeAbilities = new string[]
    {
        ABILITY_MARKED,
        ABILITY_POISONED,
        ABILITY_WOUNDED,
    };
    #endregion

    #region PROPERTIES
    public GameObject UnitCardPrefab { get => unitCardPrefab; }
    public GameObject ActionCardPrefab { get => actionCardPrefab; }
    public GameObject CardContainerPrefab { get => cardContainerPrefab; }
    public GameObject DragArrowPrefab { get => dragArrowPrefab; }

    public UnitCard[] PlayerStartUnits { get => playerStartUnits; }
    public UnitCard[] TutorialPlayerUnits { get => tutorialPlayerUnits; }

    public ActionCard Exploit_Ultimate { get => exploit_Ultimate; }
    public ActionCard Invention_Ultimate { get => invention_Ultimate; }
    public ActionCard Scheme_Ultimate { get => scheme_Ultimate; }
    public ActionCard Extraction_Ultimate { get => extraction_Ultimate; }
    
    public List<UnitCard> PlayerRecruitUnits { get; private set; }
    public List<ActionCard> ActionShopCards { get; private set; }
    #endregion

    #region METHODS

    #region UTILITY
    private void Start()
    {
        pMan = PlayerManager.Instance;
        enMan = EnemyManager.Instance;
        auMan = AudioManager.Instance;
        efMan = EffectManager.Instance;
        evMan = EventManager.Instance;
        coMan = CombatManager.Instance;
        uMan = UIManager.Instance;
        anMan = AnimationManager.Instance;

        PlayerRecruitUnits = new List<UnitCard>();
        ActionShopCards = new List<ActionCard>();
    }
    #endregion

    #region CARD HANDLING
    /******
     * *****
     * ****** NEW_CARD_INSTANCE
     * *****
     *****/
    public Card NewCardInstance(Card card, bool isExactCopy = false)
    {
        Card cardScript = null;
        if (card is UnitCard) cardScript = ScriptableObject.CreateInstance<UnitCard>();
        else if (card is ActionCard) cardScript = ScriptableObject.CreateInstance<ActionCard>();

        if (isExactCopy) cardScript.CopyCard(card);
        else cardScript.LoadCard(card);
        return cardScript;
    }
    /******
     * *****
     * ****** SHOW_CARD
     * *****
     *****/
    public enum DisplayType
    {
        Default,
        HeroSelect,
        NewCard,
        ChooseCard,
        Cardpage
    }
    public GameObject ShowCard(Card card, Vector2 position,
        DisplayType type = DisplayType.Default, bool banishAfterPlay = false)
    {
        if (card == null)
        {
            Debug.LogError("CARD IS NULL!");
            return null;
        }

        card.BanishAfterPlay = banishAfterPlay;
        GameObject prefab = null;
        if (card is UnitCard)
        {
            prefab = UnitCardPrefab;
            if (type is DisplayType.NewCard)
                prefab = prefab.GetComponent<CardZoom>().UnitZoomCardPrefab;
        }
        else if (card is ActionCard)
        {
            prefab = ActionCardPrefab;
            if (type is DisplayType.NewCard)
                prefab = prefab.GetComponent<CardZoom>().ActionZoomCardPrefab;
        }

        GameObject parent = coMan.CardZone;
        if (parent == null) parent = uMan.CurrentCanvas;

        if (parent == null)
        {
            Debug.LogError("PARENT IS NULL!");
            return null;
        }

        prefab = Instantiate(prefab, parent.transform);
        prefab.transform.position = position;
        CardDisplay cd = prefab.GetComponent<CardDisplay>();

        if (type is DisplayType.Default)
        {
            cd.CardScript = card;
            cd.CardContainer = Instantiate(CardContainerPrefab, uMan.CurrentCanvas.transform);
            cd.CardContainer.transform.position = position;
            CardContainer cc = cd.CardContainer.GetComponent<CardContainer>();
            cc.Child = prefab;
        }
        else
        {
            Card newCard = NewCardInstance(card);
            if (type is DisplayType.HeroSelect) cd.CardScript = newCard;
            else if (type is DisplayType.NewCard) cd.DisplayZoomCard(null, newCard);
            else if (type is DisplayType.ChooseCard) cd.DisplayChooseCard(newCard);
            else if (type is DisplayType.Cardpage) cd.DisplayCardPageCard(newCard);
        }
        return prefab;
    }
    /******
     * *****
     * ****** HIDE_CARD
     * *****
     *****/
    public Card HideCard(GameObject card)
    {
        card.GetComponent<CardZoom>().DestroyZoomPopups();
        Card cardScript = card.GetComponent<CardDisplay>().CardScript;
        Destroy(card.GetComponent<CardDisplay>().CardContainer);
        if (card != null) Destroy(card);
        return cardScript;
    }
    /******
     * *****
     * ****** DRAW_CARD
     * *****
     *****/
    public GameObject DrawCard(HeroManager hero,
        Card drawnCard = null, List<Effect> additionalEffects = null)
    {
        List<Card> deck = hero.CurrentDeck;
        List<GameObject> hand = hero.HandZoneCards;
        string cardTag;
        string cardZone;
        Vector2 position = new Vector2();

        if (hero == pMan)
        {
            if (hand.Count >= GameManager.MAX_HAND_SIZE)
            {
                uMan.CreateFleetingInfoPopup("Your hand is full!");
                Debug.LogWarning("PLAYER HAND IS FULL!");
                return null;
            }
            cardTag = PLAYER_CARD;
            cardZone = CombatManager.PLAYER_HAND;

            if (drawnCard == null) position.Set(-780, -427);
            else position.Set(0, -350);
        }
        else if (hero == enMan)
        {
            if (hand.Count >= GameManager.MAX_HAND_SIZE)
            {
                uMan.CreateFleetingInfoPopup("Enemy hand is full!");
                Debug.LogWarning("ENEMY HAND IS FULL!");
                return null;
            }
            cardTag = ENEMY_CARD;
            cardZone = CombatManager.ENEMY_HAND;

            if (drawnCard == null) position.Set(780, 427);
            else position.Set(0, 350);
        }
        else
        {
            Debug.LogError("PLAYER <" + hero + "> NOT FOUND!");
            return null;
        }

        // Shuffle discard into deck
        if (drawnCard == null && deck.Count < 1)
        {
            List<Card> discard;
            if (hero == pMan) discard = pMan.DiscardZoneCards;
            else if (hero == enMan) discard = enMan.DiscardZoneCards;
            else
            {
                Debug.LogError("PLAYER <" + hero + "> NOT FOUND!");
                return null;
            }
            if (discard.Count < 1)
            {
                Debug.LogError("DISCARD IS EMPTY!");
                return null;
            }
            foreach (Card c in discard) deck.Add(c);
            discard.Clear();
            ShuffleDeck(hero);
        }

        GameObject card;
        if (drawnCard == null)
        {
            auMan.StartStopSound("SFX_DrawCard");
            card = ShowCard(deck[0], position);
        }
        else
        {
            auMan.StartStopSound("SFX_CreateCard");
            card = ShowCard(drawnCard, position, DisplayType.Default, true);
        }

        if (card == null)
        {
            Debug.LogError("CARD IS NULL!");
            return null;
        }

        if (drawnCard == null) deck.RemoveAt(0);
        card.tag = cardTag;
        ChangeCardZone(card, cardZone);
        anMan.CreateParticleSystem(card, ParticleSystemHandler.ParticlesType.Drag, 1);

        hero.HandZoneCards.Add(card);

        if (additionalEffects != null)
        {
            foreach (Effect addEffect in additionalEffects)
                efMan.ResolveEffect(new List<GameObject> { card }, addEffect, false, 0, out _, false);
        }

        return card;
    }
    /******
     * *****
     * ****** CHANGE_CARD_ZONE
     * *****
     *****/
    public void ChangeCardZone(GameObject card, string newZoneName, bool returnToIndex = false, bool changeControl = false)
    {
        CardDisplay cd = card.GetComponent<CardDisplay>();
        DragDrop dd = card.GetComponent<DragDrop>();
        CardContainer container = cd.CardContainer.GetComponent<CardContainer>();
        System.Action action;

        GameObject newZone = null;
        bool isPlayed = true;
        bool wasPlayed = dd.IsPlayed; // For cards returned to hand

        uMan.SelectTarget(card, UIManager.SelectionType.Disabled); // Unnecessary?

        switch (newZoneName)
        {
            // PLAYER ZONES
            case CombatManager.PLAYER_HAND:
                newZone = pMan.HandZone;
                action = () => anMan.RevealedHandState(card);
                isPlayed = false;
                break;
            case CombatManager.PLAYER_ZONE:
                newZone = pMan.PlayZone;
                action = () => anMan.PlayedUnitState(card);
                break;
            case CombatManager.PLAYER_ACTION_ZONE:
                newZone = pMan.ActionZone;
                action = () => anMan.PlayedActionState(card);
                break;
            // ENEMY ZONES
            case CombatManager.ENEMY_HAND:
                newZone = enMan.HandZone;
                action = () => anMan.HiddenHandState(card);
                isPlayed = false;
                break;
            case CombatManager.ENEMY_ZONE:
                newZone = enMan.PlayZone;
                action = () => anMan.PlayedUnitState(card);
                break;
            case CombatManager.ENEMY_ACTION_ZONE:
                newZone = enMan.ActionZone;
                action = () => anMan.PlayedActionState(card);
                break;
            default:
                Debug.LogError("INVALID ZONE!");
                return;
        }

        container.OnAttachAction += () => action();

        if (!returnToIndex)
        {
            lastCardIndex = dd.LastIndex;
            lastContainerIndex = cd.CardContainer.transform.GetSiblingIndex();

            if (newZoneName == CombatManager.ENEMY_HAND) card.transform.SetAsFirstSibling();
            else if (newZoneName == CombatManager.PLAYER_HAND) card.transform.SetAsLastSibling();
        }

        cd.CardContainer.GetComponent<CardContainer>().MoveContainer(newZone);

        if (changeControl) dd.IsPlayed = true;
        else if (returnToIndex)
        {
            card.transform.SetSiblingIndex(lastCardIndex);
            cd.CardContainer.transform.SetSiblingIndex(lastContainerIndex);
            dd.IsPlayed = isPlayed;
        }
        else if (!isPlayed) // => PlayerHand OR EnemyHand
        {
            if (wasPlayed) // For ReturnCard effects (Play => Hand)
            {
                cd.ResetCard();
                dd.IsPlayed = false;
            }
            efMan.ApplyChangeNextCostEffects(card);
        }

        if (cd is UnitCardDisplay ucd)
        {
            bool isExhausted = false;

            if (isPlayed)
            {
                if (!changeControl && !GetAbility(card, ABILITY_BLITZ)) isExhausted = true;
                ucd.EnableVFX();

            }
            else ucd.DisableVFX();
            ucd.IsExhausted = isExhausted;

            container.OnAttachAction += () => FunctionTimer.Create(() => SetStats(card), 0.1f);

            void SetStats(GameObject unitCard)
            {
                if (unitCard == null) return;
                anMan.UnitStatChangeState(card, 0, 0, false, true);
            }
        }

    }
    /******
     * *****
     * ****** IS_PLAYABLE
     * *****
     *****/
    public bool IsPlayable(GameObject card, bool isPrecheck = false, bool ignoreCost = false)
    {
        if (card == null)
        {
            Debug.LogError("CARD IS NULL!");
            return false;
        }

        bool isPlayerCard;
        if (card.CompareTag(PLAYER_CARD)) isPlayerCard = true;
        else if (card.CompareTag(ENEMY_CARD)) isPlayerCard = false;
        else
        {
            Debug.LogError("INVALID CARD TAG!");
            return false;
        }

        CardDisplay cardDisplay = card.GetComponent<CardDisplay>();
        if (cardDisplay is UnitCardDisplay)
        {
            List<GameObject> zoneCards;
            string errorMessage;
            if (isPlayerCard)
            {
                zoneCards = pMan.PlayZoneCards;
                errorMessage = "You can't play more units!";
            }
            else
            {
                zoneCards = enMan.PlayZoneCards;
                errorMessage = "Enemy can't play more units!";

            }
            if (zoneCards.Count >= GameManager.MAX_UNITS_PLAYED)
            {
                if (isPrecheck) return false;
                uMan.CreateFleetingInfoPopup(errorMessage);
                ErrorSound();
                return false;
            }
        }
        else if (cardDisplay is ActionCardDisplay acd)
        {
            if (!efMan.CheckLegalTargets(acd.ActionCard.EffectGroupList, card, true))
            {
                if (isPrecheck) return false;
                uMan.CreateFleetingInfoPopup("You can't play that right now!");
                ErrorSound();
                return false;
            }
        }

        if (!ignoreCost)
        {
            int energyLeft;
            if (isPlayerCard) energyLeft = pMan.CurrentEnergy;
            else energyLeft = enMan.CurrentEnergy;

            if (energyLeft < cardDisplay.CurrentEnergyCost)
            {
                if (isPrecheck) return false;
                uMan.CreateFleetingInfoPopup("Not enough energy!");
                ErrorSound();
                return false;
            }
        }

        return true;
        void ErrorSound() => auMan.StartStopSound("SFX_Error");
    }
    /******
     * *****
     * ****** PLAY_CARD [HAND >>> PLAY]
     * *****
     *****/
    public void PlayCard(GameObject card)
    {
        CardDisplay cd = card.GetComponent<CardDisplay>();
        CardContainer container = cd.CardContainer.GetComponent<CardContainer>();

        evMan.PauseDelayedActions(true);
        container.OnAttachAction += () => evMan.PauseDelayedActions(false);

        // PLAYER
        if (card.CompareTag(PLAYER_CARD))
        {
            pMan.HandZoneCards.Remove(card);

            int energyLeft = pMan.CurrentEnergy;
            pMan.CurrentEnergy -= cd.CurrentEnergyCost;
            int energyChange = pMan.CurrentEnergy - energyLeft;

            if (energyChange != 0)
                anMan.ModifyHeroEnergyState(energyChange, pMan.HeroObject, false);

            evMan.NewDelayedAction(() => SelectPlayableCards(), 0, true);

            if (cd is UnitCardDisplay)
            {
                pMan.PlayZoneCards.Add(card);
                ChangeCardZone(card, CombatManager.PLAYER_ZONE);
                evMan.NewDelayedAction(() => PlayUnit(), 0, true);
            }
            else if (cd is ActionCardDisplay)
            {
                pMan.ActionZoneCards.Add(card);
                ChangeCardZone(card, CombatManager.PLAYER_ACTION_ZONE);
                evMan.NewDelayedAction(() => PlayAction(), 0, true);
            }
            else
            {
                Debug.LogError("INVALID TYPE!");
                return;
            }
        }

        // ENEMY
        else if (card.CompareTag(ENEMY_CARD))
        {
            if (cd is UnitCardDisplay)
            {
                if (enMan.PlayZoneCards.Count >= GameManager.MAX_UNITS_PLAYED)
                {
                    Debug.LogError("TOO MANY ENEMIES!");
                    return;
                }

                enMan.PlayZoneCards.Add(card);
                ChangeCardZone(card, CombatManager.ENEMY_ZONE);
                evMan.NewDelayedAction(() => PlayUnit(), 0, true);
            }
            else if (cd is ActionCardDisplay)
            {
                enMan.ActionZoneCards.Add(card);
                ChangeCardZone(card, CombatManager.ENEMY_ACTION_ZONE);
                evMan.NewDelayedAction(() => PlayAction(), 1, true);
            }
            else
            {
                Debug.LogError("INVALID TYPE!");
                return;
            }

            card.GetComponent<DragDrop>().IsPlayed = true;
            enMan.HandZoneCards.Remove(card);

            int energyLeft = enMan.CurrentEnergy;
            enMan.CurrentEnergy -= cd.CurrentEnergyCost;
            int energyChange = enMan.CurrentEnergy - energyLeft;
            anMan.ModifyHeroEnergyState(energyChange, enMan.HeroObject, false);
        }
        else
        {
            Debug.LogError("INVALID TAG!");
            return;
        }

        void PlayUnit()
        {
            if (card == null)
            {
                Debug.LogError("CARD IS NULL!");
                return;
            }

            if (card.CompareTag(PLAYER_CARD))
            {
                if (!TriggerUnitAbility(card, TRIGGER_PLAY)) UnitTriggers();
            }
            else UnitTriggers();

            card.transform.SetAsFirstSibling();
            PlayCardSound();
            ParticleBurst();

            void UnitTriggers()
            {
                uMan.CombatLog_PlayCard(card);
                efMan.ResolveChangeNextCostEffects(card); // Resolves IMMEDIATELY

                TriggerTrapAbilities(card); // Resolves 3rd
                efMan.TriggerModifiers_PlayCard(card); // Resolves 2nd
                efMan.TriggerGiveNextEffects(card); // Resolves 1st
            }
        }
        void PlayAction()
        {
            if (card == null)
            {
                Debug.LogError("CARD IS NULL!");
                return;
            }

            auMan.StartStopSound("SFX_PlayCard");
            ResolveActionCard(card); // Resolves IMMEDIATELY
            ParticleBurst();
        }
        void PlayCardSound()
        {
            Sound playSound = cd.CardScript.CardPlaySound;
            if (playSound.clip == null) Debug.LogWarning("MISSING PLAY SOUND: " + cd.CardName);
            else auMan.StartStopSound(null, playSound);
        }

        void ParticleBurst() =>
            anMan.CreateParticleSystem(card, ParticleSystemHandler.ParticlesType.Play, 1);
    }

    /******
     * *****
     * ****** DISCARD_CARD [HAND/ACTION_ZONE >>> DISCARD]
     * *****
     *****/
    public void DiscardCard(GameObject card, bool isAction = false)
    {
        CardDisplay cd = card.GetComponent<CardDisplay>();
        List<GameObject> previousZone;
        List<Card> newZone;
        HeroManager hMan = CombatManager.GetSourceHero(card);

        if (isAction) previousZone = hMan.ActionZoneCards;
        else previousZone = hMan.HandZoneCards;
        newZone = hMan.DiscardZoneCards;

        previousZone.Remove(card);
        if (cd.CardScript.BanishAfterPlay) HideCard(card);
        else
        {
            cd.ResetCard();
            newZone.Add(HideCard(card));
        }

        if (!isAction) auMan.StartStopSound("SFX_DiscardCard");
    }

    /******
     * *****
     * ****** RESOLVE_ACTION_CARD
     * *****
     *****/
    private void ResolveActionCard(GameObject card)
    {
        List<EffectGroup> groupList =
            card.GetComponent<ActionCardDisplay>().ActionCard.EffectGroupList;

        efMan.StartEffectGroupList(groupList, card);
    }

    /******
     * *****
     * ****** SELECT_PLAYABLE_CARDS
     * *****
     *****/
    public void SelectPlayableCards(bool setAllFalse = false)
    {
        evMan.NewDelayedAction(() => SelectCards(), 0, true);

        void SelectCards()
        {
            int playableCards = 0;

            bool isPlayerTurn;
            if (setAllFalse) isPlayerTurn = false;
            else isPlayerTurn = pMan.IsMyTurn;

            foreach (GameObject card in pMan.HandZoneCards)
            {
                if (card == null)
                {
                    Debug.LogError("CARD IS NULL!");
                    continue;
                }

                if (isPlayerTurn && IsPlayable(card, true))
                {
                    playableCards++;
                    uMan.SelectTarget(card, UIManager.SelectionType.Playable);
                }
                else uMan.SelectTarget(card, UIManager.SelectionType.Disabled);
            }

            bool playerHasActions;
            if (playableCards < 1) playerHasActions = false;
            else playerHasActions = true;

            foreach (GameObject ally in pMan.PlayZoneCards)
            {
                if (isPlayerTurn && coMan.CanAttack(ally, null, true, true))
                {
                    uMan.SelectTarget(ally, UIManager.SelectionType.Playable);
                    playerHasActions = true;
                }
                else uMan.SelectTarget(ally, UIManager.SelectionType.Disabled);
            }

            if (!playerHasActions)
            {
                if (pMan.UseHeroPower(false, true) ||
                    pMan.UseHeroPower(true, true)) playerHasActions = true;
            }

            if (setAllFalse) playerHasActions = false;
            uMan.SetReadyEndTurnButton(!playerHasActions);
        }
    }

    /******
     * *****
     * ****** CHANGE_UNIT_CONTROL
     * *****
     *****/
    public void ChangeUnitControl(GameObject card)
    {
        if (card.CompareTag(PLAYER_CARD))
        {
            if (enMan.PlayZoneCards.Count >= GameManager.MAX_UNITS_PLAYED)
            {
                Debug.LogWarning("TOO MANY ENEMY UNITS!");
                coMan.DestroyUnit(card);
                return;
            }

            pMan.PlayZoneCards.Remove(card);
            card.tag = ENEMY_CARD;
            ChangeCardZone(card, CombatManager.ENEMY_ZONE, false, true);
            enMan.PlayZoneCards.Add(card);
        }
        else if (card.CompareTag(ENEMY_CARD))
        {
            if (pMan.PlayZoneCards.Count >= GameManager.MAX_UNITS_PLAYED)
            {
                Debug.LogWarning("TOO MANY PLAYER UNITS!");
                coMan.DestroyUnit(card);
                return;
            }

            enMan.PlayZoneCards.Remove(card);
            card.tag = PLAYER_CARD;
            ChangeCardZone(card, CombatManager.PLAYER_ZONE, false, true);
            pMan.PlayZoneCards.Add(card);
        }
        else Debug.LogError("INVALID CARD TAG!");
    }

    public string FilterCreatedCardProgress(string text, bool isPlayerSource)
    {
        HeroManager hMan;
        if (isPlayerSource) hMan = pMan;
        else hMan = enMan;

        text = text.Replace("{EXPLOITS}", $"{hMan.ExploitsPlayed}");
        text = text.Replace("{INVENTIONS}", $"{hMan.InventionsPlayed}");
        text = text.Replace("{SCHEMES}", $"{hMan.SchemesPlayed}");
        text = text.Replace("{EXTRACTIONS}", $"{hMan.ExtractionsPlayed}");
        return text;
    }

    public string FilterKeywords(string text)
    {
        foreach (string s in AbilityKeywords)
            text = text.Replace(s, "<color=\"yellow\"><b>" + s + "</b></color>");
        foreach (string s in CardTypes)
            text = text.Replace(s, "<color=\"green\"><b>" + s + "</b></color>");

        text = FilterUnitTypes(text);
        return text;
    }

    public string FilterUnitTypes(string text)
    {
        string[] unitTypes =
        {
            MAGE + "s",
            MAGE,
            MUTANT + "s",
            MUTANT,
            ROGUE + "s",
            ROGUE,
            TECH + "s",
            TECH,
            WARRIOR + "s",
            WARRIOR
        };

        foreach (string s in unitTypes) text = ReplaceText(s);

        return text;

        string ReplaceText(string target)
        {
            text = text.Replace(target, $"<color=\"green\"><b>{target}</b></color>");
            return text;
        }
    }

    public Color GetAbilityColor(CardAbility cardAbility)
    {
        if (cardAbility.OverrideColor) return cardAbility.AbilityColor;

        if (cardAbility is TriggeredAbility ||
            cardAbility is ModifierAbility) return Color.yellow;

        foreach (string posAbi in PositiveAbilities)
        {
            if (cardAbility.AbilityName == posAbi) return Color.green;
        }

        foreach (string negAbi in NegativeAbilities)
        {
            if (cardAbility.AbilityName == negAbi) return Color.red;
        }

        return Color.yellow;
    }

    public void LoadNewActions()
    {
        ActionCard[] allActions = Resources.LoadAll<ActionCard>("Cards_Actions");
        allActions.Shuffle();
        List<ActionCard> actionList = new List<ActionCard>();

        foreach (ActionCard action in allActions)
        {
            // Card Rarity Functionality
            switch (action.CardRarity)
            {
                case Card.Rarity.Common:
                    AddCard();
                    AddCard();
                    AddCard();
                    goto case Card.Rarity.Rare;
                case Card.Rarity.Rare:
                    AddCard();
                    AddCard();
                    goto case Card.Rarity.Legend;
                case Card.Rarity.Legend:
                    AddCard();
                    break;
            }
            void AddCard() => actionList.Add(action);
        }

        while (ActionShopCards.Count < 8)
        {
            foreach (ActionCard ac in actionList)
            {
                if (ac == null) continue;
                int index = pMan.DeckList.FindIndex(x => x.CardName == ac.CardName);
                if (index == -1 && !ActionShopCards.Contains(ac))
                {
                    ActionShopCards.Add(ac);
                    if (ActionShopCards.Count > 7) break;
                }
            }
        }
    }
    public void LoadNewRecruits()
    {
        UnitCard[] allRecruits = Resources.LoadAll<UnitCard>("Cards_Units");
        allRecruits.Shuffle();

        List<UnitCard> recruitMages = new List<UnitCard>();
        List<UnitCard> recruitMutants = new List<UnitCard>();
        List<UnitCard> recruitRogues = new List<UnitCard>();
        List<UnitCard> recruitTechs = new List<UnitCard>();
        List<UnitCard> recruitWarriors = new List<UnitCard>();

        foreach (UnitCard unitCard in allRecruits)
        {
            List<UnitCard> targetList;
            switch (unitCard.CardType)
            {
                case MAGE:
                    targetList = recruitMages;
                    break;
                case MUTANT:
                    targetList = recruitMutants;
                    break;
                case ROGUE:
                    targetList = recruitRogues;
                    break;
                case TECH:
                    targetList = recruitTechs;
                    break;
                case WARRIOR:
                    targetList = recruitWarriors;
                    break;
                default:
                    Debug.LogError($"CARD TYPE NOT FOUND FOR <{unitCard.CardName}>");
                    return;
            }

            // Card Rarity Functionality
            switch (unitCard.CardRarity)
            {
                case Card.Rarity.Common:
                    AddCard();
                    AddCard();
                    AddCard();
                    goto case Card.Rarity.Rare;
                case Card.Rarity.Rare:
                    AddCard();
                    AddCard();
                    goto case Card.Rarity.Legend;
                case Card.Rarity.Legend:
                    AddCard();
                    break;
            }
            void AddCard() => targetList.Add(unitCard);
        }

        List<List<UnitCard>> recruitLists = new List<List<UnitCard>>
        {
            recruitMages,
            recruitMutants,
            recruitRogues,
            recruitTechs,
            recruitWarriors
        };

        while (PlayerRecruitUnits.Count < 8)
        {
            foreach (List<UnitCard> list in recruitLists)
            {
                foreach (UnitCard uc in list)
                {
                    if (uc == null) continue;
                    int index = pMan.DeckList.FindIndex(x => x.CardName == uc.CardName);
                    if (index == -1 && !PlayerRecruitUnits.Contains(uc))
                    {
                        PlayerRecruitUnits.Add(uc);
                        if (PlayerRecruitUnits.Count > 7) goto FinishRecruits;
                        break;
                    }
                }
            }
        FinishRecruits:;
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
                chooseCardType = "Cards_Actions";
                break;
            case ChooseCard.Unit:
                chooseCardType = "Cards_Units";
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

        // Card Rarity Functionality
        List<Card> cardPool = new List<Card>();
        foreach (Card card in allChooseCards)
        {
            switch (card.CardRarity)
            {
                case Card.Rarity.Common:
                    AddCard();
                    AddCard();
                    AddCard();
                    goto case Card.Rarity.Rare;
                case Card.Rarity.Rare:
                    AddCard();
                    AddCard();
                    goto case Card.Rarity.Legend;
                case Card.Rarity.Legend:
                    AddCard();
                    break;
            }
            void AddCard() => cardPool.Add(card);
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
                if (chooseCards.Contains(card)) continue;

                int otherCopies = 0;
                if (limitDuplicates)
                {
                    foreach (Card playerCard in pMan.DeckList)
                    {
                        if (playerCard.CardName == card.CardName)
                            otherCopies++;
                    }
                }

                if (!limitDuplicates || otherCopies < 1)
                {
                    chooseCards[index++] = card;
                    if (index == 3) break;
                }
            }
        }
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
        if (hero == GameManager.PLAYER) deck = PlayerManager.Instance.DeckList;
        else if (hero == GameManager.ENEMY) deck = EnemyManager.Instance.DeckList;
        else
        {
            Debug.LogError("INVALID HERO TAG!");
            return;
        }
        if (card is UnitCard) cardInstance = ScriptableObject.CreateInstance<UnitCard>();
        else if (card is ActionCard) cardInstance = ScriptableObject.CreateInstance<ActionCard>();
        else
        {
            Debug.LogError("INVALID CARD TYPE!");
            return;
        }
        cardInstance.LoadCard(card);
        deck.Add(cardInstance);
        if (hero == GameManager.PLAYER && newCard) UnitReputationChange(card, false);
    }
    public void RemovePlayerCard(Card card)
    {
        PlayerManager.Instance.DeckList.Remove(card);
        UnitReputationChange(card, true);
    }

    private void UnitReputationChange(Card card, bool isRemoval)
    {
        if (!(card is UnitCard)) return;

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
    public void ShuffleDeck(HeroManager hero, bool playSound = true)
    {
        if (hero == pMan) pMan.CurrentDeck.Shuffle();
        else if (hero == enMan)
        {
            enMan.CurrentDeck.Shuffle();
            playSound = false;
        }
        else
        {
            Debug.LogError("INVALID HERO!");
            return;
        }

        if (playSound) auMan.StartStopSound("SFX_ShuffleDeck");
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
            pMan.CurrentDeck.Clear();
            foreach (Card card in pMan.DeckList)
                pMan.CurrentDeck.Add(NewCardInstance(card));
            pMan.CurrentDeck.Shuffle();
        }
        else if (hero == GameManager.ENEMY)
        {
            enMan.CurrentDeck.Clear();
            foreach (Card card in enMan.DeckList)
                enMan.CurrentDeck.Add(NewCardInstance(card));
            enMan.CurrentDeck.Shuffle();
        }
        else
        {
            Debug.LogError("HERO NOT FOUND!");
            return;
        }
    }

    /******
     * *****
     * ****** GET_SPECIAL_TRIGGER
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
            Debug.LogError("TARGET IS NOT UNIT!");
            return false;
        }

        int abilityIndex = ucd.CurrentAbilities.FindIndex(x => x.AbilityName == ability);
        if (abilityIndex == -1) return false;
        else return true;
    }
    public int GetPositiveKeywords(GameObject unitCard)
    {
        if (unitCard == null)
        {
            Debug.LogError("CARD IS NULL!");
            return 0;
        }
        if (!unitCard.TryGetComponent(out UnitCardDisplay _))
        {
            Debug.LogError("TARGET IS NOT UNIT!");
            return 0;
        }

        int positiveKeywords = 0;
        foreach (string positiveKeyword in PositiveAbilities)
        {
            if (GetAbility(unitCard, positiveKeyword)) positiveKeywords++;
        }

        return positiveKeywords;
    }
    /******
     * *****
     * ****** GET_TRIGGER
     * *****
     *****/
    public static bool GetTrigger(GameObject unitCard, string triggerName)
    {
        if (unitCard == null)
        {
            Debug.LogError("CARD IS NULL!");
            return false;
        }
        if (!unitCard.TryGetComponent(out UnitCardDisplay ucd))
        {
            Debug.LogError("TARGET IS NOT UNIT!");
            return false;
        }

        foreach (CardAbility ca in ucd.CurrentAbilities)
            if (ca is TriggeredAbility tra)
                if (tra.AbilityTrigger.AbilityName == triggerName)
                {
                    if (tra.TriggerLimit != 0 && tra.TriggerCount >= tra.TriggerLimit) continue;
                    return true;
                }
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
        int totalAbilities = 0;
        List<TriggeredAbility> resolveFirstAbilities = new List<TriggeredAbility>();
        List<TriggeredAbility> resolveSecondAbilities = new List<TriggeredAbility>();
        List<TriggeredAbility> resolveLastAbilities = new List<TriggeredAbility>();
        
        foreach (CardAbility ca in ucd.CurrentAbilities.AsEnumerable().Reverse()) // Resolve abilities in top-down order
            if (ca is TriggeredAbility tra)
                if (tra.AbilityTrigger.AbilityName == triggerName)
                {
                    if (tra.TriggerLimit != 0 && tra.TriggerCount >= tra.TriggerLimit) continue;
                    tra.TriggerCount++;

                    Debug.Log("TRIGGER! <" + triggerName + ">");
                    effectFound = true;
                    int additionalTriggers =
                        efMan.TriggerModifiers_TriggerAbility(triggerName, unitCard);
                    int totalTriggers = 1 + additionalTriggers;

                    List<TriggeredAbility> targetList;
                    if (IsResolveLastAbility(tra)) targetList = resolveLastAbilities;
                    else if (IsResolveSecondAbility(tra)) targetList = resolveSecondAbilities;
                    else targetList = resolveFirstAbilities;

                    for (int i = 0; i < totalTriggers; i++)
                    {
                        targetList.Add(tra);
                        totalAbilities++;
                    }
                }

        List<string> enabledTriggers = new List<string>();
        List<string> visibleTriggers = new List<string>();

        foreach (CardAbility ca in ucd.CurrentAbilities)
            if (ca is TriggeredAbility tra)
            {
                if (tra.TriggerLimit != 0 && tra.TriggerCount >= tra.TriggerLimit) { }
                else enabledTriggers.Add(tra.AbilityTrigger.AbilityName);
            }

        foreach (CardAbility ca in ucd.CurrentAbilities)
            if (ca is TriggeredAbility tra)
            {
                if (enabledTriggers.FindIndex(x => x == tra.AbilityTrigger.AbilityName) == -1)
                {
                    ucd.EnableTriggerIcon(tra.AbilityTrigger, false);
                }
            }

        float delay = 0.25f;
        int currentAbility = 0;
        ResolveAbilities(resolveLastAbilities);
        ResolveAbilities(resolveSecondAbilities);
        ResolveAbilities(resolveFirstAbilities);
        return effectFound;

        void ResolveAbilities(List<TriggeredAbility> abilities)
        {
            if (++currentAbility == totalAbilities) delay = 0;

            foreach (TriggeredAbility tra in abilities)
            {
                evMan.NewDelayedAction(() =>
                TriggerAbility(tra), delay, true);
            }
        }
        bool IsResolveSecondAbility(TriggeredAbility tra)
        {
            foreach (EffectGroup eg in tra.EffectGroupList)
            {
                foreach (Effect e in eg.Effects)
                {
                    if (eg.Targets.TargetsSelf &&
                        (e is DamageEffect || e is DestroyEffect)) return true;
                }
            }
            return false;
        }
        bool IsResolveLastAbility(TriggeredAbility tra)
        {
            foreach (EffectGroup eg in tra.EffectGroupList)
            {
                foreach (Effect e in eg.Effects)
                {
                    if (eg.Targets.TargetsSelf &&
                        e is ChangeControlEffect) return true;
                }
            }
            return false;
        }

        void TriggerAbility(TriggeredAbility tra)
        {
            if (unitCard == null)
            {
                Debug.LogWarning("UNIT IS NULL!");
                return;
            }

            efMan.StartEffectGroupList(tra.EffectGroupList, unitCard, triggerName);
        }
    }
    /******
     * *****
     * ****** TRIGGER_PLAYED_UNITS
     * *****
     *****/
    public void TriggerPlayedUnits(string triggerName, string player)
    {
        List<GameObject> cardZone;
        if (player == GameManager.PLAYER) cardZone = pMan.PlayZoneCards;
        else cardZone = enMan.PlayZoneCards;

        foreach (GameObject unit in cardZone.AsEnumerable().Reverse()) // Trigger units in played order
        {
            UnitCardDisplay ucd = unit.GetComponent<UnitCardDisplay>();

            if (triggerName == TRIGGER_TURN_START)
            {
                int poisonValue = 0;
                foreach (CardAbility ca in ucd.CurrentAbilities)
                    if (ca.AbilityName == ABILITY_POISONED) poisonValue++;

                if (poisonValue > 0) evMan.NewDelayedAction(() =>
                SchedulePoisonEffect(unit, poisonValue), 0, true);

            }

            if (triggerName == TRIGGER_TURN_END)
            {
                foreach (CardAbility ca in ucd.CurrentAbilities)
                    if (ca.AbilityName == ABILITY_REGENERATION)
                    {
                        evMan.NewDelayedAction(() =>
                        ScheduleRegenerationEffect(unit), 0, true);
                        break;
                    }
            }

            if (!GetTrigger(unit, triggerName)) continue;

            evMan.NewDelayedAction(() =>
            TriggerUnitAbility(unit, triggerName), 0.25f, true);
        }

        if (player == GameManager.ENEMY)
        {
            EnemyHeroPower ehp = enMan.HeroScript.HeroPower as EnemyHeroPower;
            if (ehp != null && ehp.PowerTrigger.AbilityName == triggerName)
                evMan.NewDelayedAction(() =>
                enMan.UseHeroPower(), 0.5f, true);
        }

        void ScheduleRegenerationEffect(GameObject unit)
        {
            if (unit != null)
            {
                UnitCardDisplay ucd = unit.GetComponent<UnitCardDisplay>();
                if (ucd.CurrentHealth > 0 && ucd.CurrentHealth < ucd.MaxHealth)
                    evMan.NewDelayedAction(() => RegenerationEffect(unit), 0.5f, true);
            }
        }
        void SchedulePoisonEffect(GameObject unit, int poisonValue)
        {
            if (unit != null)
            {
                UnitCardDisplay ucd = unit.GetComponent<UnitCardDisplay>();
                if (ucd.CurrentHealth > 0)
                    evMan.NewDelayedAction(() =>
                    PoisonEffect(unit, poisonValue), 0.5f, true);
            }
        }
        void RegenerationEffect(GameObject unit)
        {
            HealEffect healEffect = ScriptableObject.CreateInstance<HealEffect>();
            healEffect.HealFully = true;

            UnitCardDisplay ucd = unit.GetComponent<UnitCardDisplay>();
            ucd.AbilityTriggerState(ABILITY_REGENERATION);

            efMan.ResolveEffect(new List<GameObject> { unit },
                healEffect, false, 0, out _, false);
        }
        void PoisonEffect(GameObject unit, int poisonValue)
        {
            DamageEffect damageEffect = ScriptableObject.CreateInstance<DamageEffect>();
            damageEffect.Value = poisonValue;

            UnitCardDisplay ucd = unit.GetComponent<UnitCardDisplay>();
            ucd.AbilityTriggerState(ABILITY_POISONED);

            efMan.ResolveEffect(new List<GameObject> { unit },
                damageEffect, false, 0, out _, false);
        }
    }
    /******
     * *****
     * ****** TRIGGER_TRAP_ABILITIES
     * *****
     *****/
    public void TriggerTrapAbilities(GameObject trappedUnit)
    {
        if (trappedUnit == null || GetAbility(trappedUnit, ABILITY_WARD)) return;

        List<GameObject> enemyZoneCards =
            CombatManager.GetSourceHero(trappedUnit, true).PlayZoneCards;
        List<GameObject> resolveFirstTraps = new List<GameObject>();

        foreach (GameObject trap in enemyZoneCards) // Trigger order doesn't matter, is handled manually
        {
            if (efMan.UnitsToDestroy.Contains(trap)) continue;

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
                    efMan.TriggerModifiers_SpecialTrigger(ModifierAbility.TriggerType.AllyTrapDestroyed, enemyZoneCards);

                    evMan.NewDelayedAction(() => efMan.ClearDestroyedUnits(), 0, true);

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
    #endregion
    #endregion
}