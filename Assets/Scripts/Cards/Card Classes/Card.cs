using UnityEngine;

public abstract class Card : ScriptableObject
{
    [SerializeField] private Sprite cardArt;
    [SerializeField] private Sprite cardBorder;
    [SerializeField] private int energyCost;
    [SerializeField] private string cardName;
    [SerializeField] private string cardType;
    [SerializeField] private string cardSubType;
    [SerializeField] private bool isRare;
    [TextArea] [SerializeField] private string cardDescription;
    [SerializeField] private Sound cardPlaySound;

    [SerializeField] private AnimatorOverrideController overController;
    [SerializeField] private AnimatorOverrideController zoomOverController;

    public Sprite CardArt { get => cardArt; }
    public Sprite CardBorder { get => cardBorder; }
    public int StartEnergyCost { get => energyCost; }
    public int CurrentEnergyCost { get; set; }
    public string CardName { get => cardName; }
    public string CardType { get => cardType; }
    public string CardSubType { get => cardSubType; }
    public bool IsRare { get => isRare; }
    public string CardDescription { get => cardDescription; }
    public Sound CardPlaySound { get => cardPlaySound; }

    public AnimatorOverrideController OverController { get => overController; }
    public AnimatorOverrideController ZoomOverController { get => zoomOverController; }

    public bool BanishAfterPlay { get; set; }

    public virtual void LoadCard(Card card)
    {
        cardArt = card.CardArt;
        cardBorder = card.CardBorder;
        energyCost = card.StartEnergyCost;
        cardName = card.CardName;
        cardType = card.CardType;
        cardSubType = card.CardSubType;
        isRare = card.IsRare;
        cardDescription = card.CardDescription;
        overController = card.OverController;
        zoomOverController = card.ZoomOverController;
        cardPlaySound = card.CardPlaySound;
    }
}