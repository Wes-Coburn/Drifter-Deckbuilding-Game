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
    public const string ENEMY_DISCARD = "EnemyDiscard";
    public const string ENEMY_HERO_POWER = "EnemyHeroPower";

    public GameObject DragArrowPrefab { get => dragArrowPrefab; }
    public int ActionsPlayedThisTurn
    {
        get => actionsPlayedThisTurn;
        set
        {
            actionsPlayedThisTurn = value;
            if (pMan.IsMyTurn && actionsPlayedThisTurn == 1)
            {
                evMan.NewDelayedAction(() =>
                caMan.TriggerPlayedUnits(CardManager.TRIGGER_SPARK, GameManager.PLAYER), 0);
            }
        }
    }

    /* CARD LISTS */
    public List<List<GameObject>> RevealedCardLists { get; private set; }
    public List<GameObject> PlayerHandCards { get; private set; }
    public List<GameObject> PlayerZoneCards { get; private set; }
    public List<GameObject> PlayerActionZoneCards { get; private set; }
    public List<Card> PlayerDiscardCards { get; private set; }
    public List<GameObject> EnemyHandCards { get; private set; }
    public List<GameObject> EnemyZoneCards { get; private set; }
    public List<Card> EnemyDiscardCards { get; private set; }
    
    /* GAME ZONES */
    public GameObject CardZone { get; private set; }
    public GameObject PlayerHero { get; private set; }
    public GameObject PlayerHand { get; private set; }
    public GameObject PlayerZone { get; private set; }
    public GameObject PlayerActionZone { get; private set; }
    public GameObject PlayerDiscard { get; private set; }
    public GameObject EnemyHero { get; private set; }
    public GameObject EnemyHand { get; private set; }
    public GameObject EnemyZone { get; private set; }
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
        CardZone = GameObject.Find(CARD_ZONE);
        PlayerActionZone = GameObject.Find(PLAYER_ACTION_ZONE);
        PlayerHand = GameObject.Find(PLAYER_HAND);
        PlayerZone = GameObject.Find(PLAYER_ZONE);
        PlayerDiscard = GameObject.Find(PLAYER_DISCARD);
        PlayerHero = GameObject.Find(PLAYER_HERO);
        EnemyHand = GameObject.Find(ENEMY_HAND);
        EnemyZone = GameObject.Find(ENEMY_ZONE);
        EnemyDiscard = GameObject.Find(ENEMY_DISCARD);
        EnemyHero = GameObject.Find(ENEMY_HERO);
        // ZONE LISTS
        PlayerHandCards = new List<GameObject>();
        PlayerZoneCards = new List<GameObject>();
        PlayerActionZoneCards = new List<GameObject>();
        PlayerDiscardCards = new List<Card>();
        EnemyHandCards = new List<GameObject>();
        EnemyZoneCards = new List<GameObject>();
        EnemyDiscardCards = new List<Card>();
        // REVEALED CARD LISTS
        RevealedCardLists = new List<List<GameObject>>
        {
            PlayerHandCards,
            PlayerZoneCards,
            EnemyHandCards,
            EnemyZoneCards
        };
        uMan.SelectTarget(PlayerHero, false);
        uMan.SelectTarget(EnemyHero, false);
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

    public enum DisplayType
    {
        Default,
        HeroSelect,
        NewCard,
        Cardpage
    }

    /******
     * *****
     * ****** SHOW_CARD
     * *****
     *****/
    public GameObject ShowCard(Card card, Vector2 position, DisplayType type = DisplayType.Default, bool banishAfterPlay = false)
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

        if (parent == null) Debug.LogError("PARENT IS NULL!");

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
        else if (type is DisplayType.HeroSelect) cd.CardScript = card;
        else if (type is DisplayType.NewCard) cd.DisplayZoomCard(null, card);
        else if (type is DisplayType.Cardpage) cd.DisplayCardPageCard(card);
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
        if (cardScript is SkillCard)
        {
            Card script;
            int index = pMan.PlayerDeckList.FindIndex(x => x.CardName == cardScript.CardName);
            if (index != -1) script = pMan.PlayerDeckList[index];
            else
            {
                Debug.LogError("SKILL NOT FOUND!");
                return null;
            }
            caMan.RemovePlayerCard(script); // TESTING
        }
        return cardScript;
    }

    /******
     * *****
     * ****** DRAW_CARD
     * *****
     *****/
    public void DrawCard(string hero, Card createdCard = null)
    {
        List<Card> deck;
        List<GameObject> hand;
        string cardTag;
        string cardZone;
        Vector2 position = new Vector2();

        if (createdCard != null && hero != PLAYER)
        {
            Debug.LogError("CANNOT CREATE CARDS FOR ENEMY!");
            hero = PLAYER;
        }

        if (hero == PLAYER)
        {
            deck = pMan.CurrentPlayerDeck;
            hand = PlayerHandCards;
            if (hand.Count >= GameManager.MAX_HAND_SIZE)
            {
                uMan.CreateFleetingInfoPopup("Your hand is full!");
                return;
            }
            cardTag = PLAYER_CARD;
            cardZone = PLAYER_HAND;
            if (createdCard == null) position.Set(-850, -350);
            else if (createdCard is SkillCard) position.Set(-675, -350);
            else position.Set(0, -350);
        }
        else if (hero == ENEMY)
        {
            deck = enMan.CurrentEnemyDeck;
            hand = EnemyHandCards;
            if (hand.Count >= GameManager.MAX_HAND_SIZE)
            {
                uMan.CreateFleetingInfoPopup("Enemy hand is full!");
                return;
            }
            cardTag = ENEMY_CARD;
            cardZone = ENEMY_HAND;
            position.Set(685, 370);
        }
        else
        {
            Debug.LogError("PLAYER <" + hero + "> NOT FOUND!");
            return;
        }

        // Shuffle discard into deck
        if (createdCard == null && deck.Count < 1)
        {
            List<Card> discard;
            if (hero == PLAYER) discard = PlayerDiscardCards;
            else if (hero == ENEMY) discard = EnemyDiscardCards;
            else
            {
                Debug.LogError("PLAYER <" + hero + "> NOT FOUND!");
                return;
            }
            if (discard.Count < 1)
            {
                Debug.LogWarning("DISCARD IS EMPTY!");
                uMan.CreateFleetingInfoPopup("No cards left!");
                return;
            }
            foreach (Card c in discard) deck.Add(c);
            discard.Clear();
            caMan.ShuffleDeck(hero);
        }

        GameObject card;
        if (createdCard == null)
        {
            auMan.StartStopSound("SFX_DrawCard");
            card = ShowCard(deck[0], position);
        }
        else if (createdCard is SkillCard)
        {
            auMan.StartStopSound("SFX_DrawCard");
            card = ShowCard(createdCard, position, DisplayType.Default, true);
            pMan.CurrentPlayerSkillDeck.RemoveAt(0);
            pMan.SkillDrawn = true;
            PlayerHero.GetComponent<PlayerHeroDisplay>().SkillsLeft =
                pMan.CurrentPlayerSkillDeck.Count;
            uMan.CombatLogEntry("You drew a <b><color=\"yellow\">skill</b></color>.");
        }
        else
        {
            auMan.StartStopSound("SFX_CreateCard");
            card = ShowCard(createdCard, position, DisplayType.Default, true);
        }

        if (card == null)
        {
            Debug.LogError("CARD IS NULL!");
            return;
        }

        if (createdCard == null) deck.RemoveAt(0);
        card.tag = cardTag;
        ChangeCardZone(card, cardZone);
        anMan.CreateParticleSystem(card, ParticleSystemHandler.ParticlesType.Drag, 1);

        if (hero == PLAYER)
        {
            PlayerHandCards.Add(card);
            efMan.NewDrawnCards.Add(card);
        }
        else EnemyHandCards.Add(card);
    }

    /******
     * *****
     * ****** SELECT_PLAYABLE_CARDS
     * *****
     *****/
    public void SelectPlayableCards()
    {
        //evMan.NewDelayedAction(() => SelectCards(), 0);

        void SelectCards()
        {
            bool isPlayerTurn = pMan.IsMyTurn;
            List<GameObject> emptyCards = new List<GameObject>();
            foreach (GameObject card in PlayerHandCards)
            {
                if (card == null)
                {
                    emptyCards.Add(card);
                    continue;
                }

                if (isPlayerTurn && IsPlayable(card, true))
                    uMan.SelectTarget(card, true, false, false, true);
                else uMan.SelectTarget(card, false);
            }

            foreach (GameObject card in emptyCards)
                PlayerHandCards.Remove(card);
        }
    }

    /******
     * *****
     * ****** CHANGE_CARD_ZONE
     * *****
     *****/
    public void ChangeCardZone(GameObject card, string newZoneName, bool returnToIndex = false)
    {
        uMan.SelectTarget(card, false); // Unnecessary?
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
                anMan.RevealedPlayState(card);
                break;
            case ENEMY_ZONE:
                newZone = EnemyZone;
                anMan.PlayedState(card);
                break;
        }

        CardDisplay cd = card.GetComponent<CardDisplay>();
        if (!returnToIndex)
        {
            lastCardIndex = card.GetComponent<DragDrop>().LastIndex;
            lastContainerIndex = cd.CardContainer.transform.GetSiblingIndex();
            card.transform.SetAsLastSibling();
        }

        cd.CardContainer.GetComponent<CardContainer>().MoveContainer(newZone);
        bool isPlayed = false;
        if (newZoneName == PLAYER_ZONE || newZoneName == PLAYER_ACTION_ZONE ||
            newZoneName == ENEMY_ZONE) isPlayed = true;

        if (returnToIndex)
        {
            card.transform.SetSiblingIndex(lastCardIndex);
            cd.CardContainer.transform.SetSiblingIndex(lastContainerIndex);
            card.GetComponent<DragDrop>().IsPlayed = isPlayed;
        }
        else if (!isPlayed)
        {
            cd.ResetCard();
            if (newZoneName == PLAYER_HAND)
                efMan.ApplyChangeNextCostEffects(card);
        }

        if (cd is UnitCardDisplay ucd)
        {
            if (isPlayed)
            {
                if (CardManager.GetAbility(card,
                    CardManager.ABILITY_BLITZ)) ucd.IsExhausted = false;
                else ucd.IsExhausted = true;
            }
            else ucd.IsExhausted = false;
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

        CardDisplay display = card.GetComponent<CardDisplay>();
        int actionCost = display.CurrentEnergyCost;
        int playerActions = pMan.EnergyLeft;

        if (display is UnitCardDisplay)
        {
            if (PlayerZoneCards.Count >= GameManager.MAX_UNITS_PLAYED)
            {
                if (isPrecheck) return false;
                uMan.CreateFleetingInfoPopup("Too many units!");
                ErrorSound();
                return false;
            }
        }
        else if (display is ActionCardDisplay acd)
        {
            if (!efMan.CheckLegalTargets(acd.ActionCard.EffectGroupList, card, true))
            {
                if (isPrecheck) return false;
                uMan.CreateFleetingInfoPopup("You can't play that right now!");
                ErrorSound();
                return false;
            }
        }
        if (playerActions < actionCost)
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

        // PLAYER
        if (card.CompareTag(PLAYER_CARD))
        {
            evMan.PauseDelayedActions(true); // TESTING
            container.OnAttachAction += () => evMan.PauseDelayedActions(false); // TESTING

            PlayerHandCards.Remove(card);
            pMan.EnergyLeft -= cd.CurrentEnergyCost;

            if (cd is UnitCardDisplay)
            {
                PlayerZoneCards.Add(card);
                ChangeCardZone(card, PLAYER_ZONE);
                container.OnAttachAction += () => PlayUnit(); // TESTING
            }
            else if (cd is ActionCardDisplay)
            {
                PlayerActionZoneCards.Add(card);
                ChangeCardZone(card, PLAYER_ACTION_ZONE);
                container.OnAttachAction += () => PlayAction(); // TESTING
            }
            else
            {
                Debug.LogError("CARD DISPLAY TYPE NOT FOUND!");
                return;
            }

            SelectPlayableCards();
        }
        // ENEMY
        else if (card.CompareTag(ENEMY_CARD))
        {
            if (EnemyZoneCards.Count >= GameManager.MAX_UNITS_PLAYED) return;

            EnemyHandCards.Remove(card);
            if (IsUnitCard(card))
            {
                EnemyZoneCards.Add(card);
                ChangeCardZone(card, ENEMY_ZONE);
                container.OnAttachAction += () => PlayUnit();
            }
            else
            {
                Debug.LogError("CARD DISPLAY TYPE NOT FOUND!");
                return;
            }
        }
        else
        {
            Debug.LogError("CARD TAG NOT FOUND!");
            return;
        }

        void PlayUnit()
        {
            if (card.CompareTag(PLAYER_CARD))
            {
                if (!caMan.TriggerUnitAbility(card, CardManager.TRIGGER_PLAY))
                {
                    efMan.TriggerGiveNextEffects(card);
                    efMan.ResolveChangeNextCostEffects(card);
                    uMan.CombatLog_PlayCard(card);
                }
            }
            else
            {
                caMan.TriggerUnitAbility(card, CardManager.TRIGGER_PLAY);
                uMan.CombatLog_PlayCard(card);
            }

            PlayCardSound();
            PlayAbilitySounds();
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
        void PlayAbilitySounds()
        {
            float delay = 0;
            UnitCardDisplay ucd = cd as UnitCardDisplay;
            foreach (CardAbility ca in ucd.CurrentAbilities)
            {
                if (ca is StaticAbility sa)
                {
                    FunctionTimer.Create(() =>
                    auMan.StartStopSound(null, sa.GainAbilitySound), delay);
                    delay += 0.5f;
                }
            }
        }
        void ParticleBurst()
        {
            ParticleSystemHandler particleHandler =
                anMan.CreateParticleSystem(card, ParticleSystemHandler.ParticlesType.Play, 1);
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
     * ****** DISCARD_CARD [HAND/ACTION_ZONE >>> DISCARD]
     * *****
     *****/
    public void DiscardCard(GameObject card, bool isAction = false)
    {
        if (card.CompareTag(PLAYER_CARD))
        {
            if (isAction) PlayerActionZoneCards.Remove(card);
            else PlayerHandCards.Remove(card);

            if (card.GetComponent<CardDisplay>().CardScript.BanishAfterPlay)
                HideCard(card);
            else PlayerDiscardCards.Add(HideCard(card));
        }
        else
        {
            EnemyHandCards.Remove(card);
            EnemyDiscardCards.Add(HideCard(card));
        }
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
                    uMan.CreateFleetingInfoPopup("Units with <b>Stealth</b> can't be attacked!");
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
                if (pMan.HeroPowerUsed) gMan.Tutorial_Tooltip(7);
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
                () => Strike(attacker, defender, true), efMan.DamageRayColor, 0, false);
        }
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
                        caMan.TriggerUnitAbility(striker, CardManager.TRIGGER_DEATHBLOW); // TESTING
                }
                if (!defenderDestroyed && CardManager.GetTrigger
                    (defender, CardManager.TRIGGER_DEATHBLOW))
                {
                    if (strikerDestroyed)
                        caMan.TriggerUnitAbility(defender, CardManager.TRIGGER_DEATHBLOW); // TESTING
                }
            }
            else if (!defenderDestroyed && strikerDealtDamage)
            {
                string player;
                if (striker.CompareTag(PLAYER_CARD)) player = ENEMY;
                else player = PLAYER;
                caMan.TriggerPlayedUnits(CardManager.TRIGGER_RETALIATE, player); // TESTING

                // Trigger Infiltrate BEFORE Retaliate, can cause Retaliate sources to be destroyed before triggering.
                if (CardManager.GetTrigger(striker, CardManager.TRIGGER_INFILTRATE))
                    caMan.TriggerUnitAbility(striker, CardManager.TRIGGER_INFILTRATE); // TESTING
            }

            if (!(!IsUnitCard(defender) && defenderDestroyed))
                evMan.NewDelayedAction(() => uMan.UpdateEndTurnButton(pMan.IsMyTurn, true), 0); // TESTING
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
            int power = GetUnitDisplay(striker).CurrentPower;
            if (power < 1)
            {
                dealtDamage = false;
                defenderDestroyed = false;
                return;
            }

            if (IsUnitCard(defender))
            {
                if (!CardManager.GetAbility(defender, 
                    CardManager.ABILITY_FORCEFIELD)) dealtDamage = true;
                else dealtDamage = false;
            }
            else dealtDamage = true;

            if (TakeDamage(defender, power)) defenderDestroyed = true;
            else defenderDestroyed = false;
        }
    }

    /******
     * *****
     * ****** TAKE_DAMAGE
     * *****
     *****/
    public bool TakeDamage(GameObject target, int damageValue)
    {
        if (damageValue < 1) return false;

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
            return false;
        }

        if (targetValue < 1) return false; // Don't deal damage to targets with 0 health
        newTargetValue = targetValue - damageValue;

        // Damage to heroes
        if (target == PlayerHero)
        {
            pMan.PlayerHealth = newTargetValue;
            anMan.ModifyHeroHealthState(target, -damageValue);
        }
        else if (target == EnemyHero)
        {
            enMan.EnemyHealth = newTargetValue;
            anMan.ModifyHeroHealthState(target, -damageValue);
        }
        // Damage to Units
        else
        {
            if (CardManager.GetAbility(target, CardManager.ABILITY_FORCEFIELD))
            {
                GetUnitDisplay(target).AbilityTriggerState(CardManager.ABILITY_FORCEFIELD);
                GetUnitDisplay(target).RemoveCurrentAbility(CardManager.ABILITY_FORCEFIELD);
                return false;
            }
            else
            {
                int newHealth = newTargetValue;
                if (newHealth < 0) newHealth = 0;
                int damageTaken = targetValue - newHealth; // TESTING

                GetUnitDisplay(target).CurrentHealth = newTargetValue;
                anMan.UnitTakeDamageState(target, damageTaken); // TESTING
            }
        }
        if (newTargetValue < 1)
        {
            if (IsUnitCard(target)) DestroyUnit(target);
            else
            {
                uMan.ShakeCamera(EZCameraShake.CameraShakePresets.Earthquake); // TESTING
                anMan.SetAnimatorBool(target, "IsDestroyed", true);
                bool playerWins;
                if (target == PlayerHero) playerWins = false;
                else playerWins = true;
                gMan.EndCombat(playerWins);
            }
            return true;
        }
        else return false;
    }

    /******
     * *****
     * ****** HEAL_DAMAGE
     * *****
     *****/
    public void HealDamage(GameObject target, int healingValue)
    {
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
        newTargetValue = targetValue + healingValue;
        if (newTargetValue > maxValue) newTargetValue = maxValue;

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
     * ****** IS_DAMAGED
     * *****
     *****/
    public bool IsDamaged(GameObject unitCard)
    {
        UnitCardDisplay ucd = unitCard.GetComponent<UnitCardDisplay>();
        bool isDamaged = false;
        if (ucd.CurrentHealth < ucd.MaxHealth) isDamaged = true;
        return isDamaged;
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
            int health = GetUnitDisplay(unit).CurrentHealth;
            if (health < 1) continue;
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
    public GameObject GetStrongestUnit(List<GameObject> unitList)
    {
        if (unitList.Count < 1) return null;
        int highestPower = 0;
        List<GameObject> highestPowerUnits = new List<GameObject>();

        foreach (GameObject unit in unitList)
        {
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
    public GameObject GetWeakestUnit(List<GameObject> unitList)
    {
        if (unitList.Count < 1) return null;
        int lowestPower = 999;
        List<GameObject> lowestPowerUnits = new List<GameObject>();

        foreach (GameObject unit in unitList)
        {
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

        string cardTag = card.tag;
        evMan.NewDelayedAction(() => Destroy(), 0.5f, true);
        if (HasDestroyTriggers())
            evMan.NewDelayedAction(() => DestroyTriggers(), 0, true);
        evMan.NewDelayedAction(() => DestroyFX(), 0.5f, true);

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
                DrawCard(PLAYER);
            }
        }
        void Destroy()
        {
            if (card == null)
            {
                Debug.LogError("CARD IS NULL!");
                return;
            }

            Card script = card.GetComponent<CardDisplay>().CardScript;
            card.GetComponent<CardZoom>().DestroyZoomPopups();

            if (cardTag == PLAYER_CARD)
            {
                PlayerZoneCards.Remove(card);
                if (script.BanishAfterPlay) HideCard(card); // TESTING
                else PlayerDiscardCards.Add(HideCard(card));
            }
            else if (cardTag == ENEMY_CARD)
            {
                EnemyZoneCards.Remove(card);
                if (script.BanishAfterPlay) HideCard(card); // TESTING
                else EnemyDiscardCards.Add(HideCard(card));
            }
            SelectPlayableCards();
        }
    }
}
