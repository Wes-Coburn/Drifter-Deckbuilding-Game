using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class CardDisplay : MonoBehaviour
{
    /* CARD_SCRIPTABLE_OBJECT */
    private Card cardScript;
    public Card CardScript
    {
        get => cardScript;
        set
        {
            cardScript = value;
            DisplayCard();
        }
    }

    /* CARD_DATA */
    private Animator animator;

    public string CardName
    {
        get => CardScript.CardName;
        set => cardName.GetComponent<TextMeshProUGUI>().text = value;
    }
    [SerializeField] private GameObject cardName;

    public Sprite CardArt
    {
        get => CardScript.CardArt;
        set => cardArt.GetComponent<Image>().sprite = value;
    }
    [SerializeField] private GameObject cardArt;

    public Sprite CardBorder
    {
        get => CardScript.CardBorder;
        set => cardBorder.GetComponent<Image>().sprite = value;
    }
    [SerializeField] private GameObject cardBorder;

    public string CardTypeLine
    {
        get
        {
            string spacer = "";
            if (!string.IsNullOrEmpty(CardScript.CardSubType)) spacer = " - ";
            return CardScript.CardType + spacer + CardScript.CardSubType;
        }
        set => cardTypeLine.GetComponent<TextMeshProUGUI>().SetText(value);
    }
    [SerializeField] private GameObject cardTypeLine;

    public int CurrentActionCost
    {
        get => CardScript.CurrentActionCost;
        set
        {
            CardScript.CurrentActionCost = value;
            actionCost.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
        }
    }
    [SerializeField] private GameObject actionCost;
    
    /******
     * *****
     * ****** DISPLAY_CARD
     * *****
     *****/
    protected virtual void DisplayCard()
    {
        CardName = CardScript.CardName;
        string spacer = "";
        if (!string.IsNullOrEmpty(CardScript.CardSubType)) spacer = " - ";
        CardTypeLine = CardScript.CardType + spacer + CardScript.CardSubType;
        CurrentActionCost = CardScript.StartActionCost;
        CardArt = CardScript.CardArt;
        CardBorder = CardScript.CardBorder;
        animator = gameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = CardScript.OverController;
    }

    /******
     * *****
     * ****** DISPLAY_ZOOM_CARD
     * *****
     *****/
    public virtual void DisplayZoomCard(GameObject parentCard, Card card = null)
    {
        if (card == null)
        {
            CardDisplay cd = parentCard.GetComponent<CardDisplay>();
            cardScript = cd.CardScript; // MUST COME FIRST
            CardTypeLine = cd.CardTypeLine;
            CardName = cd.CardName;
            CardArt = cd.CardArt;
            CardBorder = cd.CardBorder;

            if (CardZoom.ZoomCardIsCentered) 
                CurrentActionCost = cd.cardScript.StartActionCost;
            else CurrentActionCost = cd.CurrentActionCost;
        }
        else
        {
            cardScript = card;
            string spacer = "";
            if (!string.IsNullOrEmpty(card.CardSubType)) spacer = " - ";
            CardTypeLine = card.CardType + spacer + card.CardSubType;
            CurrentActionCost = card.StartActionCost;
            CardName = card.CardName;
            CardArt = card.CardArt;
            CardBorder = card.CardBorder;
        }
        
        if (TryGetComponent(out animator))
        {
            animator.runtimeAnimatorController = CardScript.ZoomOverController;
            AnimationManager.Instance.ZoomedState(gameObject);
        }
    }
}
