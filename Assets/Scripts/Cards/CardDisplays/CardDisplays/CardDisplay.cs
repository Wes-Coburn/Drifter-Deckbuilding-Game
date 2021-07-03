using UnityEngine;
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
    [SerializeField] private GameObject CardName;
    [SerializeField] private GameObject CardArt;
    [SerializeField] private GameObject CardBorder;
    [SerializeField] private GameObject CardTypeLine;
    [SerializeField] private GameObject ActionCost;

    /******
     * *****
     * ****** DISPLAY_CARD
     * *****
     *****/
    protected virtual void DisplayCard()
    {
        /* CardName */
        SetCardName(CardScript.CardName);
        /* CardType */
        SetCardTypeLine(CardScript.CardType, CardScript.CardSubType);
        /* ActionCost */
        SetActionCost(cardScript.ActionCost);
        /* Sprites */
        CardArt.GetComponent<SpriteRenderer>().sprite = CardScript.CardArt;
        CardBorder.GetComponent<SpriteRenderer>().sprite = CardScript.CardBorder;
        /* Animations */
        animator = gameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = CardScript.animatorOverrideController;
    }

    /******
     * *****
     * ****** DISPLAY_ZOOM_CARD
     * *****
     *****/
    public virtual void DisplayZoomCard(GameObject parentCard)
    {
        CardDisplay parentCardDisplay = parentCard.GetComponent<CardDisplay>();
        SetCardName(parentCardDisplay.GetCardName());
        SetCardTypeLine(parentCardDisplay.GetCardType(), parentCardDisplay.GetCardSubType());
        SetActionCost(parentCardDisplay.GetActionCost());
        CardArt.GetComponent<SpriteRenderer>().sprite = parentCardDisplay.CardArt.GetComponent<SpriteRenderer>().sprite;
        CardBorder.GetComponent<SpriteRenderer>().sprite = parentCardDisplay.CardBorder.GetComponent<SpriteRenderer>().sprite;
    }

    /******
     * *****
     * ****** SETTERS/GETTERS
     * *****
     *****/
    public void SetCardName(string cardName) => CardName.GetComponent<TextMeshPro>().text = cardName;
    public void SetCardArt(Sprite cardArt) => CardArt.GetComponent<SpriteRenderer>().sprite = cardArt;
    public string GetCardName() => CardName.GetComponent<TextMeshPro>().text;
    public void SetCardTypeLine(string cardType, string cardSubType) => CardTypeLine.GetComponent<TextMeshPro>().text = cardType + " - " + cardSubType;
    public string GetCardType() => CardScript.CardType;
    public string GetCardSubType() => CardScript.CardSubType;
    public void SetActionCost(int actionCost) => ActionCost.GetComponent<TextMeshPro>().text = actionCost.ToString();
    public int GetActionCost() => System.Convert.ToInt32(ActionCost.GetComponent<TextMeshPro>().text);
}
