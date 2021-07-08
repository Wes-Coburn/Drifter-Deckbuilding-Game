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
    
    /* CARD BACK SPRITE */
    [SerializeField] private Sprite cardBackSprite;
    public Sprite CardBackSprite
    {
        get => cardBackSprite;
        private set => cardBackSprite = value;
    }

    /* GAME ZONES */
    // PLAYER
    public GameObject PlayerActionZone { get; private set; }
    public GameObject PlayerHand { get; private set; }
    public GameObject PlayerZone { get; private set; }
    public GameObject PlayerDiscard { get; private set; }
    public GameObject PlayerChampion { get; private set; }
    // ENEMY
    //public GameObject EnemyActionZone { get; private set; }
    public GameObject EnemyDiscard { get; private set; }
    public GameObject EnemyHand { get; private set; }
    public GameObject EnemyZone { get; private set; }
    public GameObject EnemyChampion { get; private set; }
    
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
        PlayerChampion = GameObject.Find(PLAYER_CHAMPION);

        //EnemyActionZone = GameObject.Find(ENEMY_ACTION_ZONE);
        EnemyHand = GameObject.Find(ENEMY_HAND);
        EnemyZone = GameObject.Find(ENEMY_ZONE);
        EnemyDiscard = GameObject.Find(ENEMY_DISCARD);
        EnemyChampion = GameObject.Find(ENEMY_CHAMPION);
    }

    public FollowerCardDisplay GetFollowerCardDisplay(GameObject card) => card.GetComponent<CardDisplay>() as FollowerCardDisplay;

    /******
     * *****
     * ****** ADD_CARD
     * *****
     *****/
    public void AddCard(Card card, string player)
    {
        List<Card> deck = null;
        Card cardInstance = null;
        
        if (player == GameManager.PLAYER) deck = PlayerManager.Instance.PlayerDeck2;
        else if (player == GameManager.ENEMY) deck = EnemyManager.Instance.EnemyDeck2;
        else Debug.LogError("Player NOT FOUND!");

        if (card is FollowerCard) cardInstance = ScriptableObject.CreateInstance<FollowerCard>();
        else if (card is ActionCard) cardInstance = ScriptableObject.CreateInstance<ActionCard>();
        else Debug.LogError("Card Type NOT FOUND!");

        cardInstance.LoadCard(card);

        if (deck != null && cardInstance != null) deck.Add(cardInstance);
        else Debug.LogError("Deck or Card was NULL!");
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
        else Debug.LogError(player + " NOT FOUND!");

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
            Debug.LogError("NewCard is NULL!");
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
        int power = GetFollowerCardDisplay(attacker).CurrentPower;
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

        if (target == PlayerChampion) targetValue = pm.PlayerHealth;
        else if (target == EnemyChampion) targetValue = pm.PlayerHealth;
        else targetValue = GetFollowerCardDisplay(target).CurrentDefense;

        newTargetValue = targetValue - damageValue;
        if (target == PlayerChampion) pm.PlayerHealth = newTargetValue;
        else if (target == EnemyChampion) em.EnemyHealth = newTargetValue;
        else
        {
            GetFollowerCardDisplay(target).CurrentDefense = newTargetValue;
            AnimationManager.Instance.ModifyDefenseState(target);
        }

        if (newTargetValue < 1)
        {
            if (target.CompareTag(PLAYER_CARD)) DestroyCard(target, PLAYER);
            else if (target.CompareTag(ENEMY_CARD)) DestroyCard(target, ENEMY);
            else if (target == PlayerChampion) GameManager.Instance.EndGame(false);
            else if (target == EnemyChampion) GameManager.Instance.EndGame(true);
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
        FollowerCardDisplay fcd = GetFollowerCardDisplay(heroCard);
        int defenseScore = fcd.CurrentDefense;
        int newDefenseScore = defenseScore + healingValue;
        if (newDefenseScore > fcd.MaxDefense)
        {
            newDefenseScore = fcd.MaxDefense;
        }
        fcd.CurrentDefense = newDefenseScore;
        AnimationManager.Instance.ModifyDefenseState(heroCard);
    }

    /******
     * *****
     * ****** ADD_EFFECT
     * *****
     *****/
    public void AddEffect(GameObject heroCard, Effect effect)
    {
        Debug.LogWarning("AddEffect()");
        FollowerCardDisplay hcd = GetFollowerCardDisplay(heroCard);
        // Check for TEMPORARY effect
        if (effect.Countdown != 0)
        {
            hcd.TemporaryEffects.Add(effect);
            hcd.EffectCountdowns.Add(effect.Countdown);
        }

        // STAT_CHANGE_EFFECT
        if (effect is StatChangeEffect sce)
        {
            Debug.Log("StatChangeEffect! Value: " + sce.Value);
            if (sce.IsDefenseChange)
            {
                hcd.MaxDefense += sce.Value;
                hcd.CurrentDefense += sce.Value;
            }
            else
            {
                hcd.CurrentPower += sce.Value;
            }
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
            FollowerCardDisplay fcd = GetFollowerCardDisplay(go);
            if (fcd.TemporaryEffects.Count > 0)
            {
                int countdown = 0;
                List<int> expiredEffects = new List<int>();

                foreach (Effect effect in fcd.TemporaryEffects)
                {
                    if (--fcd.EffectCountdowns[countdown] < 1)
                    {
                        expiredEffects.Add(countdown);

                        // STAT_CHANGE_EFFECT
                        if (effect is StatChangeEffect sce)
                        {
                            if (sce.IsDefenseChange)
                            {
                                fcd.CurrentDefense -= sce.Value;
                                fcd.MaxDefense -= sce.Value;
                            }
                            else fcd.CurrentPower -= sce.Value;
                        }
                    }
                    countdown++;
                    Debug.Log("COUNTDOWN FOR EFFECT <" + effect.ToString() + "> = " + fcd.EffectCountdowns[countdown-1]);
                }

                expiredEffects.Reverse(); // REMOVE THE HIGHEST INDEXES FIRST, OTHERWISE RESULTS WILL BE INACCURATE
                foreach (int effect in expiredEffects)
                {
                    Debug.LogWarning("EFFECT REMOVED: <" + fcd.TemporaryEffects[effect].ToString() + ">");
                    fcd.CurrentEffects.Remove(fcd.TemporaryEffects[effect]);
                    fcd.TemporaryEffects.RemoveAt(effect);
                    fcd.EffectCountdowns.RemoveAt(effect);
                    Debug.Log("Current Effects: " + fcd.CurrentEffects.Count + " // Temporary Effects: " + fcd.TemporaryEffects.Count + " // Effect Countdowns: " + fcd.EffectCountdowns.Count);
                }
            }
        }
    }

    /******
     * *****
     * ****** REMOVE_TEMPORARY_ABILITIES
     * *****
     *****/
    public void RemoveTemporaryAbilities(string player)
    {
        List<GameObject> cardZone;
        if (player == PLAYER) cardZone = PlayerZoneCards;
        else cardZone = EnemyZoneCards;

        foreach (GameObject go in cardZone)
        {
            FollowerCardDisplay fcd = GetFollowerCardDisplay(go);
            if (fcd.TemporaryAbilities.Count > 0)
            {
                int countdown = 0;
                List<int> expiredAbilities = new List<int>();

                foreach (CardAbility ca in fcd.TemporaryAbilities)
                {
                    if (--fcd.AbilityCountdowns[countdown] < 1)
                    {
                        Debug.Log("ABILITY EXPIRED: <" + ca.ToString() + ">");
                        expiredAbilities.Add(countdown);
                    }
                    Debug.Log("COUNTDOWN FOR ABILITY <" + ca.ToString() + "> = " + fcd.AbilityCountdowns[countdown]);
                    countdown++;
                }

                expiredAbilities.Reverse(); // REMOVE THE HIGHEST INDEXES FIRST, OTHERWISE RESULTS WILL BE INACCURATE
                foreach (int ability in expiredAbilities)
                {
                    Debug.Log("ABILITY REMOVED: " + fcd.TemporaryAbilities[ability].ToString() + ">");
                    fcd.RemoveCurrentAbility(fcd.TemporaryAbilities[ability]);
                    fcd.AbilityCountdowns.RemoveAt(ability);
                    Debug.Log("Current Abilities: " + fcd.CurrentAbilities.Count + " // Temporary Abilities: " + fcd.TemporaryAbilities.Count + " // Abillity Countdowns: " + fcd.AbilityCountdowns.Count);
                }
            }
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
