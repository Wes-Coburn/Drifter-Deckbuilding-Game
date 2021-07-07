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
    [SerializeField] private GameObject cardName;
    [SerializeField] private GameObject cardArt;
    [SerializeField] private GameObject cardBorder;
    [SerializeField] private GameObject cardTypeLine;
    [SerializeField] private GameObject actionCost;
    
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
        cardArt.GetComponent<SpriteRenderer>().sprite = CardScript.CardArt;
        cardBorder.GetComponent<SpriteRenderer>().sprite = CardScript.CardBorder;
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
        CardDisplay cd = parentCard.GetComponent<CardDisplay>();
        SetCardName(cd.GetCardName());
        SetCardTypeLine(cd.GetCardType(), cd.GetCardSubType());
        SetActionCost(cd.GetActionCost());
        cardArt.GetComponent<SpriteRenderer>().sprite = cd.cardArt.GetComponent<SpriteRenderer>().sprite;
        cardBorder.GetComponent<SpriteRenderer>().sprite = cd.cardBorder.GetComponent<SpriteRenderer>().sprite;
    }

    /******
     * *****
     * ****** SETTERS/GETTERS
     * *****
     *****/
    public void SetCardName(string cardName) => this.cardName.GetComponent<TextMeshPro>().text = cardName;
    public void SetCardArt(Sprite cardArt) => this.cardArt.GetComponent<SpriteRenderer>().sprite = cardArt;
    public string GetCardName() => cardName.GetComponent<TextMeshPro>().text;
    public void SetCardTypeLine(string cardType, string cardSubType)
    {
        string separator = " ";
        if (!string.IsNullOrEmpty(cardSubType)) separator = " - ";
        cardTypeLine.GetComponent<TextMeshPro>().text = cardType + separator + cardSubType;
    }
    public string GetCardType() => CardScript.CardType;
    public string GetCardSubType() => CardScript.CardSubType;
    public void SetActionCost(int actionCost) => this.actionCost.GetComponent<TextMeshPro>().text = actionCost.ToString();
    public int GetActionCost() => System.Convert.ToInt32(actionCost.GetComponent<TextMeshPro>().text);
}
