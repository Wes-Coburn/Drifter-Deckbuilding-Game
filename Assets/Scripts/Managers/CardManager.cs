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
    public const int PLAYER_START_FOLLOWERS = 6; // TESTING
    public const int PLAYER_START_SKILLS = 3; // TESTING
    public const int ENEMY_START_FOLLOWERS = 6; // TESTING
    public const int ENEMY_START_SKILLS = 3; // TESTING

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
    [SerializeField] private GameObject followerCardPrefab; // TESTING
    [SerializeField] private GameObject actionCardPrefab; // TESTING

    /* CARD_SCRIPTS */
    public Card StartPlayerFollower // TESTING
    {
        get => startPlayerFollower;
        set => startPlayerFollower = value;
    }
    [Header("STARTING PLAYER FOLLOWER")]
    [SerializeField] private Card startPlayerFollower; // TESTING


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

    public FollowerCardDisplay GetFollowerDisplay(GameObject card) => card.GetComponent<CardDisplay>() as FollowerCardDisplay;

    /******
     * *****
     * ****** ADD_CARD
     * *****
     *****/
    public void AddCard(Card card, string player) // TESTING
    {
        List<Card> deck = null;
        Card cardInstance = null;
        
        if (player == GameManager.PLAYER) deck = PlayerManager.Instance.PlayerDeck2;
        else if (player == GameManager.ENEMY) deck = EnemyManager.Instance.EnemyDeck2;
        else Debug.LogError("Player NOT FOUND!");

        if (card is FollowerCard) cardInstance = ScriptableObject.CreateInstance<FollowerCard>();
        else if (card is ActionCard) cardInstance = ScriptableObject.CreateInstance<ActionCard>();
        else Debug.LogError("Card Type NOT FOUND!");

        if (deck != null && cardInstance != null)
        {
            cardInstance.LoadCard(card);
            deck.Add(cardInstance);
        }
        else Debug.LogError("Deck or Card was NULL!");
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
        go = Instantiate(go, new Vector3(0, 0, -1), Quaternion.identity);
        go.GetComponent<CardDisplay>().CardScript = card;
        return go;
    }
    private GameObject HideCard (Card card)
    {
        return null;
    }

    /******
     * *****
     * ****** DRAW_CARD
     * *****
     *****/
    public GameObject DrawCard(string player)
    {
        List<int> deck = null;
        string cardTag = null;
        string cardZone = null;

        if (player == PLAYER)
        {
            deck = PlayerManager.Instance.PlayerDeck;
            cardTag = PLAYER_CARD;
            cardZone = PLAYER_HAND;
        }
        else if (player == ENEMY)
        {
            deck = EnemyManager.Instance.EnemyDeck;
            cardTag = ENEMY_CARD;
            cardZone = ENEMY_HAND;
        }
        else Debug.LogError("PLAYER <" + player + "> NOT FOUND!");

        int cardID = deck[0];
        deck.RemoveAt(0);

        if (deck.Count < 1)
        {
            Debug.LogWarning("[CardManager.DrawCard()] NO CARDS LEFT!");
            return null;
        }

        GameObject newCard = CardLibrary.Instance.GetCard(cardID);

        newCard.tag = cardTag;
        ChangeCardZone(newCard, cardZone);

        if (player == PLAYER) PlayerHandCards.Add(newCard);
        else EnemyHandCards.Add(newCard);

        if (newCard == null)
        {
            Debug.LogError("NewCard IS NULL!");
            return null;
        }

        return newCard;
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
    public void RefreshCards(string player)
    {
        List<GameObject> cardZoneList = null;
        if (player == PLAYER) cardZoneList = PlayerZoneCards;
        else if (player == ENEMY) cardZoneList = EnemyZoneCards;
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
            Debug.LogWarning("[IsPlayable() in CardManager] COULD NOT PLAY! // ActionCost: "
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
        card.transform.position = new Vector3(xPos, yPos, -2);

        // UNNECESSARY???
        if (card.TryGetComponent<FollowerCardDisplay>(out FollowerCardDisplay fcd))
        {
            card.GetComponent<ChangeLayer>().CardsLayer();
        }
        else if (card.TryGetComponent<ActionCardDisplay>(out ActionCardDisplay acd))
        {
            card.GetComponent<ChangeLayer>().ActionsLayer();
        }
        else Debug.LogError("Card Display NOT FOUND!");
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
            case PLAYER_ACTION_ZONE: // TESTING
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

        if (card.TryGetComponent<FollowerCardDisplay>(out FollowerCardDisplay hcd))
        {
            if (zone == PLAYER_DISCARD || zone == ENEMY_DISCARD) hcd.ResetFollowerCard(true);
            else if (zone != PLAYER_ZONE && zone != ENEMY_ZONE) hcd.ResetFollowerCard();
        }
    }

    /******
     * *****
     * ****** PLAY_CARD [HAND >>> PLAY]
     * *****
     *****/
    public void PlayCard(GameObject card, string player)
    {
        if (player == PLAYER)
        {
            PlayerManager.Instance.PlayerActionsLeft -= card.GetComponent<CardDisplay>().CurrentActionCost;
            PlayerHandCards.Remove(card);

            if (card.GetComponent<CardDisplay>() is FollowerCardDisplay)
            {
                ChangeCardZone(card, PLAYER_ZONE);
                PlayerZoneCards.Add(card);
                TriggerCardAbility(card, "Play");
            }
            else if (card.GetComponent<CardDisplay>() is ActionCardDisplay)
            {
                ChangeCardZone(card, PLAYER_ACTION_ZONE);
                ResolveActionCard(card);
            }
            else Debug.LogError("CardDisplay type not found!");
        }
        else if (player == ENEMY)
        {
            card = EnemyHandCards[0];
            EnemyHandCards.Remove(card);
            EnemyZoneCards.Add(card);
            EnemyManager.Instance.EnemyActionsLeft -= card.GetComponent<CardDisplay>().CurrentActionCost;
            ChangeCardZone(card, ENEMY_ZONE);
        }
    }

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
    public void DestroyCard(GameObject card, string player)
    {
        if (player == PLAYER)
        {
            ChangeCardZone(card, PLAYER_DISCARD);
            PlayerZoneCards.Remove(card);
        }
        else if (player == ENEMY)
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
    public void DiscardCard(GameObject card, string player)
    {
        if (player == PLAYER)
        {
            ChangeCardZone(card, PLAYER_DISCARD);
            PlayerHandCards.Remove(card);
        }
        else if (player == ENEMY)
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
    public void HealDamage(GameObject heroCard, int healingValue)
    {
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
        Debug.LogWarning("AddEffect() Effect: " + effect.ToString());
        FollowerCardDisplay fcd = GetFollowerDisplay(card);

        // GIVE_ABILITY_EFFECT
        if (effect is GiveAbilityEffect gae)
        {
            Debug.Log("GiveAbilityEffect! Ability: " + gae.CardAbility.ToString());

            // TESTING TESTING TESTING
            GiveAbilityEffect newGae = ScriptableObject.CreateInstance<GiveAbilityEffect>();
            newGae.LoadEffect(gae);
            fcd.CurrentEffects.Add(newGae);
            fcd.AddCurrentAbility(newGae.CardAbility);
        }
        // STAT_CHANGE_EFFECT
        else if (effect is StatChangeEffect sce)
        {
            Debug.Log("StatChangeEffect! Value: " + sce.Value);

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
        else Debug.LogError("Effect type not found!");
    }

    /******
     * *****
     * ****** REMOVE_TEMPORARY_EFFECTS
     * *****
     *****/
    public void RemoveTemporaryEffects(string player)
    {
        List<GameObject> cardZone;
        if (player == PLAYER) cardZone = PlayerZoneCards;
        else cardZone = EnemyZoneCards;

        foreach (GameObject go in cardZone)
        {
            FollowerCardDisplay fcd = GetFollowerDisplay(go);
            List<Effect> expiredEffects = new List<Effect>();

            foreach (Effect effect in fcd.CurrentEffects)
            {
                if (effect.Countdown == 1) // Check for EXPIRED effects
                {
                    // GIVE_ABILITY_EFFECT
                    if (effect is GiveAbilityEffect gae)
                    {
                        fcd.RemoveCurrentAbility(gae.CardAbility);
                    }
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
                else if (effect.Countdown != 0) effect.Countdown -= 1; // TESTING
            }
            foreach (Effect effect in expiredEffects) fcd.CurrentEffects.Remove(effect);
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
