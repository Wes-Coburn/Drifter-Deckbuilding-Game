using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardShopButton : MonoBehaviour
{
    [SerializeField] private GameObject cardCostText;

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

    public void SetCard(Card card, CardPageDisplay.CardPageType cardPageType)
    {
        this.card = card;
        this.cardPageType = cardPageType;
        isDiscounted = false;

        switch (cardPageType)
        {
            case CardPageDisplay.CardPageType.RemoveCard:
                CardCost = ManagerHandler.G_MAN.GetSellCost(card);
                break;
            case CardPageDisplay.CardPageType.RecruitUnit:
                CardCost = ManagerHandler.G_MAN.GetRecruitCost(card as UnitCard, out isDiscounted);
                break;
            case CardPageDisplay.CardPageType.AcquireAction:
                CardCost = ManagerHandler.G_MAN.GetActionCost(card as ActionCard, out isDiscounted);
                break;
            case CardPageDisplay.CardPageType.CloneUnit:
                CardCost = ManagerHandler.G_MAN.GetCloneCost(card as UnitCard);
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
        if (ManagerHandler.AN_MAN.ProgressBarRoutine != null) return;

        switch (cardPageType)
        {
            case CardPageDisplay.CardPageType.RemoveCard:
                if (ManagerHandler.P_MAN.DeckList.Count == GameManager.MINIMUM_DECK_SIZE)
                {
                    ManagerHandler.U_MAN.CreateFleetingInfoPopup("Your deck can't have less than " + GameManager.MINIMUM_DECK_SIZE + " cards!");
                    return;
                }
                else if (ManagerHandler.P_MAN.DeckList.Count < GameManager.MINIMUM_DECK_SIZE)
                {
                    Debug.LogError("DECK LIST < MINIMUM!");
                    return;
                }
                break;
            default:
                if (ManagerHandler.P_MAN.AetherCells < cardCost)
                {
                    ManagerHandler.U_MAN.InsufficientAetherPopup();
                    return;
                }
                break;
        }

        ManagerHandler.U_MAN.CreateCardPagePopup(card, cardCost, cardPageType);
    }
}
