using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class CardDisplay : MonoBehaviour
{
    /* CARD_SCRIPTABLE_OBJECT */
    protected Card cardScript;
    public Card CardScript
    {
        get => cardScript;
        set
        {
            cardScript = value;
            DisplayCard();
        }
    }

    [SerializeField] private GameObject cardName;
    [SerializeField] private GameObject cardArt;
    [SerializeField] private GameObject cardBorder;
    [SerializeField] private GameObject cardTypeLine;
    [SerializeField] private GameObject energyCost;

    private GameObject cardContainer;
    private Animator animator;

    public GameObject CardContainer
    {
        get => cardContainer;
        set
        {
            if (cardContainer != null)
            {
                Debug.LogError("CARD CONTAINER ALREADY EXISTS!");
                return;
            }
            cardContainer = value;
        }
    }
    public string CardName
    {
        get => CardScript.CardName;
        set => cardName.GetComponent<TextMeshProUGUI>().text = value;
    }

    public Sprite CardArt
    {
        get => CardScript.CardArt;
        set => cardArt.GetComponent<Image>().sprite = value;
    }

    public Sprite CardBorder
    {
        get => CardScript.CardBorder;
        set => cardBorder.GetComponent<Image>().sprite = value;
    }
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
    public int CurrentEnergyCost
    {
        get => CardScript.CurrentEnergyCost;
        set
        {
            CardScript.CurrentEnergyCost = value;
            DisplayEnergyCost(value); // TESTING
        }
    }
    private void DisplayEnergyCost(int cost)
    {
        string displayCost;
        if (cost < 0) displayCost = "";
        else displayCost = cost.ToString();

        energyCost.GetComponent<TextMeshProUGUI>().
            SetText(displayCost);
    }
    
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
        CurrentEnergyCost = CardScript.StartEnergyCost;
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
                DisplayEnergyCost(cd.cardScript.StartEnergyCost); // TESTING
            else DisplayEnergyCost(cd.CurrentEnergyCost); // TESTING
        }
        else
        {
            cardScript = card;
            string spacer = "";
            if (!string.IsNullOrEmpty(card.CardSubType)) spacer = " - ";
            CardTypeLine = card.CardType + spacer + card.CardSubType;
            CardName = card.CardName;
            CardArt = card.CardArt;
            CardBorder = card.CardBorder;
            DisplayEnergyCost(card.StartEnergyCost); // TESTING

        }

        if (TryGetComponent(out animator))
        {
            animator.runtimeAnimatorController = CardScript.ZoomOverController;
            AnimationManager.Instance.ZoomedState(gameObject);
        }
    }

    /******
     * *****
     * ****** DISPLAY_CARD_PAGE_CARD
     * *****
     *****/
    public virtual void DisplayCardPageCard(Card card)
    {
        // blank
    }

    /******
     * *****
     * ****** DISABLE_VISUALS
     * *****
     *****/
    public virtual void DisableVisuals()
    {
        GetComponent<Animator>().enabled = false;
        GetComponent<CardSelect>().CardOutline.SetActive(false);
    }
}
