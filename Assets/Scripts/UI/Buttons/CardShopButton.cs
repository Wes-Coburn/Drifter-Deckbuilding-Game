using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardShopButton : MonoBehaviour
{
    [SerializeField] private GameObject cardCostText;

    private GameManager gMan;
    private PlayerManager pMan;
    private UIManager uMan;
    private AnimationManager anMan;
    private CardPageDisplay.CardPageType cardPageType;

    private Card card;
    private int cardCost;
    bool isDiscounted;

    private int CardCost
    {
        set
        {
            cardCost = value;
            string prefix = "";
            if (cardPageType == CardPageDisplay.CardPageType.RemoveCard) prefix = "+";
            cardCostText.GetComponent<TextMeshProUGUI>().SetText(prefix + value);
        }
    }

    private void Awake()
    {
        gMan = GameManager.Instance;
        pMan = PlayerManager.Instance;
        uMan = UIManager.Instance;
        anMan = AnimationManager.Instance;
    }

    public void SetCard(Card card, CardPageDisplay.CardPageType cardPageType)
    {
        this.card = card;
        this.cardPageType = cardPageType;
        isDiscounted = false;
        
        switch (cardPageType)
        {
            case CardPageDisplay.CardPageType.RemoveCard:
                CardCost = gMan.GetSellCost(card);
                break;
            case CardPageDisplay.CardPageType.RecruitUnit:
                CardCost = gMan.GetRecruitCost(card as UnitCard, out isDiscounted);
                break;
            case CardPageDisplay.CardPageType.AcquireAction:
                CardCost = gMan.GetActionCost(card as ActionCard, out isDiscounted);
                break;
            case CardPageDisplay.CardPageType.CloneUnit:
                CardCost = gMan.GetCloneCost(card as UnitCard);
                break;
        }

        if (isDiscounted)
        {
            Button button = cardCostText.GetComponentInParent<Button>();
            var buttonColors = button.colors;
            buttonColors.normalColor = Color.green;
            button.colors = buttonColors;
        }
    }

    public void OnClick()
    {
        if (anMan.ProgressBarRoutine != null) return;

        switch (cardPageType)
        {
            case CardPageDisplay.CardPageType.RemoveCard:
                if (pMan.DeckList.Count == GameManager.MINIMUM_DECK_SIZE)
                {
                    uMan.CreateFleetingInfoPopup("Your deck can't have less than " + GameManager.MINIMUM_DECK_SIZE + " cards!");
                    return;
                }
                else if (pMan.DeckList.Count < GameManager.MINIMUM_DECK_SIZE)
                {
                    Debug.LogError("DECK LIST < MINIMUM!");
                    return;
                }
                break;
            default:
                if (pMan.AetherCells < cardCost)
                {
                    uMan.InsufficientAetherPopup();
                    return;
                }
                break;
        }

        uMan.CreateCardPagePopup(card, cardCost, cardPageType);
    }
}
