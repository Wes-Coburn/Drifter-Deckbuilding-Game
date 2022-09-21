using System.Collections.Generic;
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
    [SerializeField] private GameObject rareIcon;
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
            if (!string.IsNullOrEmpty(CardScript.CardType) &&
                !string.IsNullOrEmpty(CardScript.CardSubType)) spacer = " - ";
            return CardScript.CardType + spacer + CardScript.CardSubType;
        }
        set => cardTypeLine.GetComponent<TextMeshProUGUI>().SetText(value);
    }
    public int CurrentEnergyCost
    {
        get
        {
            int cost = CardScript.CurrentEnergyCost;
            if (cost < 0) return 0;
            else return cost;
        }
        private set
        {
            CardScript.CurrentEnergyCost = value;
            DisplayEnergyCost(value);
        }
    }

    private void DisplayEnergyCost(int cost)
    {
        TextMeshProUGUI txtGui = energyCost.GetComponentInChildren<TextMeshProUGUI>();
        txtGui.SetText(cost.ToString());

        int startCost = CardScript.StartEnergyCost;
        if (cost < startCost) txtGui.color = Color.green;
        else if (cost > startCost) txtGui.color = Color.red;
        else txtGui.color = Color.white;
    }

    public List<Effect> CurrentEffects { get; set; }

    protected virtual void Awake() => CurrentEffects = new List<Effect>();

    /******
     * *****
     * ****** CHANGE_CURRENT_ENERGY_COST
     * *****
     *****/
    public void ChangeCurrentEnergyCost(int value)
    {
        CardScript.CurrentEnergyCost += value;
        DisplayEnergyCost(CurrentEnergyCost);
    }

    /******
     * *****
     * ****** DISPLAY_CARD
     * *****
     *****/
    protected virtual void DisplayCard()
    {
        CardName = CardScript.CardName;
        CardArt = CardScript.CardArt;
        CardBorder = CardScript.CardBorder;
        string spacer = "";
        if (!string.IsNullOrEmpty(CardScript.CardSubType)) spacer = " - ";
        CardTypeLine = CardScript.CardType + spacer + CardScript.CardSubType;
        rareIcon.SetActive(CardScript.IsRare);
        DisplayEnergyCost(CardScript.CurrentEnergyCost); // TESTING
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
            gameObject.tag = parentCard.tag;

            if (CardZoom.ZoomCardIsCentered)
                DisplayEnergyCost(cd.cardScript.StartEnergyCost);
            else DisplayEnergyCost(cd.CurrentEnergyCost);
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
            DisplayEnergyCost(card.StartEnergyCost);

        }
        if (TryGetComponent(out animator))
        {
            animator.runtimeAnimatorController = CardScript.ZoomOverController;
            AnimationManager.Instance.ZoomedState(gameObject);
        }
        rareIcon.SetActive(CardScript.IsRare);
    }

    /******
     * *****
     * ****** DISPLAY_CARD_PAGE_CARD
     * *****
     *****/
    public virtual void DisplayCardPageCard(Card card) =>
        rareIcon.SetActive(card.IsRare);

    /******
     * *****
     * ****** DISPLAY_CHOOSE_CARD
     * *****
     *****/
    public virtual void DisplayChooseCard(Card card)
    {
        // blank
    }

    /******
     * *****
     * ****** RESET_CARD
     * *****
     *****/
    public virtual void ResetCard()
    {
        GetComponent<CardSelect>().CardOutline.SetActive(false);
        CurrentEnergyCost = CardScript.StartEnergyCost;
        CurrentEffects.Clear();
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
