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

        while (playerDeck.Count < 30) playerDeck.Add(1); // FOR TESTING ONLY
        while (enemyDeck.Count < 30) enemyDeck.Add(2); // FOR TESTING ONLY
    }

    /* CARD_MANAGER_DATA */
    public const string BACKGROUND = "Background";

    public const string PLAYER_CARD = "PlayerCard";
    public const string ENEMY_CARD = "EnemyCard";

    public const string HAND_ZONE = "HandZone";
    public const string PLAY_ZONE = "PlayZone";
    public const string DISCARD_ZONE = "DiscardZone";

    public const string PLAYER_HAND = "PlayerHand";
    public const string PLAYER_ZONE = "PlayerZone";
    public const string PLAYER_DISCARD = "PlayerDiscard";

    public const string ENEMY_HAND = "EnemyHand";
    public const string ENEMY_ZONE = "EnemyZone";
    public const string ENEMY_DISCARD = "EnemyDiscard";

    /* GAME_MANAGER_DATA */
    private const string PLAYER = "Player";
    private const string ENEMY = "Enemy";
    private const int STARTING_HAND_SIZE = 3;
    
    /* CARD LISTS */
    private List<int> playerDeck = new List<int>();
    private List<int> enemyDeck = new List<int>();
    public List<GameObject> playerZoneCards;
    public List<GameObject> enemyZoneCards;
    public List<GameObject> enemyHandCards;

    /* GAME ZONES */
    public GameObject PlayerHand { get; private set; }
    public GameObject EnemyHand { get; private set; }
    public GameObject PlayerZone { get; private set; }
    public GameObject EnemyZone { get; private set; }
    public GameObject PlayerDiscard { get; private set; }
    public GameObject EnemyDiscard { get; private set; }
    
    private void Start()
    {
        playerZoneCards = new List<GameObject>();
        enemyZoneCards = new List<GameObject>();
        enemyHandCards = new List<GameObject>();
    }
    public void StartGame()
    {
        /* GAME_ZONES */
        PlayerHand = GameObject.Find(PLAYER_HAND);
        PlayerZone = GameObject.Find(PLAYER_ZONE);
        PlayerDiscard = GameObject.Find(PLAYER_DISCARD);
        EnemyHand = GameObject.Find(ENEMY_HAND);
        EnemyZone = GameObject.Find(ENEMY_ZONE);
        EnemyDiscard = GameObject.Find(ENEMY_DISCARD);
    }

    /******
     * *****
     * ****** GET_CARD_DISPLAYS
     * *****
     *****/
    private CardDisplay GetCardDisplay(GameObject card) => card.GetComponent<CardDisplay>();
    public HeroCardDisplay GetHeroCardDisplay(GameObject heroCard) => (HeroCardDisplay)GetCardDisplay(heroCard);
    //public ActionCardDisplay GetActionCardDisplay(GameObject actionCard) => (ActionCardDisplay)GetCardDisplay(actionCard);

    /******
     * *****
     * ****** SET_EXHAUSTED/REFRESH_CARDS
     * *****
     *****/
    public void SetExhausted(GameObject heroCard, bool exhausted)
    {
        heroCard.GetComponent<HeroCardDisplay>().CanAttack = !exhausted;
    }
    public void RefreshCards(string player)
    {
        List<GameObject> cardZoneList = null;
        if (player == PLAYER) cardZoneList = playerZoneCards;
        else if (player == ENEMY) cardZoneList = enemyZoneCards;
        foreach (GameObject card in cardZoneList) SetExhausted(card, false);
    }

    /******
     * *****
     * ****** IS_PLAYABLE/CAN_ATTACK
     * *****
     *****/
    public bool IsPlayable(GameObject card)
    {
        int actionCost = card.GetComponent<CardDisplay>().GetActionCost();
        int playerActionsLeft = PlayerManager.Instance.PlayerActionsLeft;

        if (playerActionsLeft >= actionCost) return true;
        else
        {
            Debug.LogWarning("[IsPlayable() in CardManager] COULD NOT PLAY! */*/* ActionCost: "
            + actionCost + " */*/* PlayerActionsLeft: " + PlayerManager.Instance.PlayerActionsLeft);
            return false;
        }
    }
    public bool CanAttack(GameObject heroCard)
    {
        if (!heroCard.GetComponent<HeroCardDisplay>().CanAttack)
        {
            Debug.LogWarning("[CanAttack() in CardManager] CanAttack = FALSE!");
            return false;
        }
        else return true;
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
        card.GetComponent<ChangeLayer>().CardsLayer(); // Unnecessary?
    }

    /******
     * *****
     * ****** CHANGE_CARD_ZONE
     * *****
     *****/
    public void ChangeCardZone(GameObject card, string zone)
    {
        if (card.TryGetComponent<HeroCardDisplay>(out HeroCardDisplay heroCardDisplay))
        {
            heroCardDisplay.CanAttack = false;
            // RESET ATTACK AND DEFENSE
        }

        Transform zoneTran = null;
        switch (zone)
        {
            case PLAYER_HAND:
                zoneTran = PlayerHand.transform;
                AnimationManager.Instance.RevealedState(card);
                break;
            case PLAYER_ZONE:
                zoneTran = PlayerZone.transform;
                AnimationManager.Instance.PlayedState(card);
                break;
            case PLAYER_DISCARD:
                zoneTran = PlayerDiscard.transform;
                break;
            case ENEMY_HAND:
                zoneTran = EnemyHand.transform;
                break;
            case ENEMY_ZONE:
                zoneTran = EnemyZone.transform;
                AnimationManager.Instance.PlayedState(card);
                break;
            case ENEMY_DISCARD:
                zoneTran = EnemyDiscard.transform;
                break;
        }
        SetCardParent(card, zoneTran);
    }

    /******
     * *****
     * ****** DRAW_CARD
     * *****
     *****/
    public void DrawCard(string player)
    {
        List<int> deck = null;
        string cardTag = null;
        string cardZone = null;

        if (player == PLAYER)
        {
            deck = playerDeck;
            cardTag = PLAYER_CARD;
            cardZone = PLAYER_HAND;
        }
        else if (player == ENEMY)
        {
            deck = enemyDeck;
            cardTag = ENEMY_CARD;
            cardZone = ENEMY_HAND;
        }

        int cardID = deck[0];
        deck.RemoveAt(0);
        
        if (deck.Count < 1)
        {
            Debug.LogWarning("[CardManager.DrawCard()] NO CARDS LEFT!");
            return;
        }

        GameObject newCard = CardLibrary.Instance.GetCard(cardID);
        newCard.tag = cardTag;
        ChangeCardZone(newCard, cardZone);

        if (player == ENEMY) enemyHandCards.Add(newCard);
    }
    public void DrawHand(string player)
    {
        for (int i = 0; i < STARTING_HAND_SIZE; i++) DrawCard(player);
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
            PlayerManager.Instance.PlayerActionsLeft -= card.GetComponent<CardDisplay>().GetActionCost();
            ChangeCardZone(card, PLAYER_ZONE);
            playerZoneCards.Add(card);
            TriggerCardAbility(card, "Play"); // TESTING
        }
        else if (player == ENEMY)
        {
            card = enemyHandCards[0];
            enemyHandCards.Remove(card);
            enemyZoneCards.Add(card);
            EnemyManager.Instance.EnemyActionsLeft -= card.GetComponent<CardDisplay>().GetActionCost();
            ChangeCardZone(card, ENEMY_ZONE);
        }
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
            playerZoneCards.Remove(card);
        }
        else if (player == ENEMY)
        {
            ChangeCardZone(card, ENEMY_DISCARD);
            enemyZoneCards.Remove(card);
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
        }
        else if (player == ENEMY)
        {
            ChangeCardZone(card, ENEMY_DISCARD);
            enemyHandCards.Remove(card);
        }
    }

    /******
     * *****
     * ****** ATTACK
     * *****
     *****/
    public void Attack(GameObject AttackingHeroCard, GameObject DefendingHeroCard)
    {
        int AttackingHeroAttackScore = GetHeroCardDisplay(AttackingHeroCard).GetAttackScore();
        TakeDamage(DefendingHeroCard, AttackingHeroAttackScore);
        SetExhausted(AttackingHeroCard, true);
    }

    /******
     * *****
     * ****** TAKE_DAMAGE
     * *****
     *****/
    public void TakeDamage(GameObject heroCard, int damageValue)
    {
        if (damageValue < 1) return;

        int defenseScore = GetHeroCardDisplay(heroCard).CurrentDefenseScore;
        int newDefenseScore = defenseScore - damageValue;
        if (newDefenseScore < 0) newDefenseScore = 0;
        GetHeroCardDisplay(heroCard).CurrentDefenseScore = newDefenseScore;
        GetHeroCardDisplay(heroCard).SetDefenseScoreModifier(-damageValue);
        AnimationManager.Instance.ModifyDefenseState(heroCard);

        if (newDefenseScore < 1)
        {
            if (heroCard.CompareTag("PlayerCard")) DestroyCard(heroCard, PLAYER);
            else DestroyCard(heroCard, ENEMY);
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

        int defenseScore = GetHeroCardDisplay(heroCard).CurrentDefenseScore;
        int newDefenseScore = defenseScore + healingValue;
        if (newDefenseScore > GetHeroCardDisplay(heroCard).MaxDefenseScore)
        {
            newDefenseScore = GetHeroCardDisplay(heroCard).MaxDefenseScore;
        }
        GetHeroCardDisplay(heroCard).CurrentDefenseScore = newDefenseScore;
        GetHeroCardDisplay(heroCard).SetDefenseScoreModifier(healingValue);
        AnimationManager.Instance.ModifyDefenseState(heroCard);
    }

    /******
     * *****
     * ****** TRIGGER_CARD_ABILITY
     * *****
     *****/
    public void TriggerCardAbility(GameObject card, string triggerName) // TESTING
    {
        Debug.Log(">>>TriggerCardAbility()<<<");
        foreach (CardAbility cardAbility in card.GetComponent<HeroCardDisplay>().CurrentAbilities)
        {
            if (cardAbility is TriggeredAbility)
            {
                Debug.LogWarning("TriggeredAbility found: *" + cardAbility.AbilityName + "*");

                TriggeredAbility kwa = cardAbility as TriggeredAbility;
                if (kwa.AbilityTrigger.AbilityName == triggerName)
                {
                    Debug.Log("AbilityTrigger found: *" + triggerName + "*");
                    EffectManager.Instance.StartNewEffectGroup(kwa.EffectGroup);
                }
            }
        }
    }
}
