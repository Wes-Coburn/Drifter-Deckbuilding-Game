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

        while (DeckCount() < 30) playerDeck.Add(1); // PLAYER DECK

        gameManager = GameManager.Instance;
        animationManager = AnimationManager.Instance;
        playerHand = GameObject.Find(PLAYER_HAND);
        playerZone = GameObject.Find(PLAYER_ZONE);
        playerDiscard = GameObject.Find(PLAYER_DISCARD);
        enemyHand = GameObject.Find(ENEMY_HAND);
        enemyZone = GameObject.Find(ENEMY_ZONE);
        enemyDiscard = GameObject.Find(ENEMY_DISCARD);
    }

    /* CARD_MANAGER_DATA */
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
    List<GameObject> playerZoneCards = new List<GameObject>();
    List<GameObject> enemyZoneCards = new List<GameObject>();

    /* GAME ZONES */
    private GameObject playerHand;
    private GameObject enemyHand;
    private GameObject playerZone;
    private GameObject enemyZone;
    private GameObject playerDiscard;
    private GameObject enemyDiscard;

    /* MANAGERS */
    private GameManager gameManager;
    private AnimationManager animationManager;

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
        Debug.Log("SET CARD PARENT");
        card.transform.SetParent(parentTransform, false);
        float xPos = card.transform.position.x;
        float yPos = card.transform.position.y;
        card.transform.position = new Vector3(xPos, yPos, -2);
        card.GetComponent<ChangeLayer>().CardsLayer(); // Unnecessary?
    }

    public void ChangeCardZone(GameObject card, string zone)
    {
        Debug.Log("CHANGE CARD ZONE");
        if (card.TryGetComponent<HeroCardDisplay>(out HeroCardDisplay heroCardDisplay))
        {
            heroCardDisplay.CanAttack = false;
            // RESET ATTACK AND DEFENSE
        }

        Transform zoneTran = null;
        switch (zone)
        {
            case PLAYER_HAND:
                zoneTran = playerHand.transform;
                animationManager.RevealedState(card);
                break;
            case PLAYER_ZONE:
                zoneTran = playerZone.transform;
                playerZoneCards.Add(card);
                animationManager.PlayedState(card);
                break;
            case PLAYER_DISCARD:
                zoneTran = playerDiscard.transform;
                playerZoneCards.Remove(card);
                break;
            case ENEMY_HAND:
                zoneTran = enemyHand.transform;
                break;
            case ENEMY_ZONE:
                zoneTran = enemyZone.transform;
                enemyZoneCards.Add(card);
                animationManager.PlayedState(card);
                break;
            case ENEMY_DISCARD:
                zoneTran = enemyDiscard.transform;
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
    public void DrawCard()
    {
        int cardID = playerDeck[0];
        playerDeck.RemoveAt(0);

        Debug.Log("DRAW CARD");

        if (DeckCount() < 1)
        {
            Debug.Log("[CardManager.DrawCard()] NO CARDS LEFT!!!");
            return;
        }

        GameObject newCard = CardLibrary.Instance.GetCard(cardID);
        newCard.tag = "PlayerCard";
        newCard.GetComponent<DragDrop>().IsDraggable = true;
        ChangeCardZone(newCard, PLAYER_HAND);
    }
    public void DrawHand()
    {
        //gameManager.UpdateActionsLeft(); // REMOVE THIS EVENTUALLY
        //for (int i = 0; i < STARTING_HAND_SIZE; i++) DrawCard();
        DrawCard();
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
                gameManager.PlayerActionsLeft -= card.GetComponent<CardDisplay>().GetActionCost();
                ChangeCardZone(card, PLAYER_ZONE);
                break;
            case ENEMY:
                gameManager.EnemyActionsLeft -= card.GetComponent<CardDisplay>().GetActionCost();
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

        int defenseScore = GetHeroCardDisplay(heroCard).GetDefenseScore();
        int newDefenseScore = defenseScore - damage;
        if (newDefenseScore < 0) newDefenseScore = 0;
        GetHeroCardDisplay(heroCard).SetDefenseScore(newDefenseScore);
        GetHeroCardDisplay(heroCard).SetDefenseScoreModifier(-damage);
        animationManager.ModifyDefenseState(heroCard);

        if (newDefenseScore < 1)
        {
            if (heroCard.CompareTag("PlayerCard")) DiscardCard(heroCard, PLAYER_DISCARD);
            else DiscardCard(heroCard, ENEMY_DISCARD);
        }
    }

    public bool IsPlayable(GameObject card)
    {
        int actionCost = card.GetComponent<CardDisplay>().GetActionCost();
        int playerActionsLeft = gameManager.PlayerActionsLeft;

        if (playerActionsLeft >= actionCost) return true;
        else
        {
            Debug.Log("[IsPlayable() in CardManager] COULD NOT PLAY! // ActionCost: " + actionCost + " // PlayerActionsLeft: " + gameManager.PlayerActionsLeft);
            return false;
        }
    }
    public bool CanAttack(GameObject heroCard)
    {
        if (gameManager.PlayerActionsLeft < 1 || !heroCard.GetComponent<HeroCardDisplay>().CanAttack)
        {
            Debug.Log("[CanAttack() in CardManager] CanAttack = FALSE!");
            return false;
        }
        else return true;
    }
}
