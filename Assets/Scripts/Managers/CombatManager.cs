using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static CombatManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    [SerializeField] private GameObject dragArrowPrefab;
    [SerializeField] private GameObject cardContainerPrefab;

    private GameManager gMan;
    private CardManager caMan;
    private AudioManager auMan;
    private EffectManager efMan;
    private EventManager evMan;
    private UIManager uMan;
    private AnimationManager anMan;
    private PlayerManager pMan;
    private EnemyManager enMan;
    private readonly string PLAYER = GameManager.PLAYER;
    private readonly string ENEMY = GameManager.ENEMY;

    private int actionsPlayedThisTurn;
    private int lastCardIndex;
    private int lastContainerIndex;

    // ALL CARDS
    public const string CARD_ZONE = "CardZone";

    // PLAYER
    public const string PLAYER_CARD = "PlayerCard";
    public const string PLAYER_HERO = "PlayerHero";
    public const string PLAYER_HAND = "PlayerHand";
    public const string PLAYER_ZONE = "PlayerZone";
    public const string PLAYER_ACTION_ZONE = "PlayerActionZone";
    public const string PLAYER_DISCARD = "PlayerDiscard";
    public const string HERO_POWER = "HeroPower";
    public const string HERO_ULTIMATE = "HeroUltimate";

    // ENEMY
    public const string ENEMY_CARD = "EnemyCard";
    public const string ENEMY_HERO = "EnemyHero";
    public const string ENEMY_HAND = "EnemyHand";
    public const string ENEMY_ZONE = "EnemyZone";
    public const string ENEMY_ACTION_ZONE = "EnemyActionZone";
    public const string ENEMY_DISCARD = "EnemyDiscard";
    public const string ENEMY_HERO_POWER = "EnemyHeroPower";

    public GameObject DragArrowPrefab { get => dragArrowPrefab; }
    public int ActionsPlayedThisTurn
    {
        get => actionsPlayedThisTurn;
        set
        {
            actionsPlayedThisTurn = value;
            if (actionsPlayedThisTurn == 1)
            {
                string player;
                if (pMan.IsMyTurn) player = GameManager.PLAYER;
                else player = GameManager.ENEMY;

                evMan.NewDelayedAction(() =>
                caMan.TriggerPlayedUnits(CardManager.TRIGGER_SPARK, player), 0, true); // TESTING
            }
        }
    }

    /* CARD LISTS */

    // PLAYER
    public List<GameObject> PlayerHandCards { get; private set; }
    public List<GameObject> PlayerZoneCards { get; private set; }
    public List<GameObject> PlayerActionZoneCards { get; private set; }
    public List<Card> PlayerDiscardCards { get; private set; }

    // ENEMY
    public List<GameObject> EnemyHandCards { get; private set; }
    public List<GameObject> EnemyZoneCards { get; private set; }
    public List<GameObject> EnemyActionZoneCards { get; private set; }
    public List<Card> EnemyDiscardCards { get; private set; }
    
    /* GAME ZONES */

    // ALL CARDS
    public GameObject CardZone { get; private set; }

    // PLAYER
    public GameObject PlayerHero { get; private set; }
    public GameObject PlayerHand { get; private set; }
    public Vector2 PlayerHandStart { get; private set; }
    public GameObject PlayerZone { get; private set; }
    public GameObject PlayerActionZone { get; private set; }
    public GameObject PlayerDiscard { get; private set; }

    // ENEMY
    public GameObject EnemyHero { get; private set; }
    public GameObject EnemyHand { get; private set; }
    public GameObject EnemyZone { get; private set; }
    public GameObject EnemyActionZone { get; private set; }
    public GameObject EnemyDiscard { get; private set; }

    private void Start()
    {
        gMan = GameManager.Instance;
        caMan = CardManager.Instance;
        auMan = AudioManager.Instance;
        efMan = EffectManager.Instance;
        evMan = EventManager.Instance;
        uMan = UIManager.Instance;
        anMan = AnimationManager.Instance;
        pMan = PlayerManager.Instance;
        enMan = EnemyManager.Instance;
    }

    /******
     * *****
     * ****** START_COMBAT_SCENE
     * *****
     *****/
    public void StartCombatScene()
    {
        // GAME ZONES
        // ALL CARDS
        CardZone = GameObject.Find(CARD_ZONE);
        // PLAYER
        PlayerHand = GameObject.Find(PLAYER_HAND);
        PlayerHandStart = PlayerHand.transform.position;
        PlayerZone = GameObject.Find(PLAYER_ZONE);
        PlayerActionZone = GameObject.Find(PLAYER_ACTION_ZONE);
        PlayerDiscard = GameObject.Find(PLAYER_DISCARD);
        PlayerHero = GameObject.Find(PLAYER_HERO);
        // ENEMY
        EnemyHand = GameObject.Find(ENEMY_HAND);
        EnemyZone = GameObject.Find(ENEMY_ZONE);
        EnemyActionZone = GameObject.Find(ENEMY_ACTION_ZONE);
        EnemyDiscard = GameObject.Find(ENEMY_DISCARD);
        EnemyHero = GameObject.Find(ENEMY_HERO);
        // ZONE LISTS
        // PLAYER
        PlayerHandCards = new List<GameObject>();
        PlayerZoneCards = new List<GameObject>();
        PlayerActionZoneCards = new List<GameObject>();
        PlayerDiscardCards = new List<Card>();
        // ENEMY
        EnemyHandCards = new List<GameObject>();
        EnemyZoneCards = new List<GameObject>();
        EnemyActionZoneCards = new List<GameObject>();
        EnemyDiscardCards = new List<Card>();
        uMan.SelectTarget(PlayerHero, UIManager.SelectionType.Disabled);
        uMan.SelectTarget(EnemyHero, UIManager.SelectionType.Disabled);
    }

    public UnitCardDisplay GetUnitDisplay(GameObject card)
    {
        if (card == null)
        {
            Debug.LogWarning("CARD IS NULL!");
            return null;
        }
        return card.GetComponent<CardDisplay>() as UnitCardDisplay;
    }

    public bool IsUnitCard(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("CARD IS NULL!");
            return false;
        }
        return target.TryGetComponent<UnitCardDisplay>(out _);
    }

    public bool IsActionCard(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("CARD IS NULL!");
            return false;
        }
        return target.TryGetComponent<ActionCardDisplay>(out _);
    }

    public enum DisplayType
    {
        Default,
        HeroSelect,
        NewCard,
        ChooseCard,
        Cardpage
    }

    /******
     * *****
     * ****** SHOW_CARD
     * *****
     *****/
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
            prefab = caMan.UnitCardPrefab;
            if (type is DisplayType.NewCard)
                prefab = prefab.GetComponent<CardZoom>().UnitZoomCardPrefab;
        }
        else if (card is ActionCard)
        {
            prefab = caMan.ActionCardPrefab;
            if (type is DisplayType.NewCard)
                prefab = prefab.GetComponent<CardZoom>().ActionZoomCardPrefab;
        }

        GameObject parent = CardZone;
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
            cd.CardContainer = Instantiate(cardContainerPrefab, uMan.CurrentCanvas.transform);
            cd.CardContainer.transform.position = position;
            CardContainer cc = cd.CardContainer.GetComponent<CardContainer>();
            cc.Child = prefab;
        }
        else
        {
            Card newCard = caMan.NewCardInstance(card);
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
    private Card HideCard(GameObject card)
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
    public GameObject DrawCard(string hero, Card drawnCard = null)
    {
        List<Card> deck;
        List<GameObject> hand;
        string cardTag;
        string cardZone;
        Vector2 position = new Vector2();

        if (hero == PLAYER)
        {
            deck = pMan.CurrentPlayerDeck;
            hand = PlayerHandCards;
            if (hand.Count >= GameManager.MAX_HAND_SIZE)
            {
                uMan.CreateFleetingInfoPopup("Your hand is full!");
                Debug.LogWarning("PLAYER HAND IS FULL!");
                return null;
            }
            cardTag = PLAYER_CARD;
            cardZone = PLAYER_HAND;

            if (drawnCard == null) position.Set(-850, -350);
            else position.Set(0, -350);
        }
        else if (hero == ENEMY)
        {
            deck = enMan.CurrentEnemyDeck;
            hand = EnemyHandCards;
            if (hand.Count >= GameManager.MAX_HAND_SIZE)
            {
                uMan.CreateFleetingInfoPopup("Enemy hand is full!");
                Debug.LogWarning("ENEMY HAND IS FULL!");
                return null;
            }
            cardTag = ENEMY_CARD;
            cardZone = ENEMY_HAND;
            
            if (drawnCard == null) position.Set(685, 370);
            else position.Set(0, 300);
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
            if (hero == PLAYER) discard = PlayerDiscardCards;
            else if (hero == ENEMY) discard = EnemyDiscardCards;
            else
            {
                Debug.LogError("PLAYER <" + hero + "> NOT FOUND!");
                return null;
            }
            if (discard.Count < 1)
            {
                Debug.LogWarning("DISCARD IS EMPTY!");
                uMan.CreateFleetingInfoPopup("No cards left!");
                return null;
            }
            foreach (Card c in discard) deck.Add(c);
            discard.Clear();
            caMan.ShuffleDeck(hero);
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

        efMan.NewDrawnCards.Add(card);
        if (hero == PLAYER) PlayerHandCards.Add(card);
        else EnemyHandCards.Add(card);
        return card;
    }

    /******
     * *****
     * ****** SELECT_PLAYABLE_CARDS
     * *****
     *****/
    public void SelectPlayableCards()
    {
        evMan.NewDelayedAction(() => SelectCards(), 0, true);

        void SelectCards()
        {
            int playableCards = 0;
            bool isPlayerTurn = pMan.IsMyTurn;

            foreach (GameObject card in PlayerHandCards)
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

            foreach (GameObject ally in PlayerZoneCards)
                if (!GetUnitDisplay(ally).IsExhausted) playerHasActions = true;

            uMan.SetActiveEndTurnButton(!playerHasActions);
        }
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
     * ****** CHANGE_CARD_CONTROL
     * *****
     *****/
    public void ChangeCardControl(GameObject card)
    {
        if (card.CompareTag(PLAYER_CARD))
        {
            PlayerZoneCards.Remove(card);

            if (EnemyZoneCards.Count >= GameManager.MAX_UNITS_PLAYED)
            {
                Debug.LogWarning("TOO MANY ENEMY UNITS!");
                HideCard(card);
                return;
            }

            card.tag = ENEMY_CARD;
            ChangeCardZone(card, ENEMY_ZONE, false, true);
            EnemyZoneCards.Add(card);
        }
        else if (card.CompareTag(ENEMY_CARD))
        {
            EnemyZoneCards.Remove(card);

            if (PlayerZoneCards.Count >= GameManager.MAX_UNITS_PLAYED)
            {
                Debug.LogWarning("TOO MANY PLAYER UNITS!");
                HideCard(card);
                return;
            }

            card.tag = PLAYER_CARD;
            ChangeCardZone(card, PLAYER_ZONE, false, true);
            PlayerZoneCards.Add(card);
        }
        else Debug.LogError("INVALID CARD TAG!");
    }

    /******
     * *****
     * ****** CHANGE_CARD_ZONE
     * *****
     *****/
    public void ChangeCardZone(GameObject card, string newZoneName, bool returnToIndex = false, bool changeControl = false)
    {
        uMan.SelectTarget(card, UIManager.SelectionType.Disabled); // Unnecessary?
        GameObject newZone = null;

        switch (newZoneName)
        {
            // PLAYER
            case PLAYER_HAND:
                newZone = PlayerHand;
                anMan.RevealedHandState(card);
                break;
            case PLAYER_ZONE:
                newZone = PlayerZone;
                anMan.PlayedState(card);
                break;
            case PLAYER_ACTION_ZONE:
                newZone = PlayerActionZone;
                anMan.RevealedHandState(card);
                break;
            // ENEMY
            case ENEMY_HAND:
                newZone = EnemyHand;
                anMan.RevealedHandState(card);
                break;
            case ENEMY_ZONE:
                newZone = EnemyZone;
                anMan.PlayedState(card);
                break;
            case ENEMY_ACTION_ZONE: // TESTING
                newZone = EnemyActionZone;
                anMan.RevealedHandState(card);
                break;
        }

        CardDisplay cd = card.GetComponent<CardDisplay>();
        if (!returnToIndex)
        {
            lastCardIndex = card.GetComponent<DragDrop>().LastIndex;
            lastContainerIndex = cd.CardContainer.transform.GetSiblingIndex();

            if (newZoneName == ENEMY_HAND) card.transform.SetAsFirstSibling();
            else card.transform.SetAsLastSibling();

        }

        cd.CardContainer.GetComponent<CardContainer>().MoveContainer(newZone);
        bool isPlayed = false;
        if (newZoneName == PLAYER_ZONE || newZoneName == PLAYER_ACTION_ZONE ||
            newZoneName == ENEMY_ZONE) isPlayed = true;

        if (changeControl) card.GetComponent<DragDrop>().IsPlayed = true;
        else if (returnToIndex)
        {
            card.transform.SetSiblingIndex(lastCardIndex);
            cd.CardContainer.transform.SetSiblingIndex(lastContainerIndex);
            card.GetComponent<DragDrop>().IsPlayed = isPlayed;
        }
        else if (!isPlayed)
        {
            cd.ResetCard(); // Unnecessary?
            if (newZoneName == PLAYER_HAND)
                efMan.ApplyChangeNextCostEffects(card);
        }

        if (cd is UnitCardDisplay ucd)
        {
            bool isExhausted = false;
            if (isPlayed && !changeControl &&
                !CardManager.GetAbility(card, CardManager.ABILITY_BLITZ)) isExhausted = true;
            ucd.IsExhausted = isExhausted;
        }

    }

    /******
     * *****
     * ****** IS_PLAYABLE
     * *****
     *****/
    public bool IsPlayable(GameObject card, bool isPrecheck = false)
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
                zoneCards = PlayerZoneCards;
                errorMessage = "You can't play more units!";
            }
            else
            {
                zoneCards = EnemyZoneCards;
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

        int energyLeft;
        if (isPlayerCard) energyLeft = pMan.EnergyLeft;
        else energyLeft = enMan.EnergyLeft;

        if (energyLeft < cardDisplay.CurrentEnergyCost)
        {
            if (isPrecheck) return false;
            uMan.CreateFleetingInfoPopup("Not enough energy!");
            ErrorSound();
            return false;
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
            PlayerHandCards.Remove(card);

            int energyLeft = pMan.EnergyLeft;
            pMan.EnergyLeft -= cd.CurrentEnergyCost;
            int energyChange = pMan.EnergyLeft - energyLeft;

            if (energyChange != 0)
                anMan.ModifyHeroEnergyState(energyChange, PlayerHero, false);

            if (cd is UnitCardDisplay)
            {
                PlayerZoneCards.Add(card);
                ChangeCardZone(card, PLAYER_ZONE);
                evMan.NewDelayedAction(() => PlayUnit(), 0);
            }
            else if (cd is ActionCardDisplay)
            {
                PlayerActionZoneCards.Add(card);
                ChangeCardZone(card, PLAYER_ACTION_ZONE);
                evMan.NewDelayedAction(() => PlayAction(), 0);
            }
            else
            {
                Debug.LogError("INVALID TYPE!");
                return;
            }

            evMan.NewDelayedAction(() => SelectPlayableCards(), 0);
        }

        // ENEMY
        else if (card.CompareTag(ENEMY_CARD))
        {
            if (cd is UnitCardDisplay)
            {
                if (EnemyZoneCards.Count >= GameManager.MAX_UNITS_PLAYED)
                {
                    Debug.LogWarning("TOO MANY ENEMIES!");
                    return;
                }

                EnemyZoneCards.Add(card);
                ChangeCardZone(card, ENEMY_ZONE);
                container.OnAttachAction += () => PlayUnit();
            }
            else if (cd is ActionCardDisplay)
            {
                EnemyActionZoneCards.Add(card);
                ChangeCardZone(card, ENEMY_ACTION_ZONE);
                container.OnAttachAction += () => PlayAction();
            }
            else
            {
                Debug.LogError("INVALID TYPE!");
                return;
            }

            card.GetComponent<DragDrop>().IsPlayed = true;
            EnemyHandCards.Remove(card);

            int energyLeft = enMan.EnergyLeft;
            enMan.EnergyLeft -= cd.CurrentEnergyCost;
            int energyChange = enMan.EnergyLeft - energyLeft;
            anMan.ModifyHeroEnergyState(energyChange, EnemyHero, false);
        }
        else
        {
            Debug.LogError("INVALID TAG!");
            return;
        }

        void PlayUnit()
        {
            if (card.CompareTag(PLAYER_CARD))
            {
                if (!caMan.TriggerUnitAbility(card, CardManager.TRIGGER_PLAY))
                {
                    uMan.CombatLog_PlayCard(card);
                    efMan.TriggerGiveNextEffects(card);
                    efMan.ResolveChangeNextCostEffects(card);
                    caMan.TriggerTrapAbilities(card);
                }
            }
            else
            {
                uMan.CombatLog_PlayCard(card);
                caMan.TriggerTrapAbilities(card);
                caMan.TriggerUnitAbility(card, CardManager.TRIGGER_PLAY);

                FunctionTimer.Create(() => SetFirstSibling(card), 0.5f);
            }

            PlayCardSound();
            ParticleBurst();
        }
        void PlayAction()
        {
            auMan.StartStopSound("SFX_PlayCard");
            ResolveActionCard(card);
            ParticleBurst();
        }
        void PlayCardSound()
        {
            Sound playSound = cd.CardScript.CardPlaySound;
            if (playSound.clip == null) Debug.LogWarning("MISSING PLAY SOUND: " + cd.CardName);
            else auMan.StartStopSound(null, playSound);
        }
        void SetFirstSibling(GameObject card)
        {
            if (card == null) return;
            card.transform.SetAsFirstSibling();
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

        if (card.CompareTag(PLAYER_CARD))
        {
            if (isAction) previousZone = PlayerActionZoneCards;
            else previousZone = PlayerHandCards;
            newZone = PlayerDiscardCards;
        }
        else
        {
            if (isAction) previousZone = EnemyActionZoneCards;
            else previousZone = EnemyHandCards;
            newZone = EnemyDiscardCards;
        }

        previousZone.Remove(card);
        if (cd.CardScript.BanishAfterPlay) HideCard(card); // TESTING
        else newZone.Add(HideCard(card));

        if (!isAction) auMan.StartStopSound("SFX_DiscardCard");
    }

    /******
     * *****
     * ****** REFRESH_UNITS
     * *****
     *****/
    public void RefreshUnits(string hero)
    {
        List<GameObject> cardZoneList = null;
        if (hero == PLAYER) cardZoneList = PlayerZoneCards;
        else if (hero == ENEMY) cardZoneList = EnemyZoneCards;
        foreach (GameObject card in cardZoneList)
            GetUnitDisplay(card).IsExhausted = false;
    }

    /******
     * *****
     * ****** CAN_ATTACK
     * *****
     *****/
    public bool CanAttack(GameObject attacker, GameObject defender, bool preCheck)
    {
        if (defender != null)
        {
            if (attacker.CompareTag(defender.tag)) return false;
            if (attacker.CompareTag(PLAYER_CARD)) 
                if (defender == PlayerHero) return false;
        }
        else
        {
            if (!preCheck)
            {
                Debug.LogError("DEFENDER IS NULL!");
                return false;
            }
        }

        // TUTORIAL!
        if (!preCheck && gMan.IsTutorial && pMan.EnergyPerTurn == 2)
        {
            if (!IsUnitCard(defender)) return false;
        }

        UnitCardDisplay atkUcd = GetUnitDisplay(attacker);
        if (atkUcd.IsExhausted)
        {
            if (preCheck)
            {
                uMan.CreateFleetingInfoPopup("<b>Exhausted</b> units can't attack!");
                SFX_Error();
            }
            return false;
        }
        else if (atkUcd.CurrentPower < 1)
        {
            if (preCheck)
            {
                uMan.CreateFleetingInfoPopup("Units with 0 power can't attack!");
                SFX_Error();
            }
            return false;
        }

        if (defender == null && preCheck) return true; // For StartDrag in DragDrop
        
        if (defender.TryGetComponent(out UnitCardDisplay defUcd))
        {
            if (EnemyHandCards.Contains(defender)) return false; // Unnecessary, already checked in CardSelect
            if (defUcd.CurrentHealth < 1) return false; // Destroyed units that haven't left play yet
            if (CardManager.GetAbility(defender, CardManager.ABILITY_STEALTH))
            {
                if (!preCheck)
                {
                    uMan.CreateFleetingInfoPopup("Units with Stealth can't be attacked!");
                    SFX_Error();
                }
                return false;
            }
        }
        return true;

        void SFX_Error() => auMan.StartStopSound("SFX_Error");
    }

    /******
     * *****
     * ****** ATTACK
     * *****
     *****/
    public void Attack(GameObject attacker, GameObject defender)
    {
        string logEntry = "";
        if (attacker.CompareTag(PLAYER_CARD))
        {
            logEntry += "<b><color=\"green\">";

            if (gMan.IsTutorial && pMan.EnergyPerTurn == 2) // TUTORIAL!
            {
                if (pMan.HeroPowerUsed) gMan.Tutorial_Tooltip(6);
                else return;
            }
        }
        else logEntry += "<b><color=\"red\">";
        logEntry += GetUnitDisplay(attacker).CardName + "</b></color> ";
        
        logEntry += "attacked ";
        if (IsUnitCard(defender))
        {
            if (defender.CompareTag(PLAYER_CARD)) logEntry += "<b><color=\"green\">";
            else logEntry += "<b><color=\"red\">";
            logEntry += GetUnitDisplay(defender).CardName + "</b></color>.";
        }
        else
        {
            if (attacker.CompareTag(PLAYER_CARD)) logEntry += "the enemy hero.";
            else logEntry += "your hero.";
        }
        uMan.CombatLogEntry(logEntry);

        GetUnitDisplay(attacker).IsExhausted = true;
        if (CardManager.GetAbility(attacker, CardManager.ABILITY_STEALTH))
            GetUnitDisplay(attacker).RemoveCurrentAbility(CardManager.ABILITY_STEALTH);

        uMan.UpdateEndTurnButton(pMan.IsMyTurn, false);
        if (!CardManager.GetAbility(attacker, CardManager.ABILITY_RANGED))
            anMan.UnitAttack(attacker, defender, IsUnitCard(defender));
        else
        {
            PlayAttackSound(attacker);
            efMan.CreateEffectRay(attacker.transform.position, defender,
                () => Strike(attacker, defender, true), efMan.DamageRayColor, false);
        }

        if (pMan.IsMyTurn) evMan.NewDelayedAction(() => SelectPlayableCards(), 0); // TESTING
    }

    public void PlayAttackSound(GameObject unitCard)
    {
        bool isMeleeAttack = true;
        if (CardManager.GetAbility(unitCard, CardManager.ABILITY_RANGED))
            isMeleeAttack = false;

        string attackSound;
        if (GetUnitDisplay(unitCard).CurrentPower < 5)
        {
            if (isMeleeAttack) attackSound = "SFX_AttackMelee";
            else attackSound = "SFX_AttackRanged";
        }
        else
        {
            if (isMeleeAttack) attackSound = "SFX_AttackMelee_Heavy";
            else attackSound = "SFX_AttackRanged_Heavy";
        }
        auMan.StartStopSound(attackSound);
    }

    /******
     * *****
     * ****** STRIKE
     * *****
     *****/
    public void Strike(GameObject striker, GameObject defender, bool isCombat)
    {
        bool strikerDestroyed;
        //bool defenderDealtDamage; // No current use

        // COMBAT
        if (isCombat)
        {
            DealDamage(striker, defender, 
                out bool strikerDealtDamage, out bool defenderDestroyed);

            if (IsUnitCard(defender))
            {
                if (!CardManager.GetAbility(striker, CardManager.ABILITY_RANGED))
                    DealDamage(defender, striker, out _, out strikerDestroyed);
                else
                {
                    //defenderDealtDamage = false;
                    strikerDestroyed = false;
                }

                if (!strikerDestroyed && CardManager.GetTrigger
                    (striker, CardManager.TRIGGER_DEATHBLOW))
                {
                    if (defenderDestroyed)
                        caMan.TriggerUnitAbility(striker, CardManager.TRIGGER_DEATHBLOW);
                }
                if (!defenderDestroyed && CardManager.GetTrigger
                    (defender, CardManager.TRIGGER_DEATHBLOW))
                {
                    if (strikerDestroyed)
                        caMan.TriggerUnitAbility(defender, CardManager.TRIGGER_DEATHBLOW);
                }
            }
            else if (!defenderDestroyed && strikerDealtDamage)
            {
                string player;
                if (striker.CompareTag(PLAYER_CARD)) player = ENEMY;
                else player = PLAYER;
                caMan.TriggerPlayedUnits(CardManager.TRIGGER_RETALIATE, player);

                // Trigger Infiltrate BEFORE Retaliate, can cause Retaliate sources to be destroyed before triggering.
                if (CardManager.GetTrigger(striker, CardManager.TRIGGER_INFILTRATE))
                    caMan.TriggerUnitAbility(striker, CardManager.TRIGGER_INFILTRATE);
            }

            if (!(!IsUnitCard(defender) && defenderDestroyed))
                evMan.NewDelayedAction(() => uMan.UpdateEndTurnButton(pMan.IsMyTurn, true), 0);
        }
        // STRIKE EFFECTS // no current use
        /*
        else
        {
            DealDamage(striker, defender,
                out bool attackerDealtDamage, out bool defenderDestroyed);

            // delay
            if (IsUnitCard(defender))
            {
                if (defenderDestroyed &&
                CardManager.GetTrigger(striker, CardManager.TRIGGER_DEATHBLOW))
                    DeathblowTrigger(striker);
            }
            else if (attackerDealtDamage &&
                CardManager.GetTrigger(striker, CardManager.TRIGGER_INFILTRATE))
                InfiltrateTrigger(striker);
        }
        */

        void DealDamage(GameObject striker, GameObject defender,
            out bool dealtDamage, out bool defenderDestroyed)
        {
            UnitCardDisplay ucd = GetUnitDisplay(striker);
            int power = ucd.CurrentPower;
            TakeDamage(defender, power, out dealtDamage, out defenderDestroyed);

            // Poisonous
            if (IsUnitCard(defender))
            {
                UnitCardDisplay defUcd = GetUnitDisplay(defender);
                if (dealtDamage && !defenderDestroyed)
                {
                    if (CardManager.GetAbility(striker, CardManager.ABILITY_POISONOUS))
                        defUcd.AddCurrentAbility(efMan.PoisonAbility);
                }
            }
        }
    }

    /******
     * *****
     * ****** TAKE_DAMAGE
     * *****
     *****/
    public void TakeDamage(GameObject target, int damageValue, out bool wasDamaged, out bool wasDestroyed)
    {
        wasDamaged = false;
        wasDestroyed = false;

        if (damageValue < 1)
        {
            Debug.LogWarning("CANNOT DEAL 0 DAMAGE!");
            return;
        }

        uMan.ShakeCamera(UIManager.Bump_Light);
        //anMan.CreateParticleSystem(target, ParticleSystemHandler.ParticlesType.Damage, 1); // TESTING

        int targetValue;
        int newTargetValue;
        if (IsUnitCard(target)) targetValue = GetUnitDisplay(target).CurrentHealth;
        else if (target == PlayerHero) targetValue = pMan.PlayerHealth;
        else if (target == EnemyHero) targetValue = enMan.EnemyHealth;
        else
        {
            Debug.LogError("INVALID TARGET!");
            return;
        }

        if (targetValue < 1) return; // Don't deal damage to targets with 0 health
        newTargetValue = targetValue - damageValue;

        // Damage to heroes
        if (target == PlayerHero)
        {
            pMan.PlayerHealth = newTargetValue;
            anMan.ModifyHeroHealthState(target, -damageValue);
            wasDamaged = true;
        }
        else if (target == EnemyHero)
        {
            enMan.EnemyHealth = newTargetValue;
            anMan.ModifyHeroHealthState(target, -damageValue);
            wasDamaged = true;
        }
        // Damage to Units
        else
        {
            if (CardManager.GetAbility(target, CardManager.ABILITY_FORCEFIELD))
            {
                GetUnitDisplay(target).AbilityTriggerState(CardManager.ABILITY_FORCEFIELD);
                GetUnitDisplay(target).RemoveCurrentAbility(CardManager.ABILITY_FORCEFIELD);
                return; // Unnecessary?
            }
            else
            {
                if (CardManager.GetAbility(target, CardManager.ABILITY_ARMORED)) // TESTING
                {
                    GetUnitDisplay(target).AbilityTriggerState(CardManager.ABILITY_ARMORED);
                    int newDamage = damageValue - 1;
                    if (newDamage < 1) return;
                    newTargetValue = targetValue - newDamage;
                }

                int newHealth = newTargetValue;
                if (newHealth < 0) newHealth = 0;
                int damageTaken = targetValue - newHealth;

                GetUnitDisplay(target).CurrentHealth = newTargetValue;
                anMan.UnitTakeDamageState(target, damageTaken);
                wasDamaged = true;
            }
        }

        if (newTargetValue < 1)
        {
            wasDestroyed = true; // TESTING

            if (IsUnitCard(target)) DestroyUnit(target);
            else
            {
                uMan.ShakeCamera(EZCameraShake.CameraShakePresets.Earthquake);
                anMan.SetAnimatorBool(target, "IsDestroyed", true);
                bool playerWins;
                if (target == PlayerHero) playerWins = false;
                else playerWins = true;
                gMan.EndCombat(playerWins);
            }
        }
    }

    /******
     * *****
     * ****** HEAL_DAMAGE
     * *****
     *****/
    public void HealDamage(GameObject target, HealEffect healEffect)
    {
        int healingValue = healEffect.Value;

        if (healingValue < 1) return;
        int targetValue;
        int maxValue;
        int newTargetValue;
        if (target == PlayerHero)
        {
            targetValue = pMan.PlayerHealth;
            maxValue = pMan.MaxPlayerHealth;
        }
        else if (target == EnemyHero)
        {
            targetValue = enMan.EnemyHealth;
            maxValue = enMan.MaxEnemyHealth;
        }
        else
        {
            targetValue = GetUnitDisplay(target).CurrentHealth;
            maxValue = GetUnitDisplay(target).MaxHealth;
        }

        if (targetValue < 1) return; // Don't heal destroyed units or heroes

        if (healEffect.HealFully) newTargetValue = maxValue;
        else
        {
            newTargetValue = targetValue + healingValue;
            if (newTargetValue > maxValue) newTargetValue = maxValue;

            if (newTargetValue < targetValue)
            {
                Debug.LogError("NEW HEALTH < PREVIOUS HEALTH!");
                return;
            }
        }

        auMan.StartStopSound("SFX_StatPlus");
        int healthChange = newTargetValue - targetValue;

        if (IsUnitCard(target))
        {
            GetUnitDisplay(target).CurrentHealth = newTargetValue;
            anMan.UnitStatChangeState(target, 0, healthChange, true);
        }
        else
        {
            if (target == PlayerHero) pMan.PlayerHealth = newTargetValue;
            else if (target == EnemyHero) enMan.EnemyHealth = newTargetValue;
            anMan.ModifyHeroHealthState(target, healthChange);
        }
    }

    /******
     * *****
     * ****** IS_DAMAGED/IS_BUFFED
     * *****
     *****/
    public bool IsDamaged(GameObject unitCard)
    {
        UnitCardDisplay ucd = unitCard.GetComponent<UnitCardDisplay>();
        bool isDamaged = false;
        if (ucd.CurrentHealth < ucd.MaxHealth) isDamaged = true;
        return isDamaged;
    }
    public bool PowerIsBuffed(GameObject unitCard)
    {
        UnitCardDisplay ucd = unitCard.GetComponent<UnitCardDisplay>();
        bool powerIsBuffed = false;
        if (ucd.CurrentPower > ucd.UnitCard.StartPower) powerIsBuffed = true;
        return powerIsBuffed;
    }
    public bool HealthIsBuffed(GameObject unitCard)
    {
        UnitCardDisplay ucd = unitCard.GetComponent<UnitCardDisplay>();
        bool healthIsBuffed = false;
        if (ucd.CurrentHealth > ucd.UnitCard.StartHealth) healthIsBuffed = true;
        return healthIsBuffed;
    }

    /******
     * *****
     * ****** GET_LOWEST_HEALTH_UNIT
     * *****
     *****/
    public GameObject GetLowestHealthUnit(List<GameObject> unitList)
    {
        if (unitList.Count < 1) return null;
        int lowestHealth = 999;
        List<GameObject> lowestHealthUnits = new List<GameObject>();

        foreach (GameObject unit in unitList)
        {
            if (!IsUnitCard(unit)) continue;

            int health = GetUnitDisplay(unit).CurrentHealth;
            if (health < 1 || efMan.UnitsToDestroy.Contains(unit)) continue;
            if (health < lowestHealth)
            {
                lowestHealth = health;
                lowestHealthUnits.Clear();
                lowestHealthUnits.Add(unit);
            }
            else if (health == lowestHealth) lowestHealthUnits.Add(unit);
        }
        if (lowestHealthUnits.Count < 1) return null;
        if (lowestHealthUnits.Count > 1)
        {
            int randomIndex = Random.Range(0, lowestHealthUnits.Count);
            return lowestHealthUnits[randomIndex];
        }
        else return lowestHealthUnits[0];
    }

    /******
     * *****
     * ****** GET_STRONGEST_UNIT
     * *****
     *****/
    public GameObject GetStrongestUnit(List<GameObject> unitList, bool targetsEnemy)
    {
        if (unitList.Count < 1) return null;
        int highestPower = 0;
        List<GameObject> highestPowerUnits = new List<GameObject>();

        foreach (GameObject unit in unitList)
        {
            if (!IsUnitCard(unit)) continue;

            if (targetsEnemy && CardManager.GetAbility(unit, CardManager.ABILITY_WARD)) continue;

            int health = GetUnitDisplay(unit).CurrentHealth;
            if (health < 1 || efMan.UnitsToDestroy.Contains(unit)) continue;

            int power = GetUnitDisplay(unit).CurrentPower;
            if (power > highestPower)
            {
                highestPower = power;
                highestPowerUnits.Clear();
                highestPowerUnits.Add(unit);
            }
            else if (power == highestPower) highestPowerUnits.Add(unit);
        }
        if (highestPowerUnits.Count < 1) return null;
        if (highestPowerUnits.Count > 1)
        {
            int randomIndex = Random.Range(0, highestPowerUnits.Count);
            return highestPowerUnits[randomIndex];
        }
        else return highestPowerUnits[0];
    }

    /******
     * *****
     * ****** GET_WEAKEST_UNIT
     * *****
     *****/
    public GameObject GetWeakestUnit(List<GameObject> unitList, bool targetsEnemy)
    {
        if (unitList.Count < 1) return null;
        int lowestPower = 999;
        List<GameObject> lowestPowerUnits = new List<GameObject>();

        foreach (GameObject unit in unitList)
        {
            if (!IsUnitCard(unit)) continue;

            if (targetsEnemy && CardManager.GetAbility(unit, CardManager.ABILITY_WARD)) continue;

            int health = GetUnitDisplay(unit).CurrentHealth;
            if (health < 1 || efMan.UnitsToDestroy.Contains(unit)) continue;

            int power = GetUnitDisplay(unit).CurrentPower;
            if (power < lowestPower)
            {
                lowestPower = power;
                lowestPowerUnits.Clear();
                lowestPowerUnits.Add(unit);
            }
            else if (power == lowestPower) lowestPowerUnits.Add(unit);
        }
        if (lowestPowerUnits.Count < 1) return null;
        if (lowestPowerUnits.Count > 1)
        {
            int randomIndex = Random.Range(0, lowestPowerUnits.Count);
            return lowestPowerUnits[randomIndex];
        }
        else return lowestPowerUnits[0];
    }

    /******
     * *****
     * ****** DESTROY_UNIT [PLAY >>> DISCARD]
     * *****
     *****/
    public void DestroyUnit(GameObject card)
    {
        if (card == null)
        {
            Debug.LogError("CARD IS NULL!");
            return;
        }

        efMan.UnitsToDestroy.Add(card); // TESTING
        string cardTag = card.tag;
        evMan.NewDelayedAction(() => Destroy(), 0.5f, true);
        if (HasDestroyTriggers()) evMan.NewDelayedAction(() => DestroyTriggers(), 0, true);
        evMan.NewDelayedAction(() => DestroyFX(), 0.25f, true);

        bool HasDestroyTriggers()
        {
            if (CardManager.GetTrigger(card,
                CardManager.TRIGGER_REVENGE)) return true;
            if (CardManager.GetAbility(card,
                CardManager.ABILITY_MARKED)) return true;
            return false;
        }
        void DestroyFX()
        {
            if (card == null)
            {
                Debug.LogError("CARD IS NULL!");
                return;
            }
            Sound deathSound = GetUnitDisplay(card).UnitCard.UnitDeathSound;
            AudioManager.Instance.StartStopSound(null, deathSound);
            anMan.DestroyUnitCardState(card);
        }
        void DestroyTriggers()
        {
            if (card == null)
            {
                Debug.LogError("CARD IS NULL!");
                return;
            }
            caMan.TriggerUnitAbility(card, CardManager.TRIGGER_REVENGE);
            if (CardManager.GetAbility(card, CardManager.ABILITY_MARKED))
            {
                GetUnitDisplay(card).AbilityTriggerState(CardManager.ABILITY_MARKED);
                if (card.CompareTag(ENEMY_CARD)) DrawCard(PLAYER);
                else DrawCard(ENEMY);
            }
        }
        void Destroy()
        {
            if (card == null)
            {
                Debug.LogError("CARD IS NULL!");
                return;
            }

            efMan.UnitsToDestroy.Remove(card); // TESTING

            Card cardScript = card.GetComponent<CardDisplay>().CardScript;
            card.GetComponent<CardZoom>().DestroyZoomPopups();

            if (cardTag == PLAYER_CARD)
            {
                PlayerZoneCards.Remove(card);
                if (cardScript.BanishAfterPlay) HideCard(card);
                else PlayerDiscardCards.Add(HideCard(card));
            }
            else if (cardTag == ENEMY_CARD)
            {
                EnemyZoneCards.Remove(card);
                if (cardScript.BanishAfterPlay) HideCard(card);
                else EnemyDiscardCards.Add(HideCard(card));
            }
            else Debug.LogError("INVALID TAG!");

            if (pMan.IsMyTurn) SelectPlayableCards(); // TESTING
        }
    }
}
