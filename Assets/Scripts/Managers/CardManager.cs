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
    public const string BACKGROUND = "Background";
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

    /* CARD LISTS */
    // PLAYER
    public List<GameObject> PlayerHandCards { get; private set; }
    public List<GameObject> PlayerZoneCards { get; private set; }
    public List<GameObject> PlayerDiscardCards { get; private set; }
    // ENEMY
    public List<GameObject> EnemyHandCards { get; private set; }
    public List<GameObject> EnemyZoneCards { get; private set; }
    public List<GameObject> EnemyDiscardCards { get; private set; }
    
    /* GAME ZONES */
    // PLAYER
    public GameObject PlayerActionZone { get; private set; }
    public GameObject PlayerHand { get; private set; }
    public GameObject PlayerZone { get; private set; }
    public GameObject PlayerDiscard { get; private set; }
    public GameObject PlayerHero { get; private set; }
    // ENEMY
    //public GameObject EnemyActionZone { get; private set; }
    public GameObject EnemyDiscard { get; private set; }
    public GameObject EnemyHand { get; private set; }
    public GameObject EnemyZone { get; private set; }
    public GameObject EnemyHero { get; private set; }

    /* CARD_PREFABS */
    [SerializeField] private GameObject unitCardPrefab;
    [SerializeField] private GameObject actionCardPrefab;

    /* CARD_SCRIPTS */
    public Card StartPlayerUnit_1
    {
        get => startUnit1;
        set => startUnit1 = value;
    }
    [Header("STARTING PLAYER UNITS")]
    [SerializeField] private Card startUnit1;
    public Card StartPlayerUnit_2
    {
        get => startUnit2;
        set => startUnit2 = value;
    }
    [SerializeField] private Card startUnit2;

    /* CARD BACK SPRITE */
    public Sprite CardBackSprite
    {
        get => cardBackSprite;
        private set => cardBackSprite = value;
    }
    [SerializeField] private Sprite cardBackSprite;

    private void Start()
    {
        // PLAYER
        PlayerHandCards = new List<GameObject>();
        PlayerZoneCards = new List<GameObject>();
        PlayerDiscardCards = new List<GameObject>();
        // ENEMY
        EnemyHandCards = new List<GameObject>();
        EnemyZoneCards = new List<GameObject>();
        EnemyDiscardCards = new List<GameObject>();
    }
    public void StartGameScene()
    {
        /* GAME_ZONES */
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
        foreach (Card card in deckList)
        {
            currentDeck.Add(card);
        }

        ShuffleDeck(currentDeck);
    }

    /******
     * *****
     * ****** ADD_CARD
     * *****
     *****/
    public void AddCard(Card card, string hero)
    {
        List<Card> deck = null;
        Card cardInstance = null;
        
        if (hero == GameManager.PLAYER) deck = PlayerManager.Instance.PlayerDeckList;
        else if (hero == GameManager.ENEMY) deck = EnemyManager.Instance.EnemyDeckList;
        else Debug.LogError("HERO NOT FOUND!");

        if (card is UnitCard) cardInstance = ScriptableObject.CreateInstance<UnitCard>();
        else if (card is ActionCard) cardInstance = ScriptableObject.CreateInstance<ActionCard>();
        else Debug.LogError("CARD TYPE NOT FOUND!");

        if (deck == null) Debug.LogError("DECK IS NULL!");
        if (cardInstance == null) Debug.LogError("CARD IS NULL!");
        else if (deck != null && cardInstance != null)
        {
            cardInstance.LoadCard(card);
            deck.Add(cardInstance);
        }
    }

    /******
     * *****
     * ****** SHOW/HIDE_CARD
     * *****
     *****/
    private GameObject ShowCard (Card card)
    {
        GameObject go = null;
        if (card is UnitCard) go = unitCardPrefab;
        else if (card is ActionCard) go = actionCardPrefab;
        else Debug.LogError("CARD TYPE NOT FOUND!");
        go = Instantiate(go, new Vector3(0, 0, CARD_Z_POSITION), Quaternion.identity);
        go.GetComponent<CardDisplay>().CardScript = card;
        return go;
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
    public GameObject DrawCard(string hero)
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
            return null;
        }

        // Shuffle discard into deck
        if (deck.Count < 1)
        {
            List<GameObject> discard;
            if (hero == PLAYER) discard = PlayerDiscardCards;
            else if (hero == ENEMY) discard = EnemyDiscardCards;
            else
            {
                Debug.LogError("PLAYER <" + hero + "> NOT FOUND!");
                return null;
            }

            foreach (GameObject go in discard)
            {
                deck.Add(HideCard(go));
            }
            discard.Clear();
            ShuffleDeck(deck);
        }

        GameObject card = ShowCard(deck[0]);
        deck.RemoveAt(0);

        card.tag = cardTag;
        ChangeCardZone(card, cardZone);

        if (hero == PLAYER) PlayerHandCards.Add(card);
        else EnemyHandCards.Add(card);

        if (card == null)
        {
            Debug.LogError("CARD IS NULL!");
            return null;
        }
        return card;
    }
    public void DrawHand(string player, int handSize)
    {
        for (int i = 0; i < handSize; i++)
        {
            FunctionTimer.Create(() => DrawCard(player), i);
        }
    }

    /******
     * *****
     * ****** SHUFFLE_DECK
     * *****
     *****/
    public void ShuffleDeck(List<Card> deck)
    {
        Debug.Log("SHUFFLE DECK!");
        deck.Shuffle();
        // Shuffle sounds, animations
    }

    /******
     * *****
     * ****** REFRESH_FOLLOWERS
     * *****
     *****/
    public void RefreshFollowers(string hero)
    {
        List<GameObject> cardZoneList = null;
        if (hero == PLAYER) cardZoneList = PlayerZoneCards;
        else if (hero == ENEMY) cardZoneList = EnemyZoneCards;
        foreach (GameObject card in cardZoneList) card.GetComponent<UnitCardDisplay>().IsExhausted = false;
    }

    /******
     * *****
     * ****** IS_PLAYABLE/IS_EXHAUSTED
     * *****
     *****/
    public bool IsPlayable(GameObject card)
    {
        int actionCost = card.GetComponent<CardDisplay>().CurrentActionCost;
        int playerActionsLeft = PlayerManager.Instance.PlayerActionsLeft;
        
        if (card.GetComponent<CardDisplay>() is ActionCardDisplay acd)
        {
            if (!EffectManager.Instance.CheckLegalTargets(acd.ActionCard.EffectGroupList, card, true))
                return false;
        }

        if (playerActionsLeft >= actionCost) return true;
        else
        {
            Debug.LogWarning("COULD NOT PLAY! // ActionCost: "
            + actionCost + " // PlayerActionsLeft: " + PlayerManager.Instance.PlayerActionsLeft + 
            " // IsMyTurn = " + PlayerManager.Instance.IsMyTurn);
            return false;
        }
    }

    /******
     * *****
     * ****** SET_CARD_PARENT
     * *****
     *****/
    public static int GetAbility(GameObject card, string ability)
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
        {
            card.GetComponent<ChangeLayer>().CardsLayer();
        }
        else if (cd is ActionCardDisplay)
        {
            card.GetComponent<ChangeLayer>().ActionsLayer();
        }
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
        // PLAYER
        if (card.CompareTag(PLAYER_CARD))
        {
            PlayerManager.Instance.PlayerActionsLeft -= card.GetComponent<CardDisplay>().CurrentActionCost;
            PlayerHandCards.Remove(card);

            if (card.GetComponent<CardDisplay>() is UnitCardDisplay)
            {
                PlayerZoneCards.Add(card);
                ChangeCardZone(card, PLAYER_ZONE);

                // Implement for enemy!
                TriggerCardAbility(card, "Play");
                TriggerGiveNextEffect(card);
            }
            else if (card.GetComponent<CardDisplay>() is ActionCardDisplay)
            {
                // Implement for enemy!
                ChangeCardZone(card, PLAYER_ACTION_ZONE);
                ResolveActionCard(card);
            }
            else Debug.LogError("CARDDISPLAY TYPE NOT FOUND!");
        }

        // ENEMY
        else if (card.CompareTag(ENEMY_CARD))
        {
            EnemyHandCards.Remove(card);
            EnemyZoneCards.Add(card);
            ChangeCardZone(card, ENEMY_ZONE);
        }

        else Debug.LogError("CARD TAG NOT FOUND!");
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
        if (hero == PLAYER)
        {
            ChangeCardZone(card, PLAYER_DISCARD);
            PlayerZoneCards.Remove(card);
            PlayerDiscardCards.Add(card);
        }
        else if (hero == ENEMY)
        {
            ChangeCardZone(card, ENEMY_DISCARD);
            EnemyZoneCards.Remove(card);
            EnemyDiscardCards.Add(card);
        }
    }

    /******
     * *****
     * ****** DISCARD_CARD [HAND >>> DISCARD]
     * *****
     *****/
    public void DiscardCard(GameObject card, string hero)
    {
        if (hero == PLAYER)
        {
            ChangeCardZone(card, PLAYER_DISCARD);
            PlayerHandCards.Remove(card);
            PlayerDiscardCards.Add(card);
        }
        else if (hero == ENEMY)
        {
            ChangeCardZone(card, ENEMY_DISCARD);
            EnemyHandCards.Remove(card);
            EnemyDiscardCards.Add(card);
        }
    }

    /******
     * *****
     * ****** ATTACK
     * *****
     *****/
    public void Attack(GameObject attacker, GameObject defender)
    {
        Strike(attacker, defender);
        attacker.GetComponent<UnitCardDisplay>().IsExhausted = true;

        if (defender.CompareTag(ENEMY_CARD) || defender.CompareTag(PLAYER_CARD))
        {
            if (defender.GetComponent<UnitCardDisplay>().CurrentDefense > 0)
            {
                if (GetAbility(attacker, "Ranged") == -1)
                {
                    Strike(defender, attacker);
                }
            }
        }
    }

    /******
     * *****
     * ****** STRIKE
     * *****
     *****/
    public void Strike(GameObject striker, GameObject defender)
    {
        int power = GetUnitDisplay(striker).CurrentPower;
        TakeDamage(defender, power);
    }

    /******
     * *****
     * ****** TAKE_DAMAGE
     * *****
     *****/
    public void TakeDamage(GameObject target, int damageValue)
    {
        if (damageValue < 1) return;
        int targetValue;
        int newTargetValue;
        PlayerManager pm = PlayerManager.Instance;
        EnemyManager em = EnemyManager.Instance;

        if (target == PlayerHero) targetValue = pm.PlayerHealth;
        else if (target == EnemyHero) targetValue = em.EnemyHealth;
        else targetValue = GetUnitDisplay(target).CurrentDefense;

        newTargetValue = targetValue - damageValue;
        if (target == PlayerHero) pm.PlayerHealth = newTargetValue;
        else if (target == EnemyHero) em.EnemyHealth = newTargetValue;
        else
        {
            GetUnitDisplay(target).CurrentDefense = newTargetValue;
            AnimationManager.Instance.ModifyDefenseState(target);
        }

        if (newTargetValue < 1)
        {
            if (target.CompareTag(PLAYER_CARD)) DestroyCard(target, PLAYER);
            else if (target.CompareTag(ENEMY_CARD)) DestroyCard(target, ENEMY);
            else if (target == PlayerHero) GameManager.Instance.EndCombat(false);
            else if (target == EnemyHero) GameManager.Instance.EndCombat(true);
        }
    }

    /******
     * *****
     * ****** HEAL_DAMAGE
     * *****
     *****/
    public void HealDefense(GameObject heroCard, int healingValue) // UNUSED!!!
    {
        // ADD HEALING FOR HEROES
        if (healingValue < 1) return;
        UnitCardDisplay fcd = GetUnitDisplay(heroCard);
        int def = fcd.CurrentDefense;
        int newDef = def + healingValue;
        if (newDef > fcd.MaxDefense) newDef = fcd.MaxDefense;
        fcd.CurrentDefense = newDef;
        AnimationManager.Instance.ModifyDefenseState(heroCard);
    }

    /******
     * *****
     * ****** ADD_EFFECT
     * *****
     *****/
    public void AddEffect(GameObject card, Effect effect)
    {
        Debug.LogWarning("EFFECT ADDED: <" + effect.ToString() + ">");
        UnitCardDisplay fcd = GetUnitDisplay(card);

        // GIVE_ABILITY_EFFECT
        if (effect is GiveAbilityEffect gae)
        {
            Debug.Log("GiveAbilityEffect! <" + gae.CardAbility.ToString() + ">");

            GiveAbilityEffect newGae = ScriptableObject.CreateInstance<GiveAbilityEffect>();
            newGae.LoadEffect(gae);

            // If ability already exists, update countdown instead of adding
            if (!fcd.AddCurrentAbility(newGae.CardAbility))
            {
                foreach (Effect effect2 in fcd.CurrentEffects)
                {
                    if (effect2 is GiveAbilityEffect gae2)
                    {
                        if (gae2.CardAbility == newGae.CardAbility)
                        {
                            if (newGae.Countdown == 0 || newGae.Countdown > gae2.Countdown)
                            {
                                gae2.Countdown = newGae.Countdown;
                            }
                        }
                    }
                }
            }
            else fcd.CurrentEffects.Add(newGae);
        }
        // STAT_CHANGE_EFFECT
        else if (effect is StatChangeEffect sce)
        {
            // TESTING TESTING TESTING
            StatChangeEffect newSce = ScriptableObject.CreateInstance<StatChangeEffect>();
            newSce.LoadEffect(sce);
            fcd.CurrentEffects.Add(newSce);

            if (sce.IsDefenseChange)
            {
                fcd.MaxDefense += sce.Value;
                fcd.CurrentDefense += sce.Value;
            }
            else fcd.CurrentPower += sce.Value;
        }
        else Debug.LogError("EFFECT TYPE NOT FOUND!");
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
                    if (effect is GiveAbilityEffect gae) fcd.RemoveCurrentAbility(gae.CardAbility);
                    
                    // STAT_CHANGE_EFFECT
                    else if (effect is StatChangeEffect sce)
                    {
                        if (sce.IsDefenseChange)
                        {
                            fcd.CurrentDefense -= sce.Value;
                            fcd.MaxDefense -= sce.Value;
                        }
                        else fcd.CurrentPower -= sce.Value;
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
        List<GiveNextUnitEffect> gnfe = EffectManager.Instance.GiveNextEffects;
        List<GiveNextUnitEffect> resolvedGnfe = new List<GiveNextUnitEffect>();

        if (gnfe.Count > 0)
        {
            List<GameObject> targets = new List<GameObject> { card };
            foreach (GiveNextUnitEffect gnfe2 in gnfe)
            {
                // CHECK FOR ALLY/ENEMY HERE
                EffectManager.Instance.ResolveEffect(targets, gnfe2.Effect);
                resolvedGnfe.Add(gnfe2);
            }
            foreach (GiveNextUnitEffect rGnfe in resolvedGnfe)
            {
                gnfe.Remove(rGnfe);
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
        {
            if (gnfe.Countdown == 1) expiredGne.Add(gnfe);
            else if (gnfe.Countdown != 0) gnfe.Countdown -= 1;
        }
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
    public void TriggerCardAbility(GameObject card, string triggerName)
    {
        foreach (CardAbility ca in card.GetComponent<UnitCardDisplay>().CurrentAbilities)
        {
            if (ca is TriggeredAbility tra)
            {
                if (tra.AbilityTrigger.AbilityName == triggerName)
                {
                    EffectManager.Instance.StartEffectGroupList(tra.EffectGroupList, PlayerHero);
                }
            }
        }
    }
}
