using System.Collections.Generic;
using UnityEngine;

public abstract class Card : ScriptableObject
{
    [SerializeField] private Sprite cardArt;
    [SerializeField] private Sprite cardBorder;
    [SerializeField, Range(0, GameManager.MAXIMUM_ENERGY)] private int energyCost;
    [SerializeField] private Effect.ConditionType costConditionType;
    [SerializeField] private int costConditionValue;
    [SerializeField] private string cardName;
    [SerializeField] private string cardType;
    [SerializeField] private string cardSubType;
    public enum Rarity
    {
        Common,
        Rare,
        Legend
    }
    [SerializeField] private Rarity cardRarity;
    [TextArea] [SerializeField] private string cardDescription;
    [SerializeField] private Sound cardPlaySound;
    [SerializeField] private AnimatorOverrideController overController;
    [SerializeField] private AnimatorOverrideController zoomOverController;

    public Sprite CardArt { get => cardArt; }
    public Sprite CardBorder { get => cardBorder; }
    public int StartEnergyCost { get => energyCost; }
    public Effect.ConditionType CostConditionType { get => costConditionType; }
    public int CostConditionValue { get => costConditionValue; }
    public string CardName { get => cardName; }
    public string CardType { get => cardType; }
    public string CardSubType { get => cardSubType; }
    public Rarity CardRarity { get => cardRarity; }
    public string CardDescription { get => cardDescription; }
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
        CurrentEnergyCost = energyCost;
        cardName = card.CardName;
        cardType = card.CardType;
        cardSubType = card.CardSubType;
        cardRarity = card.CardRarity;
        cardDescription = card.CardDescription;
        overController = card.OverController;
        zoomOverController = card.ZoomOverController;
        cardPlaySound = card.CardPlaySound;

        CurrentEffects = new List<Effect>();
        PermanentEffects = new List<Effect>();
    }

    public virtual void CopyCard(Card card)
    {
        cardArt = card.CardArt;
        cardBorder = card.CardBorder;
        energyCost = card.StartEnergyCost;
        costConditionType = card.CostConditionType;
        costConditionValue = card.CostConditionValue;
        CurrentEnergyCost = card.CurrentEnergyCost;
        cardName = card.CardName;
        cardType = card.CardType;
        cardSubType = card.CardSubType;
        cardRarity = card.CardRarity;
        cardDescription = card.CardDescription;
        overController = card.OverController;
        zoomOverController = card.ZoomOverController;
        cardPlaySound = card.CardPlaySound;

        CurrentEffects = new List<Effect>();
        foreach (Effect e in card.CurrentEffects) // TESTING, new instances
        {
            Effect newEffect = CreateInstance(e.GetType().Name) as Effect;
            newEffect.LoadEffect(e);
            CurrentEffects.Add(newEffect);
        }

        PermanentEffects = new List<Effect>();
        foreach (Effect e in card.PermanentEffects) // TESTING, new instances // Needed for cards returned to hand
        {
            Effect newEffect = CreateInstance(e.GetType().Name) as Effect;
            newEffect.LoadEffect(e);
            PermanentEffects.Add(newEffect);
        }
    }
}