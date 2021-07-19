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

    public string CardName
    {
        get => CardScript.CardName;
        set => cardName.GetComponent<TextMeshPro>().text = value;
    }
    [SerializeField] private GameObject cardName;

    public Sprite CardArt
    {
        get => CardScript.CardArt;
        set => cardArt.GetComponent<SpriteRenderer>().sprite = value;
    }
    [SerializeField] private GameObject cardArt;

    public Sprite CardBorder
    {
        get => CardScript.CardBorder;
        set => cardBorder.GetComponent<SpriteRenderer>().sprite = value;
    }
    [SerializeField] private GameObject cardBorder;

    public string CardTypeLine
    {
        get => CardScript.CardType + " - " + CardScript.CardSubType;
        set => cardTypeLine.GetComponent<TextMeshPro>().SetText(value);
    }
    [SerializeField] private GameObject cardTypeLine;

    public int CurrentActionCost
    {
        get => CardScript.CurrentActionCost;
        set
        {
            CardScript.CurrentActionCost = value;
            actionCost.GetComponent<TextMeshPro>().SetText(value.ToString());
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
    public virtual void DisplayZoomCard(GameObject parentCard)
    {
        CardDisplay cd = parentCard.GetComponent<CardDisplay>();
        cardScript = cd.CardScript;
        CardName = cd.CardName;
        CardTypeLine = cd.CardTypeLine;
        CurrentActionCost = cd.CurrentActionCost;
        CardArt = cd.CardArt;
        CardBorder = cd.CardBorder;

        if (gameObject.TryGetComponent<Animator>(out animator))
        {
            animator.runtimeAnimatorController = CardScript.ZoomOverController;
            AnimationManager.Instance.ZoomedState(gameObject);
        }
    }
}
