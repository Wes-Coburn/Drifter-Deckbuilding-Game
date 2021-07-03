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
    public const string PLAYER_CHAMPION = "PlayerChampion";

    public const string ENEMY_HAND = "EnemyHand";
    public const string ENEMY_ZONE = "EnemyZone";
    public const string ENEMY_DISCARD = "EnemyDiscard";
    public const string ENEMY_CHAMPION = "EnemyChampion";

    /* GAME_MANAGER_DATA */
    private const string PLAYER = GameManager.PLAYER;
    private const string ENEMY = GameManager.ENEMY;
    private const int STARTING_HAND_SIZE = GameManager.STARTING_HAND_SIZE;
    
    /* CARD LISTS */
    private List<int> playerDeck = new List<int>();
    private List<int> enemyDeck = new List<int>();
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
    public GameObject PlayerHand { get; private set; }
    public GameObject PlayerZone { get; private set; }
    public GameObject PlayerDiscard { get; private set; }
    public GameObject PlayerChampion { get; private set; }
    // ENEMY
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
        PlayerHand = GameObject.Find(PLAYER_HAND);
        PlayerZone = GameObject.Find(PLAYER_ZONE);
        PlayerDiscard = GameObject.Find(PLAYER_DISCARD);
        PlayerChampion = GameObject.Find(PLAYER_CHAMPION);
        EnemyHand = GameObject.Find(ENEMY_HAND);
        EnemyZone = GameObject.Find(ENEMY_ZONE);
        EnemyDiscard = GameObject.Find(ENEMY_DISCARD);
        EnemyChampion = GameObject.Find(ENEMY_CHAMPION);
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
    private void SetExhausted(GameObject heroCard, bool exhausted)
    {
        heroCard.GetComponent<HeroCardDisplay>().CanAttack = !exhausted;
    }
    public void RefreshCards(string player)
    {
        List<GameObject> cardZoneList = null;
        if (player == PLAYER) cardZoneList = PlayerZoneCards;
        else if (player == ENEMY) cardZoneList = EnemyZoneCards;
        foreach (GameObject card in cardZoneList) SetExhausted(card, false);
    }

    /******
     * *****
     * ****** RESET_HERO_CARD
     * *****
     *****/
    private void ResetHeroCard(GameObject card)
    {
        HeroCardDisplay hcd = GetHeroCardDisplay(card);
        HeroCard hc = (HeroCard)hcd.CardScript;

        hcd.SetAttackScore(hc.AttackScore);
        hcd.SetDefenseScore(hc.DefenseScore);
        hcd.CanAttack = false;
        card.GetComponent<DragDrop>().IsPlayed = false;
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
        Transform zoneTran = null;
        switch (zone)
        {
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

        if (card.TryGetComponent<HeroCardDisplay>(out HeroCardDisplay hcd))
        {
            if (zone != PLAYER_ZONE && zone != ENEMY_ZONE) ResetHeroCard(card);
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
            PlayerManager.Instance.PlayerActionsLeft -= card.GetComponent<CardDisplay>().GetActionCost();
            ChangeCardZone(card, PLAYER_ZONE);
            PlayerHandCards.Remove(card);
            PlayerZoneCards.Add(card);
            TriggerCardAbility(card, "Play"); // TESTING
        }
        else if (player == ENEMY)
        {
            card = EnemyHandCards[0];
            EnemyHandCards.Remove(card);
            EnemyZoneCards.Add(card);
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
            PlayerZoneCards.Remove(card);
            Debug.Log("playerZoneCards.Count == " + PlayerZoneCards.Count);
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
        int AttackingHeroAttackScore = GetHeroCardDisplay(attacker).GetAttackScore();
        TakeDamage(defender, AttackingHeroAttackScore);
        SetExhausted(attacker, true);
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

        if (target == PlayerChampion)
        {
            targetValue = PlayerManager.Instance.PlayerHealth;
        }
        else if (target == EnemyChampion)
        {
            targetValue = PlayerManager.Instance.PlayerHealth;
        }
        else
        {
            targetValue = GetHeroCardDisplay(target).CurrentDefenseScore;
        }

        newTargetValue = targetValue - damageValue;
        //if (newTargetValue < 0) newTargetValue = 0;

        if (target == PlayerChampion)
        {
            PlayerManager.Instance.PlayerHealth = newTargetValue;
        }
        else if (target == EnemyChampion)
        {
            EnemyManager.Instance.EnemyHealth = newTargetValue;
        }
        else
        {
            GetHeroCardDisplay(target).CurrentDefenseScore = newTargetValue;
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

        int defenseScore = GetHeroCardDisplay(heroCard).CurrentDefenseScore;
        int newDefenseScore = defenseScore + healingValue;
        if (newDefenseScore > GetHeroCardDisplay(heroCard).MaxDefenseScore)
        {
            newDefenseScore = GetHeroCardDisplay(heroCard).MaxDefenseScore;
        }
        GetHeroCardDisplay(heroCard).CurrentDefenseScore = newDefenseScore;
        AnimationManager.Instance.ModifyDefenseState(heroCard);
    }

    /******
     * *****
     * ****** CHANGE_STATS
     * *****
     *****/
    public void ChangeStats(GameObject heroCard, int changeValue, bool isDefenseChange, bool isTemporary)
    {
        if (isDefenseChange)
        {
            GetHeroCardDisplay(heroCard).CurrentDefenseScore += changeValue;
            GetHeroCardDisplay(heroCard).MaxDefenseScore += changeValue;
        }
        else
        {
            GetHeroCardDisplay(heroCard).CurrentAttackScore += changeValue;
            if (isTemporary)
            {
                heroCard.GetComponent<HeroCardDisplay>().TemporaryAttackModifier = changeValue;
            }
        }            
    }

    /******
     * *****
     * ****** REMOVE_TEMPORARY_STATS
     * *****
     *****/
    public void RemoveTemporaryStats(string player)
    {
        List<GameObject> cardZone;
        if (player == PLAYER) cardZone = PlayerZoneCards;
        else cardZone = EnemyZoneCards;

        foreach (GameObject go in cardZone)
        {
            HeroCardDisplay hcd = GetHeroCardDisplay(go);
            if (hcd.TemporaryAttackModifier != 0)
            {
                hcd.CurrentAttackScore -= hcd.TemporaryAttackModifier;
                hcd.TemporaryAttackModifier = 0;
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
            HeroCardDisplay hcd = GetHeroCardDisplay(go);
            if (hcd.TemporaryAbilities.Count > 0)
            {
                foreach (CardAbility ca in hcd.TemporaryAbilities)
                {
                    hcd.RemoveCurrentAbility(ca);
                }
                hcd.TemporaryAbilities.Clear();
            }
        }
    }

    /******
     * *****
     * ****** TRIGGER_CARD_ABILITY
     * *****
     *****/
    public void TriggerCardAbility(GameObject card, string triggerName) // TESTING
    {
        Debug.Log("TriggerCardAbility()");
        foreach (CardAbility cardAbility in card.GetComponent<HeroCardDisplay>().CurrentAbilities)
        {
            if (cardAbility is TriggeredAbility)
            {
                TriggeredAbility tra = cardAbility as TriggeredAbility;
                if (tra.AbilityTrigger.AbilityName == triggerName)
                {
                    EffectManager.Instance.StartNewEffectGroup(tra.EffectGroup); // FOR TESTING ONLY
                }
            }
        }
    }
}
