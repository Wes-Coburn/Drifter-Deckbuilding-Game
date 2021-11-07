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
    private const string PLAYER = GameManager.PLAYER;
    private const string ENEMY = GameManager.ENEMY;

    // Actions Played
    private int actionsPlayedThisTurn;
    
    public const string PLAYER_CARD = "PlayerCard";
    public const string ENEMY_CARD = "EnemyCard";
    public const string PLAYER_HERO = "PlayerHero";
    public const string PLAYER_HAND = "PlayerHand";
    public const string PLAYER_ZONE = "PlayerZone";
    public const string PLAYER_ACTION_ZONE = "PlayerActionZone";
    public const string PLAYER_DISCARD = "PlayerDiscard";
    public const string PLAYER_FOLLOWER = "PlayerFollower";
    public const string ENEMY_HERO = "EnemyHero";
    public const string ENEMY_HAND = "EnemyHand";
    public const string ENEMY_ZONE = "EnemyZone";
    public const string ENEMY_DISCARD = "EnemyDiscard";

    public GameObject DragArrowPrefab { get => dragArrowPrefab; }
    public bool IsInCombat { get; set; }

    // TESTING
    public int ActionsPlayedThisTurn
    {
        get => actionsPlayedThisTurn;
        set
        {
            actionsPlayedThisTurn = value;
            Debug.LogWarning("ACTIONS PLAYED => <" + actionsPlayedThisTurn + ">");
            if (pMan.IsMyTurn && actionsPlayedThisTurn == 1)
            {
                evMan.NewDelayedAction(() =>
                caMan.TriggerPlayedUnits(CardManager.TRIGGER_RESEARCH), 0);
            }
        }
    }

    /* CARD LISTS */
    public List<GameObject> PlayerHandCards { get; private set; }
    public List<GameObject> PlayerZoneCards { get; private set; }
    public List<Card> PlayerDiscardCards { get; private set; }
    public List<GameObject> EnemyHandCards { get; private set; }
    public List<GameObject> EnemyZoneCards { get; private set; }
    public List<Card> EnemyDiscardCards { get; private set; }
    
    /* GAME ZONES */
    public GameObject PlayerHero { get; private set; }
    public GameObject PlayerHand { get; private set; }
    public GameObject PlayerZone { get; private set; }
    public GameObject PlayerActionZone { get; private set; }
    public GameObject PlayerDiscard { get; private set; }
    public GameObject EnemyHero { get; private set; }
    public GameObject EnemyHand { get; private set; }
    public GameObject EnemyZone { get; private set; }
    //public GameObject EnemyActionZone { get; private set; }
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
        PlayerActionZone = GameObject.Find(PLAYER_ACTION_ZONE);
        PlayerHand = GameObject.Find(PLAYER_HAND);
        PlayerZone = GameObject.Find(PLAYER_ZONE);
        PlayerDiscard = GameObject.Find(PLAYER_DISCARD);
        PlayerHero = GameObject.Find(PLAYER_HERO);
        EnemyHand = GameObject.Find(ENEMY_HAND);
        EnemyZone = GameObject.Find(ENEMY_ZONE);
        EnemyDiscard = GameObject.Find(ENEMY_DISCARD);
        EnemyHero = GameObject.Find(ENEMY_HERO);
        // ZONE OBJECTS LISTS
        PlayerHandCards = new List<GameObject>();
        PlayerZoneCards = new List<GameObject>();
        PlayerDiscardCards = new List<Card>();
        EnemyHandCards = new List<GameObject>();
        EnemyZoneCards = new List<GameObject>();
        EnemyDiscardCards = new List<Card>();
        uMan.SelectTarget(PlayerHero, false);
        uMan.SelectTarget(EnemyHero, false);
    }

    public UnitCardDisplay GetUnitDisplay(GameObject card)
        => card.GetComponent<CardDisplay>() as UnitCardDisplay;

    public bool IsUnitCard(GameObject target) => 
        target.TryGetComponent<UnitCardDisplay>(out _);

    /******
     * *****
     * ****** SHOW/HIDE_CARD
     * *****
     *****/
    public GameObject ShowCard(Card card, Vector2 position, 
        bool isShowcase = false, bool isCardPage = false)
    {
        if (card == null)
        {
            Debug.LogError("CARD IS NULL!");
            return null;
        }

        GameObject prefab = null;
        if (card is UnitCard)
        {
            prefab = caMan.UnitCardPrefab;
            if (isShowcase)
                prefab = prefab.GetComponent<CardZoom>().UnitZoomCardPrefab;
        }
        else if (card is ActionCard)
        {
            prefab = caMan.ActionCardPrefab;
            if (isShowcase)
                prefab = prefab.GetComponent<CardZoom>().ActionZoomCardPrefab;
        }
        prefab = Instantiate(prefab, uMan.CurrentCanvas.transform);
        if (isShowcase) prefab.GetComponent<CardDisplay>().DisplayZoomCard(null, card);
        else
        {
            CardDisplay cd = prefab.GetComponent<CardDisplay>();
            if (!isCardPage) cd.CardScript = card;
            else
            {
                if (cd is UnitCardDisplay ucd)
                    ucd.DisplayCardPageCard(card as UnitCard);
                else cd.CardScript = card;
            }
            cd.CardContainer = Instantiate(cardContainerPrefab, uMan.CurrentCanvas.transform);
            cd.CardContainer.transform.position = position;
            CardContainer cc = cd.CardContainer.GetComponent<CardContainer>();
            cc.Child = prefab;
            prefab.transform.SetParent(cc.gameObject.transform, false);
        }
        return prefab;
    }
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
    public void DrawCard(string hero)
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
                uMan.CreateFleetinInfoPopup("Your hand is full!");
                return;
            }

            cardTag = PLAYER_CARD;
            cardZone = PLAYER_HAND;
            position.Set(-750, -350);
        }
        else if (hero == ENEMY)
        {
            deck = enMan.CurrentEnemyDeck;
            hand = EnemyHandCards;
            if (hand.Count >= GameManager.MAX_HAND_SIZE)
            {
                uMan.CreateFleetinInfoPopup("Enemy hand is full!");
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
        if (deck.Count < 1)
        {
            List<Card> discard;
            if (hero == PLAYER) discard = PlayerDiscardCards;
            else if (hero == ENEMY) discard = EnemyDiscardCards;
            else
            {
                Debug.LogError("PLAYER <" + hero + "> NOT FOUND!");
                return;
            }
            foreach (Card c in discard) deck.Add(c);
            discard.Clear();
            caMan.ShuffleDeck(deck);
        }

        GameObject card = ShowCard(deck[0], position);
        if (card == null)
        {
            Debug.LogError("CARD IS NULL!");
            return;
        }

        deck.RemoveAt(0);
        card.tag = cardTag;
        ChangeCardZone(card, cardZone);
        if (hero == PLAYER)
        {
            PlayerHandCards.Add(card);
            // Added to NewDrawnCards for upcoming effects in the current effect group
            // Added to CurrentLegalTargets for the current effect in the current effect group
            efMan.NewDrawnCards.Add(card);
            if (uMan.PlayerIsTargetting &&
                efMan.CurrentEffect is DrawEffect)
            {
                if (!efMan.CurrentLegalTargets.Contains(card))
                    efMan.CurrentLegalTargets.Add(card);
                uMan.SelectTarget(card, true);
            }
        }
        else EnemyHandCards.Add(card);
        auMan.StartStopSound("SFX_DrawCard");
    }

    /******
     * *****
     * ****** REFRESH_FOLLOWERS
     * *****
     *****/
    public void PrepareAllies(string hero)
    {
        List<GameObject> cardZoneList = null;
        if (hero == PLAYER) cardZoneList = PlayerZoneCards;
        else if (hero == ENEMY) cardZoneList = EnemyZoneCards;
        foreach (GameObject card in cardZoneList)
            GetUnitDisplay(card).IsExhausted = false;
    }

    /******
     * *****
     * ****** IS_PLAYABLE
     * *****
     *****/
    public bool IsPlayable(GameObject card)
    {
        CardDisplay display = card.GetComponent<CardDisplay>();
        int actionCost = display.CurrentActionCost;
        int playerActions = pMan.PlayerActionsLeft;

        if (display is UnitCardDisplay)
        {
            if (PlayerZoneCards.Count >= GameManager.MAX_UNITS_PLAYED)
            {
                uMan.CreateFleetinInfoPopup("Too many units!");
                ErrorSound();
                return false;
            }
        }
        else if (display is ActionCardDisplay acd)
            if (!efMan.CheckLegalTargets(acd.ActionCard.EffectGroupList, card, true))
            {
                uMan.CreateFleetinInfoPopup("You can't play that right now!");
                ErrorSound();
                return false;
            }
        if (playerActions < actionCost)
        {
            uMan.CreateFleetinInfoPopup("Not enough actions!");
            ErrorSound();
            return false;
        }
        return true;
        void ErrorSound() => auMan.StartStopSound("SFX_Error");
    }

    /******
     * *****
     * ****** CHANGE_CARD_ZONE
     * *****
     *****/
    public void ChangeCardZone(GameObject card, string newZoneName)
    {
        GameObject zone = null;
        switch (newZoneName)
        {
            // PLAYER
            case PLAYER_HAND:
                zone = PlayerHand;
                anMan.RevealedHandState(card);
                break;
            case PLAYER_ZONE:
                zone = PlayerZone;
                anMan.PlayedState(card);
                break;
            case PLAYER_ACTION_ZONE:
                zone = PlayerActionZone;
                anMan.RevealedHandState(card);
                break;
            // ENEMY
            case ENEMY_HAND:
                zone = EnemyHand;
                anMan.RevealedPlayState(card);
                break;
            case ENEMY_ZONE:
                zone = EnemyZone;
                anMan.PlayedState(card);
                break;
        }

        MoveCard(card, zone);
        uMan.SelectTarget(card, false);

        if (card.TryGetComponent(out UnitCardDisplay ucd))
        {
            bool played = false;
            if (newZoneName == PLAYER_ZONE || newZoneName == ENEMY_ZONE) played = true;
            ucd.ResetUnitCard(played);
        }
        else card.GetComponent<DragDrop>().IsPlayed = false; // TESTING

        static void MoveCard(GameObject card, GameObject newParent)
        {
            card.GetComponent<CardDisplay>().CardContainer.
                GetComponent<CardContainer>().MoveContainer(newParent);
        }
    }

    /******
     * *****
     * ****** PLAY_CARD [HAND >>> PLAY]
     * *****
     *****/
    public void PlayCard(GameObject card)
    {
        // PLAYER
        if (card.CompareTag(PLAYER_CARD))
        {
            pMan.PlayerActionsLeft -= card.GetComponent<CardDisplay>().CurrentActionCost;
            PlayerHandCards.Remove(card);

            if (IsUnitCard(card))
            {
                PlayerZoneCards.Add(card);
                ChangeCardZone(card, PLAYER_ZONE);
                PlayUnit();
            }
            else if (card.TryGetComponent<ActionCardDisplay>(out _))
            {
                ChangeCardZone(card, PLAYER_ACTION_ZONE);
                PlayAction();
            }
            else
            {
                Debug.LogError("CARD DISPLAY TYPE NOT FOUND!");
                return;
            }
        }
        // ENEMY
        else if (card.CompareTag(ENEMY_CARD))
        {
            if (EnemyZoneCards.Count >= GameManager.MAX_UNITS_PLAYED)
            {
                uMan.CreateFleetinInfoPopup("Too many enemy units!");
                return;
            }

            EnemyHandCards.Remove(card);
            if (IsUnitCard(card))
            {
                EnemyZoneCards.Add(card);
                ChangeCardZone(card, ENEMY_ZONE);
                PlayUnit();
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
            if (pMan.IsMyTurn) // TESTING
            {
                if (!caMan.TriggerUnitAbility(card, CardManager.TRIGGER_PLAY))
                    efMan.TriggerGiveNextEffect(card);
            }
            PlayCardSound();
            PlayAbilitySounds();
        }
        void PlayAction()
        {
            PlayCardSound();
            ResolveActionCard(card);
        }
        void PlayCardSound()
        {
            Sound playSound = card.GetComponent<CardDisplay>().CardScript.CardPlaySound;
            auMan.StartStopSound(null, playSound);
        }
        void PlayAbilitySounds()
        {
            float delay = 0.3f;
            foreach (CardAbility ca in card.GetComponent<UnitCardDisplay>().CurrentAbilities)
            {
                if (ca is StaticAbility sa)
                {
                    FunctionTimer.Create(() =>
                    auMan.StartStopSound(null, sa.GainAbilitySound), delay);
                }
                delay += 0.3f;
            }
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
     * ****** DISCARD_CARD [HAND >>> DISCARD]
     * *****
     *****/
    public void DiscardCard(GameObject card, string hero, bool isAction = false)
    {
        if (hero == PLAYER)
        {
            PlayerHandCards.Remove(card);
            PlayerDiscardCards.Add(HideCard(card));
        }
        else if (hero == ENEMY)
        {
            EnemyHandCards.Remove(card);
            EnemyDiscardCards.Add(HideCard(card));
        }
        if (!isAction) auMan.StartStopSound("SFX_DiscardCard");
    }

    /******
     * *****
     * ****** CAN_ATTACK
     * *****
     *****/
    public bool CanAttack(GameObject attacker, GameObject defender, bool preCheck)
    {
        if (efMan.EffectsResolving) return false;

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

        UnitCardDisplay atkUcd = GetUnitDisplay(attacker);
        if (atkUcd.IsExhausted)
        {
            if (!preCheck)
                uMan.CreateFleetinInfoPopup("Exhausted units can't attack!");
            return false;
        }
        else if (atkUcd.CurrentPower < 1)
        {
            if (!preCheck)
                uMan.CreateFleetinInfoPopup("Units with 0 power can't attack!");
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
                    uMan.CreateFleetinInfoPopup("Units with Stealth can't be attacked!");
                return false;
            }
        }
        return true;
    }

    /******
     * *****
     * ****** ATTACK
     * *****
     *****/
    public void Attack(GameObject attacker, GameObject defender)
    {
        // Immediate Actions
        GetUnitDisplay(attacker).IsExhausted = true;
        if (CardManager.GetAbility(attacker, CardManager.ABILITY_STEALTH))
            GetUnitDisplay(attacker).RemoveCurrentAbility(CardManager.ABILITY_STEALTH);
        if (!CardManager.GetAbility(attacker, CardManager.ABILITY_RANGED))
            auMan.StartStopSound("SFX_AttackMelee");
        else auMan.StartStopSound("SFX_AttackRanged");
        bool defenderIsUnit = IsUnitCard(defender);
        anMan.UnitAttack(attacker, defender, defenderIsUnit);

        // Delayed Actions
        Strike(attacker, defender, true);
    }

    /******
     * *****
     * ****** STRIKE
     * *****
     *****/
    public void Strike(GameObject striker, GameObject defender, bool isCombat)
    {
        bool strikerDestroyed;
        bool defenderDealtDamage;

        // COMBAT
        if (isCombat)
        {
            DealDamage(striker, defender, 
                out bool strikerDealtDamage, out bool defenderDestroyed);
            if (IsUnitCard(defender))
            {
                if (!CardManager.GetAbility(striker, CardManager.ABILITY_RANGED))
                    DealDamage(defender, striker, 
                        out defenderDealtDamage, out strikerDestroyed);
                else
                {
                    defenderDealtDamage = false;
                    strikerDestroyed = false;
                }

                if (!strikerDestroyed &&
                    CardManager.GetTrigger(striker, CardManager.TRIGGER_DEATHBLOW))
                {
                    if (defenderDestroyed)
                        TriggerDelay(() => DeathblowTrigger(striker));
                }
                if (!defenderDestroyed &&
                    CardManager.GetTrigger(defender, CardManager.TRIGGER_DEATHBLOW))
                {
                    if (strikerDestroyed)
                        TriggerDelay(() => DeathblowTrigger(defender));
                }
            }
            else if (!defenderDestroyed && strikerDealtDamage &&
                CardManager.GetTrigger(striker, CardManager.TRIGGER_INFILTRATE))
                TriggerDelay(() => InfiltrateTrigger(striker));
        }

        // STRIKE EFFECTS
        else
        {
            DealDamage(striker, defender,
                out bool attackerDealtDamage, out bool defenderDestroyed);
            if (IsUnitCard(defender))
            {
                if (defenderDestroyed &&
                CardManager.GetTrigger(striker, CardManager.TRIGGER_DEATHBLOW))
                    TriggerDelay(() => DeathblowTrigger(striker));
            }
            else if (attackerDealtDamage &&
                CardManager.GetTrigger(striker, CardManager.TRIGGER_INFILTRATE))
                TriggerDelay(() => InfiltrateTrigger(striker));
        }

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
                if (!CardManager.GetAbility(defender, CardManager.ABILITY_FORCEFIELD))
                    dealtDamage = true;
                else dealtDamage = false;
            }
            else dealtDamage = true;

            if (TakeDamage(defender, power))
                defenderDestroyed = true;
            else defenderDestroyed = false;
        }

        void TriggerDelay(System.Action action) =>
            evMan.NewDelayedAction(() => action(), 0.5f, true);
        
        void InfiltrateTrigger(GameObject unit) =>
            caMan.TriggerUnitAbility(unit, CardManager.TRIGGER_INFILTRATE);
        
        void DeathblowTrigger(GameObject unit) => 
            caMan.TriggerUnitAbility(unit, CardManager.TRIGGER_DEATHBLOW);
    }

    /******
     * *****
     * ****** TAKE_DAMAGE
     * *****
     *****/
    public bool TakeDamage(GameObject target, int damageValue)
    {
        if (damageValue < 1) return false;

        int targetValue;
        int newTargetValue;
        if (target == PlayerHero) targetValue = pMan.PlayerHealth;
        else if (target == EnemyHero) targetValue = enMan.EnemyHealth;
        else targetValue = GetUnitDisplay(target).CurrentHealth;

        if (targetValue < 1) return false; // Don't deal damage to destroyed units or heroes
        newTargetValue = targetValue - damageValue;
        // Damage to heroes
        if (target == PlayerHero)
        {
            pMan.PlayerHealth = newTargetValue;
            anMan.ModifyHeroHealthState(target);
        }
        else if (target == EnemyHero)
        {
            enMan.EnemyHealth = newTargetValue;
            anMan.ModifyHeroHealthState(target);
        }
        // Damage to Units
        else
        {
            if (CardManager.GetAbility(target, CardManager.ABILITY_FORCEFIELD))
            {
                GetUnitDisplay(target).AbilityTriggerState(CardManager.ABILITY_FORCEFIELD);
                FunctionTimer.Create(() =>
                GetUnitDisplay(target).RemoveCurrentAbility(CardManager.ABILITY_FORCEFIELD), 1.75f); // TESTING
                return false;
            }
            else
            {
                GetUnitDisplay(target).CurrentHealth = newTargetValue;
                anMan.UnitTakeDamageState(target);
            }
        }
        if (newTargetValue < 1)
        {
            if (IsUnitCard(target)) DestroyUnit(target, true);
            else if (target == PlayerHero) gMan.EndCombat(false);
            else if (target == EnemyHero) gMan.EndCombat(true);
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
            maxValue = GameManager.PLAYER_STARTING_HEALTH;
        }
        else if (target == EnemyHero)
        {
            targetValue = enMan.EnemyHealth;
            maxValue = GameManager.ENEMY_STARTING_HEALTH;
        }
        else
        {
            targetValue = GetUnitDisplay(target).CurrentHealth;
            maxValue = GetUnitDisplay(target).MaxHealth;
        }
        if (targetValue < 1) return; // Don't heal destroyed units or heroes
        newTargetValue = targetValue + healingValue;
        if (newTargetValue > maxValue) newTargetValue = maxValue;
        if (target == PlayerHero) pMan.PlayerHealth = newTargetValue;
        else if (target == EnemyHero) enMan.EnemyHealth = newTargetValue;
        else GetUnitDisplay(target).CurrentHealth = newTargetValue;
        anMan.UnitStatChangeState(target, false, true);
    }

    /******
     * *****
     * ****** DESTROY_UNIT [PLAY >>> DISCARD]
     * *****
     *****/
    public void DestroyUnit(GameObject card, bool isDelayed)
    {
        if (card == null)
        {
            Debug.LogError("CARD IS NULL!");
            return;
        }

        string cardTag = card.tag;
        if (isDelayed)
        {
            FunctionTimer.Create(() => DestroyFX(), 0.5f);
            evMan.NewDelayedAction(() => Destroy(), 1, true);
            if (HasDestroyTriggers())
                evMan.NewDelayedAction(() =>
                DestroyTriggers(), 0.5f, true);
        }
        else
        {
            DestroyFX();
            evMan.NewDelayedAction(() => Destroy(), 1, true);
            if (HasDestroyTriggers())
                evMan.NewDelayedAction(() =>
                DestroyTriggers(), 0, true);
        }

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
            card.GetComponent<CardZoom>().DestroyZoomPopups();
            if (cardTag == PLAYER_CARD)
            {
                PlayerZoneCards.Remove(card);
                PlayerDiscardCards.Add(HideCard(card));
            }
            else if (cardTag == ENEMY_CARD)
            {
                EnemyZoneCards.Remove(card);
                EnemyDiscardCards.Add(HideCard(card));
            }
        }
    }
}
