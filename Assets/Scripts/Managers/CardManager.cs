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

    [SerializeField] private GameObject unitCardPrefab;
    [SerializeField] private GameObject actionCardPrefab;
    [SerializeField] private GameObject newCardPopupPrefab;
    [SerializeField] private UnitCard[] playerStartUnits;
    [SerializeField] private Sprite cardBackSprite;

    public GameObject UnitCardPrefab { get => unitCardPrefab; }
    public GameObject ActionCardPrefab { get => actionCardPrefab; }
    public GameObject NewCardPopup { get; private set; }
    public UnitCard[] PlayerStartUnits { get => playerStartUnits; }

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
     * ****** CARD_ABILITIES
     * *****
     *****/
    public static bool GetAbility(GameObject card, string ability)
    {
        if (card == null)
        {
            Debug.LogError("CARD IS NULL!");
            return false;
        }
        UnitCardDisplay ucd = card.GetComponent<UnitCardDisplay>();
        int abilityIndex = ucd.CurrentAbilities.FindIndex(x => x.AbilityName == ability);
        if (abilityIndex == -1) return false;
        else return true;
    }
    public static int GetAbilityIndex(GameObject card, string ability)
    {
        UnitCardDisplay ucd = card.GetComponent<UnitCardDisplay>();
        int abilityIndex = ucd.CurrentAbilities.FindIndex(x => x.AbilityName == ability);
        return abilityIndex;
    }
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
