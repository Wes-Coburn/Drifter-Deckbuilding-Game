using UnityEngine;

public abstract class Card : ScriptableObject
{
    public Sprite CardArt { get => cardArt; }
    [SerializeField] private Sprite cardArt;

    public Sprite CardBorder { get => cardBorder; }
    [SerializeField] private Sprite cardBorder;

    public int StartActionCost { get => actionCost; }
    [SerializeField] private int actionCost;
    public int CurrentActionCost { get; set; }

    public string CardName { get => cardName; }
    [SerializeField] private string cardName;

    public string CardType { get => cardType; }
    [SerializeField] private string cardType;

    public string CardSubType { get => cardSubType; }
    [SerializeField] private string cardSubType;

    public string CardDescription { get => cardDescription; }
    [TextArea] [SerializeField] private string cardDescription;

    public AnimatorOverrideController AnimatorOverrideController { get => animatorOverrideController; }
    [SerializeField] private AnimatorOverrideController animatorOverrideController;

    public virtual void LoadCard(Card card)
    {
        cardArt = card.CardArt;
        cardBorder = card.CardBorder;
        actionCost = card.StartActionCost;
        cardName = card.CardName;
        cardType = card.CardType;
        cardSubType = card.CardSubType;
        cardDescription = card.CardDescription;
        animatorOverrideController = card.AnimatorOverrideController;
    }
}