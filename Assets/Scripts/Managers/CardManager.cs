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

        while (playerDeck.Count < 30) playerDeck.Add(1); // Player Deck
        while (enemyDeck.Count < 30) enemyDeck.Add(2); // Enemy Deck
    }

    /* CARD_MANAGER_DATA */
    private const string PLAYER_CARD = CardManagerData.PLAYER_CARD;
    private const string ENEMY_CARD = CardManagerData.ENEMY_CARD;
    private const string PLAYER_HAND = CardManagerData.PLAYER_HAND;
    private const string PLAYER_ZONE = CardManagerData.PLAYER_ZONE;
    private const string PLAYER_DISCARD = CardManagerData.PLAYER_DISCARD;
    private const string ENEMY_HAND = CardManagerData.ENEMY_HAND;
    private const string ENEMY_ZONE = CardManagerData.ENEMY_ZONE;
    private const string ENEMY_DISCARD = CardManagerData.ENEMY_DISCARD;

    /* GAME_MANAGER_DATA */
    private const string PLAYER = GameManagerData.PLAYER;
    private const string ENEMY = GameManagerData.ENEMY;
    private const int STARTING_HAND_SIZE = GameManagerData.STARTING_HAND_SIZE;
    
    /* CARD LISTS */
    private List<int> playerDeck = new List<int>();
    private List<int> enemyDeck = new List<int>();
    List<GameObject> playerZoneCards = new List<GameObject>();
    List<GameObject> enemyZoneCards = new List<GameObject>();
    List<GameObject> enemyHandCards = new List<GameObject>();

    /* GAME ZONES */
    public GameObject PlayerHand { get; private set; }
    public GameObject EnemyHand { get; private set; }
    public GameObject PlayerZone { get; private set; }
    public GameObject EnemyZone { get; private set; }
    public GameObject PlayerDiscard { get; private set; }
    public GameObject EnemyDiscard { get; private set; }

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

    private CardDisplay GetCardDisplay(GameObject card) => card.GetComponent<CardDisplay>();
    public HeroCardDisplay GetHeroCardDisplay(GameObject heroCard) => (HeroCardDisplay)GetCardDisplay(heroCard);
    //public ActionCardDisplay GetActionCardDisplay(GameObject actionCard) => (ActionCardDisplay)GetCardDisplay(actionCard);

    public int DeckCount() => playerDeck.Count;

    public void SetExhausted(GameObject heroCard, bool exhausted)
    {
        heroCard.GetComponent<HeroCardDisplay>().CanAttack = !exhausted;
        heroCard.GetComponent<Animator>().SetBool("Exhausted", exhausted);
    }

    public void RefreshCards(string player)
    {
        List<GameObject> cardZoneList = null;
        if (player == PLAYER) cardZoneList = playerZoneCards;
        else if (player == ENEMY) cardZoneList = enemyZoneCards;
        foreach (GameObject card in cardZoneList) SetExhausted(card, false);
    }

    public void SetCardParent(GameObject card, Transform parentTransform)
    {
        card.transform.SetParent(parentTransform, false);
        float xPos = card.transform.position.x;
        float yPos = card.transform.position.y;
        card.transform.position = new Vector3(xPos, yPos, -2);
        card.GetComponent<ChangeLayer>().CardsLayer(); // Unnecessary?
    }

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
                playerZoneCards.Add(card);
                AnimationManager.Instance.PlayedState(card);
                break;
            case PLAYER_DISCARD:
                zoneTran = PlayerDiscard.transform;
                playerZoneCards.Remove(card);
                break;
            case ENEMY_HAND:
                zoneTran = EnemyHand.transform;
                break;
            case ENEMY_ZONE:
                zoneTran = EnemyZone.transform;
                enemyZoneCards.Add(card);
                AnimationManager.Instance.PlayedState(card);
                break;
            case ENEMY_DISCARD:
                zoneTran = EnemyDiscard.transform;
                enemyZoneCards.Remove(card);
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
            Debug.Log("[CardManager.DrawCard()] NO CARDS LEFT!!!");
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
     * ****** PLAY_CARD
     * *****
     *****/
    public void PlayCard(GameObject card, string player)
    {
        switch (player)
        {
            case PLAYER:
                GameManager.Instance.PlayerActionsLeft -= card.GetComponent<CardDisplay>().GetActionCost();
                ChangeCardZone(card, PLAYER_ZONE);
                break;
            case ENEMY:
                card = enemyHandCards[0]; // TESTING
                enemyHandCards.RemoveAt(0); // TESTING
                
                GameManager.Instance.EnemyActionsLeft -= card.GetComponent<CardDisplay>().GetActionCost();
                ChangeCardZone(card, ENEMY_ZONE);
                break;
        }
    }

    public void DiscardCard(GameObject card, string player)
    {
        switch (player)
        {
            case PLAYER:
                ChangeCardZone(card, PLAYER_DISCARD);
                break;
            case ENEMY:
                ChangeCardZone(card, ENEMY_DISCARD);
                break;
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

    public void TakeDamage(GameObject heroCard, int damage)
    {
        if (damage < 1) return;

        int defenseScore = GetHeroCardDisplay(heroCard).CurrentDefenseScore; // TESTING
        int newDefenseScore = defenseScore - damage;
        if (newDefenseScore < 0) newDefenseScore = 0;
        GetHeroCardDisplay(heroCard).CurrentDefenseScore = newDefenseScore; // TESTING
        GetHeroCardDisplay(heroCard).SetDefenseScoreModifier(-damage);
        AnimationManager.Instance.ModifyDefenseState(heroCard);

        if (newDefenseScore < 1)
        {
            if (heroCard.CompareTag("PlayerCard")) DiscardCard(heroCard, PLAYER_DISCARD);
            else DiscardCard(heroCard, ENEMY_DISCARD);
        }
    }

    public bool IsPlayable(GameObject card)
    {
        int actionCost = card.GetComponent<CardDisplay>().GetActionCost();
        int playerActionsLeft = GameManager.Instance.PlayerActionsLeft;

        if (playerActionsLeft >= actionCost) return true;
        else
        {
            Debug.Log("[IsPlayable() in CardManager] COULD NOT PLAY! */*/* ActionCost: " + actionCost + " */*/* PlayerActionsLeft: " + GameManager.Instance.PlayerActionsLeft);
            return false;
        }
    }
    public bool CanAttack(GameObject heroCard)
    {
        if (GameManager.Instance.PlayerActionsLeft < 1 || !heroCard.GetComponent<HeroCardDisplay>().CanAttack)
        {
            Debug.Log("[CanAttack() in CardManager] CanAttack = FALSE!");
            return false;
        }
        else return true;
    }
}
