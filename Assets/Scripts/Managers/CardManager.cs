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

    /* CARD_MANAGER_DATA */
    public const int CARD_Z_POSITION = -2;
    public const string WORLD_SPACE = "WorldSpace";
    public const string PLAYER_CARD = "PlayerCard";
    public const string ENEMY_CARD = "EnemyCard";
    public const string PLAYER_ACTION_ZONE = "PlayerActionZone";
    public const string PLAYER_HAND = "PlayerHand";
    public const string PLAYER_ZONE = "PlayerZone";
    public const string PLAYER_DISCARD = "PlayerDiscard";
    public const string PLAYER_HERO = "PlayerHero";
    public const string PLAYER_FOLLOWER = "PlayerFollower";
    //public const string ENEMY_ACTION_ZONE = "EnemyActionZone";
    public const string ENEMY_HAND = "EnemyHand";
    public const string ENEMY_ZONE = "EnemyZone";
    public const string ENEMY_DISCARD = "EnemyDiscard";
    public const string ENEMY_HERO = "EnemyHero";
    public const string ENEMY_FOLLOWER = "EnemyFollower";
    /* GAME_MANAGER_DATA */
    private const string PLAYER = GameManager.PLAYER;
    private const string ENEMY = GameManager.ENEMY;

    [SerializeField] private GameObject unitCardPrefab;
    [SerializeField] private GameObject actionCardPrefab;
    [SerializeField] private GameObject newCardPopupPrefab;
    [SerializeField] private UnitCard[] playerStartUnits;
    [SerializeField] private Sprite cardBackSprite;
    
    public GameObject NewCardPopup { get; private set; }
    public UnitCard[] PlayerStartUnits { get => playerStartUnits; }
    //public Sprite CardBackSprite { get => cardBackSprite; }

    /* CARD LISTS */
    public List<GameObject> PlayerHandCards { get; private set; }
    public List<GameObject> PlayerZoneCards { get; private set; }
    public List<Card> PlayerDiscardCards { get; private set; }
    public List<GameObject> EnemyHandCards { get; private set; }
    public List<GameObject> EnemyZoneCards { get; private set; }
    public List<Card> EnemyDiscardCards { get; private set; }
    /* GAME ZONES */
    public GameObject PlayerActionZone { get; private set; }
    public GameObject PlayerHand { get; private set; }
    public GameObject PlayerZone { get; private set; }
    public GameObject PlayerDiscard { get; private set; }
    public GameObject PlayerHero { get; private set; }
    //public GameObject EnemyActionZone { get; private set; }
    public GameObject EnemyDiscard { get; private set; }
    public GameObject EnemyHand { get; private set; }
    public GameObject EnemyZone { get; private set; }
    public GameObject EnemyHero { get; private set; }

    public void StartCombatScene()
    {
        // GAME_ZONES
        PlayerActionZone = GameObject.Find(PLAYER_ACTION_ZONE);
        PlayerHand = GameObject.Find(PLAYER_HAND);
        PlayerZone = GameObject.Find(PLAYER_ZONE);
        PlayerDiscard = GameObject.Find(PLAYER_DISCARD);
        PlayerHero = GameObject.Find(PLAYER_HERO);
        //EnemyActionZone = GameObject.Find(ENEMY_ACTION_ZONE);
        EnemyHand = GameObject.Find(ENEMY_HAND);
        EnemyZone = GameObject.Find(ENEMY_ZONE);
        EnemyDiscard = GameObject.Find(ENEMY_DISCARD);
        EnemyHero = GameObject.Find(ENEMY_HERO);
        // GAME_ZONE_LISTS
        PlayerHandCards = new List<GameObject>();
        PlayerZoneCards = new List<GameObject>();
        PlayerDiscardCards = new List<Card>();
        EnemyHandCards = new List<GameObject>();
        EnemyZoneCards = new List<GameObject>();
        EnemyDiscardCards = new List<Card>();

        UIManager.Instance.SelectTarget(PlayerHero, false);
        UIManager.Instance.SelectTarget(EnemyHero, false);
    }

    public UnitCardDisplay GetUnitDisplay(GameObject card)
        => card.GetComponent<CardDisplay>() as UnitCardDisplay;

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
            deckList = PlayerManager.Instance.PlayerDeckList;
            currentDeck = PlayerManager.Instance.CurrentPlayerDeck;
        }
        else if (hero == GameManager.ENEMY)
        {
            deckList = EnemyManager.Instance.EnemyDeckList;
            currentDeck = EnemyManager.Instance.CurrentEnemyDeck;
        }
        else
        {
            Debug.LogError("HERO NOT FOUND!");
            return;
        }
        
        currentDeck.Clear();
        foreach (Card card in deckList) currentDeck.Add(card);
        ShuffleDeck(currentDeck);
    }

    /******
     * *****
     * ****** ADD_CARD
     * *****
     *****/
    public void AddCard(Card card, string hero, bool isStartingCard = true)
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
            DestroyNewCardPopup();
            CreateNewCardPopup(cardInstance);
        }
    }

    /******
     * *****
     * ****** CREATE/DESTROY_NEW_CARD_POPUP
     * *****
     *****/
    private void CreateNewCardPopup(Card card)
    {
        NewCardPopup = Instantiate(newCardPopupPrefab, UIManager.Instance.CurrentCanvas.transform);
        NewCardPopup.GetComponent<NewCardPopupDisplay>().CurrentCard = card;
        // play sounds
    }
    public void DestroyNewCardPopup()
    {
        if (NewCardPopup != null)
        {
            Destroy(NewCardPopup);
            NewCardPopup = null;
        }
    }

    /******
     * *****
     * ****** SHOW/HIDE_CARD
     * *****
     *****/
    public GameObject ShowCard (Card card, bool isShowcase = false)
    {
        GameObject prefab = null;
        if (card is UnitCard)
        {
            prefab = unitCardPrefab;
            if (isShowcase)
                prefab = prefab.GetComponent<CardZoom>().UnitZoomCardPrefab;
        }
        else if (card is ActionCard)
        {
            prefab = actionCardPrefab;
            if (isShowcase)
                prefab = prefab.GetComponent<CardZoom>().ActionZoomCardPrefab;
        }

        prefab = Instantiate(prefab, new Vector3(0, 0, CARD_Z_POSITION), Quaternion.identity);
        if (isShowcase) prefab.GetComponent<CardDisplay>().DisplayZoomCard(null, card);
        else prefab.GetComponent<CardDisplay>().CardScript = card;
        return prefab;
    }
    private Card HideCard (GameObject card)
    {
        Card cardScript = card.GetComponent<CardDisplay>().CardScript;
        Destroy(card);
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
        string cardTag;
        string cardZone;

        if (hero == PLAYER)
        {
            deck = PlayerManager.Instance.CurrentPlayerDeck;
            cardTag = PLAYER_CARD;
            cardZone = PLAYER_HAND;
        }
        else if (hero == ENEMY)
        {
            deck = EnemyManager.Instance.CurrentEnemyDeck;
            cardTag = ENEMY_CARD;
            cardZone = ENEMY_HAND;
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

            foreach (Card c in discard) 
                deck.Add(c);
            discard.Clear();
            ShuffleDeck(deck);
        }

        GameObject card = ShowCard(deck[0]);

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
            EffectManager em = EffectManager.Instance;
            em.NewDrawnCards.Add(card);

            if (UIManager.Instance.PlayerIsTargetting && 
                em.CurrentEffect is DrawEffect)
            {
                if (!em.CurrentLegalTargets.Contains(card))
                    em.CurrentLegalTargets.Add(card);
                card.GetComponent<CardSelect>().CardOutline.SetActive(true);
            }
        }
        else EnemyHandCards.Add(card);
        AudioManager.Instance.StartStopSound("SFX_DrawCard");
    }

    /******
     * *****
     * ****** SHUFFLE_DECK
     * *****
     *****/
    public void ShuffleDeck(List<Card> deck)
    {
        deck.Shuffle();
        AudioManager.Instance.StartStopSound("SFX_ShuffleDeck");
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
            card.GetComponent<UnitCardDisplay>().IsExhausted = false;
    }

    /******
     * *****
     * ****** IS_PLAYABLE/IS_EXHAUSTED
     * *****
     *****/
    public bool IsPlayable(GameObject card)
    {
        int actionCost = card.GetComponent<CardDisplay>().CurrentActionCost;
        int playerActions = PlayerManager.Instance.PlayerActionsLeft;

        if (playerActions < actionCost)
        {
            UIManager.Instance.CreateFleetinInfoPopup("Not enough action points!");
            return false;
        }
        if (card.GetComponent<CardDisplay>() is ActionCardDisplay acd)
            if (!EffectManager.Instance.CheckLegalTargets(acd.ActionCard.EffectGroupList, card, true))
            {
                UIManager.Instance.CreateFleetinInfoPopup("You can't play that right now!");
                return false;
            }
        return true;
    }

    /******
     * *****
     * ****** GET_ABILITY
     * *****
     *****/
    public static bool GetAbility(GameObject card, string ability)
    {
        UnitCardDisplay fcd = card.GetComponent<UnitCardDisplay>();
        int abilityIndex = fcd.CurrentAbilities.FindIndex(x => x.AbilityName == ability);
        if (abilityIndex == -1) return false;
        else return true;
    }

    /******
     * *****
     * ****** GET_ABILITY_INDEX
     * *****
     *****/
    public static int GetAbilityIndex(GameObject card, string ability)
    {
        UnitCardDisplay fcd = card.GetComponent<UnitCardDisplay>();
        int abilityIndex = fcd.CurrentAbilities.FindIndex(x => x.AbilityName == ability);
        return abilityIndex;
    }

    /******
     * *****
     * ****** SET_CARD_PARENT
     * *****
     *****/
    public void SetCardParent(GameObject card, Transform parentTransform)
    {
        card.transform.SetParent(parentTransform, false);
        float xPos = card.transform.position.x;
        float yPos = card.transform.position.y;
        card.transform.position = new Vector3(xPos, yPos, CARD_Z_POSITION);
        // UNNECESSARY?
        CardDisplay cd = card.GetComponent<CardDisplay>();
        if (cd is UnitCardDisplay)
            card.GetComponent<ChangeLayer>().CardsLayer();
        else if (cd is ActionCardDisplay)
            card.GetComponent<ChangeLayer>().ActionsLayer();
        else Debug.LogError("CARD DISPLAY TYPE NOT FOUND!");
    }

    /******
     * *****
     * ****** CHANGE_CARD_ZONE
     * *****
     *****/
    public void ChangeCardZone(GameObject card, string zone)
    {
        Transform zoneTran = null;
        switch (zone)
        {
            // PLAYER
            case PLAYER_HAND:
                zoneTran = PlayerHand.transform;
                AnimationManager.Instance.RevealedHandState(card);
                break;
            case PLAYER_ZONE:
                zoneTran = PlayerZone.transform;
                AnimationManager.Instance.PlayedState(card);
                break;
            case PLAYER_ACTION_ZONE:
                zoneTran = PlayerActionZone.transform;
                AnimationManager.Instance.PlayedState(card);
                break;
            case PLAYER_DISCARD:
                zoneTran = PlayerDiscard.transform;
                AnimationManager.Instance.RevealedPlayState(card);
                break;
            // ENEMY
            case ENEMY_HAND:
                zoneTran = EnemyHand.transform;
                break;
            case ENEMY_ZONE:
                zoneTran = EnemyZone.transform;
                AnimationManager.Instance.PlayedState(card);
                break;
            /*
            case ENEMY_ACTION_ZONE:
                zoneTran = EnemyActionZone.transform;
                AnimationManager.Instance.PlayedState(card);
            */
            case ENEMY_DISCARD:
                zoneTran = EnemyDiscard.transform;
                AnimationManager.Instance.RevealedPlayState(card);
                break;
        }
        SetCardParent(card, zoneTran);

        if (card.GetComponent<CardDisplay>() is UnitCardDisplay fcd)
        {
            bool played = false;
            if (zone == PLAYER_ZONE || zone == ENEMY_ZONE) played = true;
            fcd.ResetUnitCard(played);
        }
    }

    /******
     * *****
     * ****** PLAY_CARD [HAND >>> PLAY]
     * *****
     *****/
    public void PlayCard(GameObject card)
    {
        void PlayUnit()
        {
            TriggerCardAbility(card, "Play");
            FunctionTimer.Create(() => PlayCardSound(), 0f);
            FunctionTimer.Create(() => PlayAbilitySounds(), 0.4f);
        }
        void PlayAction()
        {
            FunctionTimer.Create(() => PlayCardSound(), 0f);
            ResolveActionCard(card);
        }
        void PlayCardSound()
        {
            Sound playSound = card.GetComponent<CardDisplay>().CardScript.CardPlaySound;
            FunctionTimer.Create(() => 
            AudioManager.Instance.StartStopSound(null, playSound), 0.2f);
        }
        void PlayAbilitySounds()
        {
            float delay = 0.3f;
            foreach (CardAbility ca in card.GetComponent<UnitCardDisplay>().CurrentAbilities)
            {
                if (ca is StaticAbility sa)
                {
                    FunctionTimer.Create(() => 
                    AudioManager.Instance.StartStopSound(null, sa.GainAbilitySound), delay);
                }
                delay += 0.3f;
            }
        }

        // PLAYER
        if (card.CompareTag(PLAYER_CARD))
        {
            PlayerManager.Instance.PlayerActionsLeft -= card.GetComponent<CardDisplay>().CurrentActionCost;
            PlayerHandCards.Remove(card);

            if (card.GetComponent<CardDisplay>() is UnitCardDisplay)
            {
                PlayerZoneCards.Add(card);
                ChangeCardZone(card, PLAYER_ZONE);
                PlayUnit();
                FunctionTimer.Create(() => TriggerGiveNextEffect(card), 0.4f);
            }
            else if (card.GetComponent<CardDisplay>() is ActionCardDisplay)
            {
                ChangeCardZone(card, PLAYER_ACTION_ZONE);
                PlayAction();
            }
            else
            {
                Debug.LogError("CARDDISPLAY TYPE NOT FOUND!");
                return;
            }
        }
        // ENEMY
        else if (card.CompareTag(ENEMY_CARD))
        {
            EnemyHandCards.Remove(card);

            if (card.GetComponent<CardDisplay>() is UnitCardDisplay)
            {
                EnemyZoneCards.Add(card);
                ChangeCardZone(card, ENEMY_ZONE);
                PlayUnit();
            }
            /* Enemies don't play action cards
            else if (card.GetComponent<CardDisplay>() is ActionCardDisplay)
            {
                ChangeCardZone(card, ENEMY_ACTION_ZONE);
                PlayAction();
            }
            */
            else
            {
                Debug.LogError("CARDDISPLAY TYPE NOT FOUND!");
                return;
            }
        }
        else
        {
            Debug.LogError("CARD TAG NOT FOUND!");
            return;
        }
    }

    /******
     * *****
     * ****** RESOLVE_ACTION_CARD
     * *****
     *****/
    private void ResolveActionCard(GameObject card)
    {
        List<EffectGroup> groupList = card.GetComponent<ActionCardDisplay>().ActionCard.EffectGroupList;
        EffectManager.Instance.StartEffectGroupList(groupList, card);
    }

    /******
     * *****
     * ****** DESTROY_CARD [PLAY >>> DISCARD]
     * *****
     *****/
    public void DestroyCard(GameObject card, string hero)
    {
        void Destroy()
        {
            TriggerCardAbility(card, "Revenge");
            if (GetAbility(card, "Marked"))
                CardManager.Instance.DrawCard(PLAYER);

            if (hero == PLAYER)
            {
                PlayerZoneCards.Remove(card);
                PlayerDiscardCards.Add(HideCard(card));
            }
            else if (hero == ENEMY)
            {
                EnemyZoneCards.Remove(card);
                EnemyDiscardCards.Add(HideCard(card));
            }
        }
        FunctionTimer.Create(() => Destroy(), 1f);
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
        if (!isAction) AudioManager.Instance.StartStopSound("SFX_DiscardCard");
    }

    /******
     * *****
     * ****** CAN_ATTACK
     * *****
     *****/
    public bool CanAttack(GameObject attacker, GameObject defender, bool preCheck = false)
    {
        if (defender != null)
        {
            if (attacker.CompareTag(defender.tag)) return false;
            if (attacker.CompareTag(CardManager.PLAYER_CARD))
                if (defender == PlayerHero) return false;
            else if (attacker.CompareTag(CardManager.ENEMY_CARD))
                if (defender == EnemyHero) return false;
        }

        if (GetUnitDisplay(attacker).IsExhausted)
        {
            if (!preCheck) 
                UIManager.Instance.CreateFleetinInfoPopup("Exhausted units can't attack!");
            return false;
        }
        if (defender == null) return true; // For StartDrag in DragDrop
        else if (defender != PlayerHero && defender != EnemyHero)
            if (GetAbility(defender, "Stealth"))
            {
                if (!preCheck)
                    UIManager.Instance.CreateFleetinInfoPopup("Units with Stealth can't be attacked!");
                return false;
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
        Strike(attacker, defender);
        GetUnitDisplay(attacker).IsExhausted = true;

        if (GetAbility(attacker, "Stealth"))
            attacker.GetComponent<UnitCardDisplay>().RemoveCurrentAbility("Stealth");

        bool defenderIsUnit;
        if (defender.CompareTag(ENEMY_CARD) || defender.CompareTag(PLAYER_CARD))
        {
            defenderIsUnit = true;
            if (!GetAbility(attacker, "Ranged"))
                Strike(defender, attacker);
        }
        else defenderIsUnit = false;

        if (!GetAbility(attacker, "Ranged"))
            AudioManager.Instance.StartStopSound("SFX_AttackMelee");
        else AudioManager.Instance.StartStopSound("SFX_AttackRanged");

        AnimationManager.Instance.UnitAttack(attacker, defender, defenderIsUnit); // TESTING
    }

    /******
     * *****
     * ****** STRIKE
     * *****
     *****/
    public void Strike(GameObject striker, GameObject defender)
    {
        int power = GetUnitDisplay(striker).CurrentPower;
        TriggerCardAbility(striker, "Strike");
        if (TakeDamage(defender, power))
            TriggerCardAbility(striker, "Deathblow");
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
        PlayerManager pm = PlayerManager.Instance;
        EnemyManager em = EnemyManager.Instance;

        if (target == PlayerHero) targetValue = pm.PlayerHealth;
        else if (target == EnemyHero) targetValue = em.EnemyHealth;
        else targetValue = GetUnitDisplay(target).CurrentDefense;
        newTargetValue = targetValue - damageValue;
        
        // Damage to heroes
        if (target == PlayerHero)
        {
            pm.PlayerHealth = newTargetValue;
            AnimationManager.Instance.ModifyHealthState(target);
        }
        else if (target == EnemyHero)
        {
            em.EnemyHealth = newTargetValue;
            AnimationManager.Instance.ModifyHealthState(target);
        }
        else // Damage to Units
        {
            if (GetAbility(target, "Shield"))
            {
                target.GetComponent<UnitCardDisplay>().RemoveCurrentAbility("Shield");
                return false;
            }
            else
            {
                GetUnitDisplay(target).CurrentDefense = newTargetValue;
                AnimationManager.Instance.ModifyDefenseState(target);
            }
        }

        if (newTargetValue < 1)
        {
            if (target.CompareTag(PLAYER_CARD)) DestroyCard(target, PLAYER);
            else if (target.CompareTag(ENEMY_CARD)) DestroyCard(target, ENEMY);
            else if (target == PlayerHero) GameManager.Instance.EndCombat(false);
            else if (target == EnemyHero) GameManager.Instance.EndCombat(true);

            if (target == PlayerHero || target == EnemyHero) return false;
            else
            {
                Sound deathSound = target.GetComponent<UnitCardDisplay>().UnitCard.UnitDeathSound;
                AudioManager.Instance.StartStopSound(null, deathSound);
                return true;
            }
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
        int targetValue;
        int maxValue;
        int newTargetValue;
        PlayerManager pm = PlayerManager.Instance;
        EnemyManager em = EnemyManager.Instance;

        if (target == PlayerHero)
        {
            targetValue = pm.PlayerHealth;
            maxValue = GameManager.PLAYER_STARTING_HEALTH;
        }
        else if (target == EnemyHero)
        {
            targetValue = em.EnemyHealth;
            maxValue = GameManager.ENEMY_STARTING_HEALTH;
        }
        else
        {
            targetValue = GetUnitDisplay(target).CurrentDefense;
            maxValue = GetUnitDisplay(target).MaxDefense;
        }
        newTargetValue = targetValue + healingValue;
        if (newTargetValue > maxValue) newTargetValue = maxValue;

        if (target == PlayerHero) pm.PlayerHealth = newTargetValue;
        else if (target == EnemyHero) em.EnemyHealth = newTargetValue;
        else GetUnitDisplay(target).CurrentDefense = newTargetValue;
        AnimationManager.Instance.ModifyDefenseState(target);
    }

    /******
     * *****
     * ****** ADD_EFFECT
     * *****
     *****/
    public void AddEffect(GameObject card, Effect effect)
    {
        Debug.Log("EFFECT ADDED: <" + effect.ToString() + ">");
        UnitCardDisplay ucd = GetUnitDisplay(card);

        // GIVE_ABILITY_EFFECT
        if (effect is GiveAbilityEffect gae)
        {
            Debug.Log("GiveAbilityEffect! <" + gae.CardAbility.ToString() + ">");
            GiveAbilityEffect newGae = ScriptableObject.CreateInstance<GiveAbilityEffect>();
            newGae.LoadEffect(gae);

            // If ability already exists, update countdown instead of adding
            if (!ucd.AddCurrentAbility(newGae.CardAbility, true))
            {
                foreach (Effect effect2 in ucd.CurrentEffects)
                    if (effect2 is GiveAbilityEffect gae2)
                        if (gae2.CardAbility == newGae.CardAbility)
                            if (newGae.Countdown == 0 || newGae.Countdown > gae2.Countdown)
                                gae2.Countdown = newGae.Countdown;
            }
            else ucd.CurrentEffects.Add(newGae);
        }
        // STAT_CHANGE_EFFECT
        else if (effect is StatChangeEffect sce)
        {
            StatChangeEffect newSce = ScriptableObject.CreateInstance<StatChangeEffect>();
            newSce.LoadEffect(sce);
            ucd.CurrentEffects.Add(newSce);

            int statChange = sce.Value;
            if (sce.IsNegative) statChange = -statChange;

            if (sce.IsDefenseChange)
            {
                ucd.MaxDefense += statChange;
                ucd.CurrentDefense += statChange;
            }
            else ucd.CurrentPower += statChange;
        }
        else
        {
            Debug.LogError("EFFECT TYPE NOT FOUND!");
            return;
        }
    }

    /******
     * *****
     * ****** REMOVE_TEMPORARY_EFFECTS
     * *****
     *****/
    public void RemoveTemporaryEffects(string hero)
    {
        static void DestroyEffect(Effect effect)
        {
            Destroy(effect);
            effect = null;
        }

        List<GameObject> cardZone;
        if (hero == PLAYER) cardZone = PlayerZoneCards;
        else cardZone = EnemyZoneCards;

        foreach (GameObject card in cardZone)
        {
            UnitCardDisplay fcd = GetUnitDisplay(card);
            List<Effect> expiredEffects = new List<Effect>();

            foreach (Effect effect in fcd.CurrentEffects)
            {
                if (effect.Countdown == 1) // Check for EXPIRED effects
                {
                    Debug.LogWarning("EFFECT REMOVED: <" + effect.ToString() + ">");
                    // GIVE_ABILITY_EFFECT
                    if (effect is GiveAbilityEffect gae) 
                        fcd.RemoveCurrentAbility(gae.CardAbility.AbilityName);
                    // STAT_CHANGE_EFFECT
                    else if (effect is StatChangeEffect sce)
                    {
                        int statChange = sce.Value;
                        if (sce.IsNegative) statChange = -statChange;

                        if (sce.IsDefenseChange)
                        {
                            fcd.CurrentDefense -= statChange;
                            fcd.MaxDefense -= statChange;
                        }
                        else fcd.CurrentPower -= statChange;
                    }
                    expiredEffects.Add(effect);
                }
                else if (effect.Countdown != 0)
                {
                    effect.Countdown -= 1;
                    Debug.LogWarning("COUNTOWN FOR EFFECT <" + effect.ToString() + "> IS: " + effect.Countdown);
                }
            }
            foreach (Effect effect in expiredEffects)
            {
                fcd.CurrentEffects.Remove(effect);
                DestroyEffect(effect);
            }
            fcd = null;
            expiredEffects = null;
        }
    }

    /******
     * *****
     * ****** TRIGGER_GIVE_NEXT_EFFECT
     * *****
     *****/
    public void TriggerGiveNextEffect(GameObject card)
    {
        static void DestroyEffect(Effect effect)
        {
            Destroy(effect);
            effect = null;
        }

        // GIVE_NEXT_FOLLOWER_EFFECTS
        List<GiveNextUnitEffect> gnfeList = EffectManager.Instance.GiveNextEffects;
        List<GiveNextUnitEffect> resolvedGnfe = new List<GiveNextUnitEffect>();

        if (gnfeList.Count > 0)
        {
            List<GameObject> targets = new List<GameObject> { card };
            foreach (GiveNextUnitEffect gnfe in gnfeList)
            {
                // CHECK FOR ALLY/ENEMY HERE
                foreach (Effect e in gnfe.Effects)
                    EffectManager.Instance.ResolveEffect(targets, e);
                if (--gnfe.Multiplier < 1) resolvedGnfe.Add(gnfe);
            }
            foreach (GiveNextUnitEffect rGnfe in resolvedGnfe)
            {
                gnfeList.Remove(rGnfe);
                DestroyEffect(rGnfe);
            }
        }
    }

    /******
     * *****
     * ****** REMOVE_GIVE_NEXT_EFFECTS
     * *****
     *****/
    public void RemoveGiveNextEffects()
    {
        static void DestroyEffect(Effect effect)
        {
            Destroy(effect);
            effect = null;
        }

        List<GiveNextUnitEffect> gne = EffectManager.Instance.GiveNextEffects;
        List<GiveNextUnitEffect> expiredGne = new List<GiveNextUnitEffect>();
        foreach (GiveNextUnitEffect gnfe in gne)
            if (gnfe.Countdown == 1) expiredGne.Add(gnfe);
            else if (gnfe.Countdown != 0) gnfe.Countdown -= 1;
        foreach (GiveNextUnitEffect xGnfe in expiredGne)
        {
            gne.Remove(xGnfe);
            DestroyEffect(xGnfe);
        }
    }

    /******
     * *****
     * ****** TRIGGER_CARD_ABILITY
     * *****
     *****/
    public bool TriggerCardAbility(GameObject card, string triggerName)
    {
        foreach (CardAbility ca in card.GetComponent<UnitCardDisplay>().CurrentAbilities)
            if (ca is TriggeredAbility tra)
                if (tra.AbilityTrigger.AbilityName == triggerName)
                {
                    EffectManager.Instance.StartEffectGroupList(tra.EffectGroupList, card);
                    return true;
                }
        return false;
    }
}
