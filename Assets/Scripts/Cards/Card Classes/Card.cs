using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class Card : ScriptableObject
{
    [SerializeField] private Sprite cardArt, cardBorder;
    [SerializeField, Range(0, GameManager.MAXIMUM_ENERGY)] private int energyCost;
    [SerializeField] private Effect.ConditionType costConditionType;
    [SerializeField, Range(0, 10), Tooltip("The value checked by the condition, if any")] private int costConditionValue;
    [SerializeField, Range(-5, 0), Tooltip("The modifier applied to the cost")] private int costConditionModifier;
    [SerializeField] private string cardName, cardType, cardSubType;
    public enum Rarity
    {
        Common,
        Rare,
        Legend
    }
    [SerializeField] private Rarity cardRarity;
    [TextArea, SerializeField] private string cardDescription;
    [SerializeField] protected Card[] relatedCards;
    [SerializeField] private CreatedCardType[] relatedCardTypes;
    [SerializeField] private Sound cardPlaySound;
    [SerializeField] private AnimatorOverrideController overController, zoomOverController;

    public Sprite CardArt { get => cardArt; }
    public Sprite CardBorder { get => cardBorder; }
    public int StartEnergyCost { get => energyCost; }
    public Effect.ConditionType CostConditionType { get => costConditionType; }
    public int CostConditionValue { get => costConditionValue; }
    public int CostConditionModifier { get => costConditionModifier; }
    public string CardName { get => cardName; }
    public string CardType { get => cardType; }
    public string CardSubType { get => cardSubType; }
    public Rarity CardRarity { get => cardRarity; }
    public string CardDescription { get => cardDescription; }
    public Card[] RelatedCards
    {
        
        get
        {
            List<Card> allRelatedCards = new();
            
            if (relatedCards != null) allRelatedCards.AddRange(relatedCards);

            if (relatedCardTypes != null)
            {
                foreach (var cardType in relatedCardTypes)
                {
                    var cardTypeCards = Managers.CA_MAN.GetCreatedCards(cardType, true);
                    if (cardTypeCards != null) allRelatedCards.AddRange(cardTypeCards);
                }
            }

            return allRelatedCards.ToArray();
        }
    }
    public enum CreatedCardType
    {
        Exploit,
        Invention,
        Scheme,
        Extraction,
        Trap
    }
    public CreatedCardType[] RelatedCardTypes { get => relatedCardTypes; }
    public Sound CardPlaySound { get => cardPlaySound; }

    public AnimatorOverrideController OverController { get => overController; }
    public AnimatorOverrideController ZoomOverController { get => zoomOverController; }

    public int CurrentEnergyCost { get; set; }
    public List<Effect> CurrentEffects { get; private set; }
    public List<Effect> PermanentEffects { get; private set; }

    public bool BanishAfterPlay { get; set; }

    public virtual void LoadCard(Card card)
    {
        cardArt = card.CardArt;
        cardBorder = card.CardBorder;
        energyCost = card.StartEnergyCost;
        costConditionType = card.CostConditionType;
        costConditionValue = card.CostConditionValue;
        costConditionModifier = card.CostConditionModifier;
        CurrentEnergyCost = energyCost;
        cardName = card.CardName;
        cardType = card.CardType;
        cardSubType = card.CardSubType;
        cardRarity = card.CardRarity;
        cardDescription = card.CardDescription;
        relatedCards = (Card[])card.relatedCards?.Clone();
        relatedCardTypes = (CreatedCardType[])card.RelatedCardTypes?.Clone();
        overController = card.OverController;
        zoomOverController = card.ZoomOverController;
        cardPlaySound = card.CardPlaySound;

        CurrentEffects = new();
        PermanentEffects = new();
    }

    public virtual void CopyCard(Card card)
    {
        cardArt = card.CardArt;
        cardBorder = card.CardBorder;
        energyCost = card.StartEnergyCost;
        costConditionType = card.CostConditionType;
        costConditionValue = card.CostConditionValue;
        costConditionModifier = card.CostConditionModifier;
        CurrentEnergyCost = card.CurrentEnergyCost;
        cardName = card.CardName;
        cardType = card.CardType;
        cardSubType = card.CardSubType;
        cardRarity = card.CardRarity;
        cardDescription = card.CardDescription;
        relatedCards = (Card[])card.relatedCards?.Clone();
        relatedCardTypes = (CreatedCardType[])card.RelatedCardTypes?.Clone();
        overController = card.OverController;
        zoomOverController = card.ZoomOverController;
        cardPlaySound = card.CardPlaySound;

        CurrentEffects = new();
        foreach (var e in card.CurrentEffects) // TESTING, new instances
        {
            var newEffect = CreateInstance(e.GetType().Name) as Effect;
            newEffect.LoadEffect(e);
            CurrentEffects.Add(newEffect);
        }

        PermanentEffects = new();
        foreach (var e in card.PermanentEffects) // TESTING, new instances // Needed for cards returned to hand
        {
            var newEffect = CreateInstance(e.GetType().Name) as Effect;
            newEffect.LoadEffect(e);
            PermanentEffects.Add(newEffect);
        }
    }
}