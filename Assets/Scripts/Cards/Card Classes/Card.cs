using UnityEngine;

public abstract class Card : ScriptableObject
{
    [SerializeField] private Sprite cardArt;
    [SerializeField] private Sprite cardBorder;
    [SerializeField] private int actionCost;
    [SerializeField] private string cardName;
    [SerializeField] private string cardType;
    [SerializeField] private string cardSubType;
    [TextArea] [SerializeField] private string cardDescription;
    [SerializeField] private AnimatorOverrideController overController;
    [SerializeField] private AnimatorOverrideController zoomOverController;
    [SerializeField] private Sound cardPlaySound;

    public Sprite CardArt { get => cardArt; }
    public Sprite CardBorder { get => cardBorder; }
    public int StartActionCost { get => actionCost; }
    public int CurrentActionCost { get; set; }
    public string CardName { get => cardName; }
    public string CardType { get => cardType; }
    public string CardSubType { get => cardSubType; }
    public string CardDescription { get => cardDescription; }
    public AnimatorOverrideController OverController { get => overController; }
    public AnimatorOverrideController ZoomOverController { get => zoomOverController; }

    public Sound CardPlaySound { get => cardPlaySound; }

    public virtual void LoadCard(Card card)
    {
        cardArt = card.CardArt;
        cardBorder = card.CardBorder;
        actionCost = card.StartActionCost;
        cardName = card.CardName;
        cardType = card.CardType;
        cardSubType = card.CardSubType;
        cardDescription = card.CardDescription;
        overController = card.OverController;
        zoomOverController = card.ZoomOverController;
        cardPlaySound = card.CardPlaySound;
    }
}