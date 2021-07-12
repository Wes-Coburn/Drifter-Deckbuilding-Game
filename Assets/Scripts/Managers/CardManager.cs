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

    public const int PLAYER_START_FOLLOWERS = 6;
    public const int PLAYER_START_SKILLS = 3;
    public const int ENEMY_START_FOLLOWERS = 6;
    public const int ENEMY_START_SKILLS = 3;

    public const string BACKGROUND = "Background";
    public const string PLAYER_CARD = "PlayerCard";
    public const string ENEMY_CARD = "EnemyCard";
    public const string PLAYER_ACTION_ZONE = "PlayerActionZone";
    public const string PLAYER_HAND = "PlayerHand";
    public const string PLAYER_ZONE = "PlayerZone";
    public const string PLAYER_DISCARD = "PlayerDiscard";
    public const string PLAYER_CHAMPION = "PlayerChampion";
    //public const string ENEMY_ACTION_ZONE = "EnemyActionZone";
    public const string ENEMY_HAND = "EnemyHand";
    public const string ENEMY_ZONE = "EnemyZone";
    public const string ENEMY_DISCARD = "EnemyDiscard";
    public const string ENEMY_CHAMPION = "EnemyChampion";

    /* GAME_MANAGER_DATA */
    private const string PLAYER = GameManager.PLAYER;
    private const string ENEMY = GameManager.ENEMY;
    private const int STARTING_HAND_SIZE = GameManager.STARTING_HAND_SIZE;
    
    /* CARD LISTS */
    public List<GameObject> PlayerZoneCards;
    public List<GameObject> EnemyZoneCards;
    public List<GameObject> PlayerHandCards;
    public List<GameObject> EnemyHandCards;

    /* CARD_PREFABS */
    [SerializeField] private GameObject followerCardPrefab;
    [SerializeField] private GameObject actionCardPrefab;

    /* CARD_SCRIPTS */
    public Card StartPlayerFollower
    {
        get => startPlayerFollower;
        set => startPlayerFollower = value;
    }
    [Header("STARTING PLAYER FOLLOWER")]
    [SerializeField] private Card startPlayerFollower;

    /* CARD BACK SPRITE */
    public Sprite CardBackSprite
    {
        get => cardBackSprite;
        private set => cardBackSprite = value;
    }
    [SerializeField] private Sprite cardBackSprite;

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
    
    private void Start()
    {
        PlayerZoneCards = new List<GameObject>();
        EnemyZoneCards = new List<GameObject>();
        EnemyHandCards = new List<GameObject>();
    }
    public void StartGameScene()
    {
        /* GAME_ZONES */
        PlayerActionZone = GameObject.Find(PLAYER_ACTION_ZONE);
        PlayerHand = GameObject.Find(PLAYER_HAND);
        PlayerZone = GameObject.Find(PLAYER_ZONE);
        PlayerDiscard = GameObject.Find(PLAYER_DISCARD);
        PlayerHero = GameObject.Find(PLAYER_CHAMPION);

        //EnemyActionZone = GameObject.Find(ENEMY_ACTION_ZONE);
        EnemyHand = GameObject.Find(ENEMY_HAND);
        EnemyZone = GameObject.Find(ENEMY_ZONE);
        EnemyDiscard = GameObject.Find(ENEMY_DISCARD);
        EnemyHero = GameObject.Find(ENEMY_CHAMPION);
    }

    public FollowerCardDisplay GetFollowerDisplay(GameObject card)
        => card.GetComponent<CardDisplay>() as FollowerCardDisplay;

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

        currentDeck.Shuffle();
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

        if (card is FollowerCard) cardInstance = ScriptableObject.CreateInstance<FollowerCard>();
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
        if (card is FollowerCard) go = followerCardPrefab;
        else if (card is ActionCard) go = actionCardPrefab;
        else Debug.LogError("CARD TYPE NOT FOUND!");
        go = Instantiate(go, new Vector3(0, 0, CARD_Z_POSITION), Quaternion.identity);
        go.GetComponent<CardDisplay>().CardScript = card;
        return go;
    }
    private GameObject HideCard (Card card) // UNUSED!!!
    {
        return null;
    }

    /******
     * *****
     * ****** DRAW_CARD
     * *****
     *****/
    public GameObject DrawCard(string hero)
    {
        List<Card> deck = null;
        string cardTag = null;
        string cardZone = null;

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
        else Debug.LogError("PLAYER <" + hero + "> NOT FOUND!");


        if (deck.Count < 1)
        {
            Debug.LogWarning("NO CARDS LEFT IN DECK!");
            return null;
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
    public void DrawHand(string player)
    {
        for (int i = 0; i < STARTING_HAND_SIZE; i++)
        {
            FunctionTimer.Create(() => DrawCard(player), i);
        }
    }

    /******
     * *****
     * ****** REFRESH_CARDS
     * *****
     *****/
    public void RefreshCards(string hero)
    {
        List<GameObject> cardZoneList = null;
        if (hero == PLAYER) cardZoneList = PlayerZoneCards;
        else if (hero == ENEMY) cardZoneList = EnemyZoneCards;
        foreach (GameObject card in cardZoneList) card.GetComponent<FollowerCardDisplay>().IsExhausted = false;
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
            if (!EffectManager.Instance.CheckLegalTargets(acd.ActionCard.EffectGroup, true))
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
    public void SetCardParent(GameObject card, Transform parentTransform)
    {
        card.transform.SetParent(parentTransform, false);
        float xPos = card.transform.position.x;
        float yPos = card.transform.position.y;
        card.transform.position = new Vector3(xPos, yPos, CARD_Z_POSITION);

        // UNNECESSARY?
        CardDisplay cd = card.GetComponent<CardDisplay>();
        if (cd is FollowerCardDisplay)
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
            case PLAYER_ACTION_ZONE:
                zoneTran = PlayerActionZone.transform;
                AnimationManager.Instance.PlayedState(card);
                break;
            case PLAYER_HAND:
                zoneTran = PlayerHand.transform;
                AnimationManager.Instance.RevealedHandState(card);
                break;
            case PLAYER_ZONE:
                zoneTran = PlayerZone.transform;
                AnimationManager.Instance.PlayedState(card);
                break;
            case PLAYER_DISCARD:
                zoneTran = PlayerDiscard.transform;
                AnimationManager.Instance.RevealedPlayState(card);
                break;
            /*
            case ENEMY_ACTION_ZONE:
                zoneTran = EnemyActionZone.transform;
                AnimationManager.Instance.PlayedState(card);
            */
            case ENEMY_HAND:
                zoneTran = EnemyHand.transform;
                AnimationManager.Instance.HiddenState(card);
                break;
            case ENEMY_ZONE:
                zoneTran = EnemyZone.transform;
                AnimationManager.Instance.PlayedState(card);
                break;
            case ENEMY_DISCARD:
                zoneTran = EnemyDiscard.transform;
                AnimationManager.Instance.RevealedPlayState(card);
                break;
        }
        SetCardParent(card, zoneTran);

        if (card.GetComponent<CardDisplay>() is FollowerCardDisplay fcd)
        {
            bool played;
            if (zone == PLAYER_ZONE || zone == ENEMY_ZONE) played = true;
            else played = false;
            fcd.ResetFollowerCard(played);
        }
    }

    /******
     * *****
     * ****** PLAY_CARD [HAND >>> PLAY]
     * *****
     *****/
    public void PlayCard(GameObject card, string hero)
    {
        if (hero == PLAYER)
        {
            PlayerManager.Instance.PlayerActionsLeft -= card.GetComponent<CardDisplay>().CurrentActionCost;
            PlayerHandCards.Remove(card);

            if (card.GetComponent<CardDisplay>() is FollowerCardDisplay)
            {
                ChangeCardZone(card, PLAYER_ZONE);
                PlayerZoneCards.Add(card);
                TriggerCardAbility(card, "Play");
                TriggerGiveNextEffect(card);
            }
            else if (card.GetComponent<CardDisplay>() is ActionCardDisplay)
            {
                ChangeCardZone(card, PLAYER_ACTION_ZONE);
                ResolveActionCard(card);
            }
            else Debug.LogError("CARDDISPLAY TYPE NOT FOUND!");
        }
        else if (hero == ENEMY)
        {
            card = EnemyHandCards[0];
            EnemyHandCards.Remove(card);
            EnemyZoneCards.Add(card);
            EnemyManager.Instance.EnemyActionsLeft -= card.GetComponent<CardDisplay>().CurrentActionCost;
            ChangeCardZone(card, ENEMY_ZONE);
        }
    }

    /******
     * *****
     * ****** RESOLVE_ACTION_CARD
     * *****
     *****/
    private void ResolveActionCard(GameObject card)
    {
        List<Effect> effectGroup = card.GetComponent<ActionCardDisplay>().ActionCard.EffectGroup;
        EffectManager.Instance.StartEffectGroup(effectGroup, card);
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
        }
        else if (hero == ENEMY)
        {
            ChangeCardZone(card, ENEMY_DISCARD);
            EnemyZoneCards.Remove(card);
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
        }
        else if (hero == ENEMY)
        {
            ChangeCardZone(card, ENEMY_DISCARD);
            EnemyHandCards.Remove(card);
        }
    }

    /******
     * *****
     * ****** ATTACK
     * *****
     *****/
    public void Attack(GameObject attacker, GameObject defender)
    {
        int power = GetFollowerDisplay(attacker).CurrentPower;
        TakeDamage(defender, power);
        attacker.GetComponent<FollowerCardDisplay>().IsExhausted = true;
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
        else if (target == EnemyHero) targetValue = pm.PlayerHealth;
        else targetValue = GetFollowerDisplay(target).CurrentDefense;

        newTargetValue = targetValue - damageValue;
        if (target == PlayerHero) pm.PlayerHealth = newTargetValue;
        else if (target == EnemyHero) em.EnemyHealth = newTargetValue;
        else
        {
            GetFollowerDisplay(target).CurrentDefense = newTargetValue;
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
        FollowerCardDisplay fcd = GetFollowerDisplay(heroCard);
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
        FollowerCardDisplay fcd = GetFollowerDisplay(card);

        // GIVE_ABILITY_EFFECT
        if (effect is GiveAbilityEffect gae)
        {
            Debug.Log("GiveAbilityEffect! <" + gae.CardAbility.ToString() + ">");

            // TESTING TESTING TESTING
            GiveAbilityEffect newGae = ScriptableObject.CreateInstance<GiveAbilityEffect>();
            newGae.LoadEffect(gae);
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
            FollowerCardDisplay fcd = GetFollowerDisplay(card);
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
     * ****** REMOVE_GIVE_NEXT_EFFECTS
     * *****
     *****/
    public void TriggerGiveNextEffect(GameObject card)
    {
        Debug.Log("TriggerGiveNextEffect()");
        static void DestroyEffect(Effect effect)
        {
            Destroy(effect);
            effect = null;
        }

        // GIVE_NEXT_FOLLOWER_EFFECTS
        List<GiveNextFollowerEffect> gnfe = EffectManager.Instance.GiveNextEffects;
        List<GiveNextFollowerEffect> resolvedGnfe = new List<GiveNextFollowerEffect>();

        if (gnfe.Count > 0)
        {
            List<GameObject> targets = new List<GameObject> { card };
            foreach (GiveNextFollowerEffect gnfe2 in gnfe)
            {
                // CHECK FOR ALLY/ENEMY HERE
                EffectManager.Instance.ResolveEffect(targets, gnfe2.Effect);
                resolvedGnfe.Add(gnfe2);
            }
            foreach (GiveNextFollowerEffect rGnfe in resolvedGnfe)
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
        Debug.Log("RemoveGiveNextEffects()");
        static void DestroyEffect(Effect effect)
        {
            Destroy(effect);
            effect = null;
        }

        List<GiveNextFollowerEffect> gne = EffectManager.Instance.GiveNextEffects;
        List<GiveNextFollowerEffect> expiredGne = new List<GiveNextFollowerEffect>();
        foreach (GiveNextFollowerEffect gnfe in gne)
        {
            if (gnfe.Countdown == 1)
            {
                expiredGne.Add(gnfe);
            }
            else if (gnfe.Countdown != 0) gnfe.Countdown -= 1;
        }
        foreach (GiveNextFollowerEffect xGnfe in expiredGne)
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
        Debug.Log("TriggerCardAbility()");
        foreach (CardAbility ca in card.GetComponent<FollowerCardDisplay>().CurrentAbilities)
        {
            if (ca is TriggeredAbility tra)
            {
                if (tra.AbilityTrigger.AbilityName == triggerName)
                {
                    EffectManager.Instance.StartEffectGroup(tra.EffectGroup);
                }
            }
        }
    }
}
