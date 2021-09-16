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

    private GameManager gMan;
    private CardManager caMan;
    private AudioManager auMan;
    private EffectManager efMan;
    private UIManager uMan;
    private AnimationManager anMan;
    private PlayerManager pMan;
    private EnemyManager enMan;
    private const string PLAYER = GameManager.PLAYER;
    private const string ENEMY = GameManager.ENEMY;


    public const int CARD_Z_POSITION = -2;
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
    //public const string ENEMY_ACTION_ZONE = "EnemyActionZone";
    public const string ENEMY_DISCARD = "EnemyDiscard";

    public GameObject DragArrowPrefab { get => dragArrowPrefab; }
    public bool IsInCombat { get; set; }

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
        uMan.SelectTarget(PlayerHero, false);
        uMan.SelectTarget(EnemyHero, false);
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
            deckList = pMan.PlayerDeckList;
            currentDeck = pMan.CurrentPlayerDeck;
        }
        else if (hero == GameManager.ENEMY)
        {
            deckList = enMan.EnemyDeckList;
            currentDeck = enMan.CurrentEnemyDeck;
        }
        else
        {
            Debug.LogError("HERO NOT FOUND!");
            return;
        }

        currentDeck.Clear();
        foreach (Card card in deckList) currentDeck.Add(card);
        currentDeck.Shuffle();
    }

    /******
     * *****
     * ****** SHOW/HIDE_CARD
     * *****
     *****/
    public GameObject ShowCard(Card card, bool isShowcase = false)
    {
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
        prefab = Instantiate(prefab, new Vector3(0, 0, CARD_Z_POSITION), Quaternion.identity);
        if (isShowcase) prefab.GetComponent<CardDisplay>().DisplayZoomCard(null, card);
        else prefab.GetComponent<CardDisplay>().CardScript = card;
        return prefab;
    }
    private Card HideCard(GameObject card)
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
            deck = pMan.CurrentPlayerDeck;
            cardTag = PLAYER_CARD;
            cardZone = PLAYER_HAND;
        }
        else if (hero == ENEMY)
        {
            deck = enMan.CurrentEnemyDeck;
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

            foreach (Card c in discard) deck.Add(c);
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
            efMan.NewDrawnCards.Add(card);

            if (uMan.PlayerIsTargetting &&
                efMan.CurrentEffect is DrawEffect)
            {
                if (!efMan.CurrentLegalTargets.Contains(card))
                    efMan.CurrentLegalTargets.Add(card);
                card.GetComponent<CardSelect>().CardOutline.SetActive(true);
            }
        }
        else EnemyHandCards.Add(card);
        auMan.StartStopSound("SFX_DrawCard");
    }

    /******
     * *****
     * ****** SHUFFLE_DECK
     * *****
     *****/
    public void ShuffleDeck(List<Card> deck)
    {
        deck.Shuffle();
        auMan.StartStopSound("SFX_ShuffleDeck");
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
     * ****** IS_PLAYABLE
     * *****
     *****/
    public bool IsPlayable(GameObject card)
    {
        int actionCost = card.GetComponent<CardDisplay>().CurrentActionCost;
        int playerActions = pMan.PlayerActionsLeft;
        if (card.GetComponent<CardDisplay>() is ActionCardDisplay acd)
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
     * ****** SET_CARD_PARENT
     * *****
     *****/
    public void SetCardParent(GameObject card, Transform parent, bool isInHand = false)
    {
        card.transform.SetParent(parent, false);
        float xPos = card.transform.position.x;
        float yPos = card.transform.position.y;
        int zPos;
        if (isInHand) zPos = -3;
        else zPos = CARD_Z_POSITION;
        card.transform.position = 
            new Vector3(xPos, yPos, zPos);        
    }

    /******
     * *****
     * ****** CHANGE_CARD_ZONE
     * *****
     *****/
    public void ChangeCardZone(GameObject card, string zone)
    {
        Transform zoneTran = null;
        bool isInHand = false;
        switch (zone)
        {
            // PLAYER
            case PLAYER_HAND:
                zoneTran = PlayerHand.transform;
                anMan.RevealedHandState(card);
                isInHand = true;
                break;
            case PLAYER_ZONE:
                zoneTran = PlayerZone.transform;
                anMan.PlayedState(card);
                break;
            case PLAYER_ACTION_ZONE:
                zoneTran = PlayerActionZone.transform;
                anMan.PlayedState(card);
                break;
            // ENEMY
            case ENEMY_HAND:
                zoneTran = EnemyHand.transform;
                isInHand = true;
                break;
            case ENEMY_ZONE:
                zoneTran = EnemyZone.transform;
                anMan.PlayedState(card);
                break;
            /*
            case ENEMY_ACTION_ZONE:
                zoneTran = EnemyActionZone.transform;
                AnimationManager.Instance.PlayedState(card);
            */
        }
        SetCardParent(card, zoneTran, isInHand);
        card.GetComponent<CardSelect>().CardOutline.SetActive(false);
        if (card.GetComponent<CardDisplay>() is UnitCardDisplay ucd)
        {
            bool played = false;
            if (zone == PLAYER_ZONE || zone == ENEMY_ZONE) played = true;
            ucd.ResetUnitCard(played);
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
            caMan.TriggerCardAbility(card, "Play");
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
            auMan.StartStopSound(null, playSound), 0.2f);
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
        // PLAYER
        if (card.CompareTag(PLAYER_CARD))
        {
            pMan.PlayerActionsLeft -= card.GetComponent<CardDisplay>().CurrentActionCost;
            PlayerHandCards.Remove(card);

            if (card.GetComponent<CardDisplay>() is UnitCardDisplay)
            {
                PlayerZoneCards.Add(card);
                ChangeCardZone(card, PLAYER_ZONE);
                PlayUnit();
                FunctionTimer.Create(() =>
                efMan.TriggerGiveNextEffect(card), 0.4f);
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
        efMan.StartEffectGroupList(groupList, card);
    }

    /******
     * *****
     * ****** DESTROY_CARD [PLAY >>> DISCARD]
     * *****
     *****/
    public void DestroyUnit(GameObject card, bool isDelayed = true)
    {
        if (efMan.CurrentEffect != null) // TESTING
        {
            efMan.UnitsToDestroy.Add(card);
            return;
        }
        if (efMan.UnitsToDestroy.Contains(card)) // TESTING
            efMan.UnitsToDestroy.Remove(card);

        string cardTag = card.tag;
        if (isDelayed)
        {
            FunctionTimer.Create(() => Triggers(), 0.5f);
            FunctionTimer.Create(() => Destroy(), 1);
        }
        else
        {
            Triggers();
            Destroy();
        }

        void Triggers()
        {
            caMan.TriggerCardAbility(card, "Revenge");
            if (CardManager.GetAbility(card, "Marked"))
                DrawCard(PLAYER);
        }
        void Destroy()
        {
            Sound deathSound = GetUnitDisplay(card).UnitCard.UnitDeathSound;
            AudioManager.Instance.StartStopSound(null, deathSound);

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

            if (efMan.UnitsToDestroy.Count > 0) // TESTING
                DestroyUnit(efMan.UnitsToDestroy[0], isDelayed);
        }
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
    public bool CanAttack(GameObject attacker, GameObject defender, bool preCheck = false)
    {
        if (defender != null)
        {
            if (attacker.CompareTag(defender.tag)) return false;
            if (attacker.CompareTag(PLAYER_CARD)) 
                if (defender == PlayerHero) return false;
        }
        if (GetUnitDisplay(attacker).IsExhausted)
        {
            if (!preCheck)
                uMan.CreateFleetinInfoPopup("Exhausted units can't attack!");
            return false;
        }
        if (defender == null) return true; // For StartDrag in DragDrop
        else if (defender.TryGetComponent(out UnitCardDisplay ucd))
        {
            if (ucd.CurrentHealth < 1) return false; // Destroyed units that haven't left play yet
            if (CardManager.GetAbility(defender, "Stealth"))
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
        string STEALTH = "Stealth";
        string RANGED = "Ranged";

        Strike(attacker, defender);
        GetUnitDisplay(attacker).IsExhausted = true;
        if (CardManager.GetAbility(attacker, STEALTH))
            FunctionTimer.Create(() => 
            attacker.GetComponent<UnitCardDisplay>().RemoveCurrentAbility(STEALTH), 0.5f);
        bool defenderIsUnit;
        if (defender.CompareTag(ENEMY_CARD) || defender.CompareTag(PLAYER_CARD))
        {
            defenderIsUnit = true;
            if (!CardManager.GetAbility(attacker, RANGED))
                Strike(defender, attacker);
        }
        else defenderIsUnit = false;
        if (!CardManager.GetAbility(attacker, RANGED))
            auMan.StartStopSound("SFX_AttackMelee");
        else auMan.StartStopSound("SFX_AttackRanged");
        anMan.UnitAttack(attacker, defender, defenderIsUnit);
    }

    /******
     * *****
     * ****** STRIKE
     * *****
     *****/
    public void Strike(GameObject striker, GameObject defender)
    {
        int power = GetUnitDisplay(striker).CurrentPower;
        caMan.TriggerCardAbility(striker, "Strike");
        if (TakeDamage(defender, power))
            caMan.TriggerCardAbility(striker, "Deathblow");
    }

    /******
     * *****
     * ****** TAKE_DAMAGE
     * *****
     *****/
    public bool TakeDamage(GameObject target, int damageValue)
    {
        if (damageValue < 1) return false;
        string SHIELD = "Shield";

        int targetValue;
        int newTargetValue;
        if (target == PlayerHero) targetValue = pMan.PlayerHealth;
        else if (target == EnemyHero) targetValue = enMan.EnemyHealth;
        else targetValue = GetUnitDisplay(target).CurrentHealth;
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
        else // Damage to Units
        {
            if (GetUnitDisplay(target).CurrentHealth < 1) return false;
            if (CardManager.GetAbility(target, SHIELD))
            {
                GetUnitDisplay(target).RemoveCurrentAbility(SHIELD);
                return false;
            }
            else
            {
                GetUnitDisplay(target).CurrentHealth = newTargetValue;
                anMan.ModifyUnitHealthState(target);
            }
        }
        if (newTargetValue < 1)
        {
            if (target.TryGetComponent<UnitCardDisplay>(out _)) DestroyUnit(target);
            else if (target == PlayerHero) gMan.EndCombat(false);
            else if (target == EnemyHero) gMan.EndCombat(true);
            if (target == PlayerHero || target == EnemyHero) return false;
            else  return true;
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
        newTargetValue = targetValue + healingValue;
        if (newTargetValue > maxValue) newTargetValue = maxValue;
        if (target == PlayerHero) pMan.PlayerHealth = newTargetValue;
        else if (target == EnemyHero) enMan.EnemyHealth = newTargetValue;
        else GetUnitDisplay(target).CurrentHealth = newTargetValue;
        anMan.ModifyUnitHealthState(target);
    }
}
