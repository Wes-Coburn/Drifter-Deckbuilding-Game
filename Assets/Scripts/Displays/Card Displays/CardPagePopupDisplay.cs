using TMPro;
using UnityEngine;

public class CardPagePopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;

    private CardPageDisplay.CardPageType cardPageType;

    private Card card;
    private int cardCost;

    private bool rewardIsReady;
    private int previousProgress;
    private int currentProgress;

    private string PopupText
    {
        set
        {
            popupText.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    public void SetCard(Card card, int cardCost, CardPageDisplay.CardPageType cardPageType)
    {
        this.card = card;
        this.cardCost = cardCost;
        this.cardPageType = cardPageType;
        rewardIsReady = false;
        string text = "";

        switch (cardPageType)
        {
            case CardPageDisplay.CardPageType.RemoveCard:
                text += "Sell";
                break;
            case CardPageDisplay.CardPageType.RecruitUnit:
                text += "Recruit";
                break;
            case CardPageDisplay.CardPageType.AcquireAction:
                text += "Buy";
                break;
            case CardPageDisplay.CardPageType.CloneUnit:
                text += "Clone";
                break;
            default:
                Debug.LogError("INVALID TYPE!");
                return;
        }

        text += " <b><u>" + card.CardName + "</u></b>" +
            " for <color=\"red\"><b>" + cardCost + "</b></color> aether?";
        PopupText = text;
    }

    public void ConfirmButton_OnClick()
    {
        bool setProgressBar = false;
        switch (cardPageType)
        {
            case CardPageDisplay.CardPageType.RemoveCard:
                RemoveCard();
                break;
            case CardPageDisplay.CardPageType.RecruitUnit:
                setProgressBar = true;
                RecruitUnit();
                break;
            case CardPageDisplay.CardPageType.AcquireAction:
                setProgressBar = true;
                AcquireAction();
                break;
            case CardPageDisplay.CardPageType.CloneUnit:
                CloneUnit();
                break;
            default:
                Debug.LogError("INVALID TYPE!");
                return;
        }

        ManagerHandler.U_MAN.CreateCardPage(cardPageType, false);
        AnimationManager.Instance.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress);

        if (setProgressBar) FindObjectOfType<CardPageDisplay>().SetProgressBar
                (previousProgress, currentProgress, rewardIsReady);
    }

    private void RemoveCard()
    {
        ManagerHandler.P_MAN.AetherCells += cardCost;
        ManagerHandler.CA_MAN.RemovePlayerCard(card);
    }

    private void RecruitUnit()
    {
        int recruitIndex = ManagerHandler.CA_MAN.PlayerRecruitUnits.FindIndex(x => x.CardName == card.CardName);
        if (recruitIndex != -1) ManagerHandler.CA_MAN.PlayerRecruitUnits.RemoveAt(recruitIndex);
        else Debug.LogError("RECRUIT UNIT NOT FOUND!");

        ManagerHandler.P_MAN.AetherCells -= cardCost;
        previousProgress = ManagerHandler.G_MAN.RecruitLoyalty;
        if (++ManagerHandler.G_MAN.RecruitLoyalty == GameManager.RECRUIT_LOYALTY_GOAL) rewardIsReady = true;
        else if (ManagerHandler.G_MAN.RecruitLoyalty > GameManager.RECRUIT_LOYALTY_GOAL) ManagerHandler.G_MAN.RecruitLoyalty = 0;
        currentProgress = ManagerHandler.G_MAN.RecruitLoyalty;

        ManagerHandler.CA_MAN.AddCard(card, GameManager.PLAYER, true);
    }

    private void AcquireAction()
    {
        int actionIndex = ManagerHandler.CA_MAN.ActionShopCards.FindIndex(x => x.CardName == card.CardName);
        if (actionIndex != -1) ManagerHandler.CA_MAN.ActionShopCards.RemoveAt(actionIndex);
        else Debug.LogError("ACQUIRED ACTION NOT FOUND!");

        ManagerHandler.P_MAN.AetherCells -= cardCost;
        previousProgress = ManagerHandler.G_MAN.ActionShopLoyalty;
        if (++ManagerHandler.G_MAN.ActionShopLoyalty == GameManager.ACTION_LOYALTY_GOAL) rewardIsReady = true;
        else if (ManagerHandler.G_MAN.ActionShopLoyalty > GameManager.ACTION_LOYALTY_GOAL) ManagerHandler.G_MAN.ActionShopLoyalty = 0;
        currentProgress = ManagerHandler.G_MAN.ActionShopLoyalty;

        ManagerHandler.CA_MAN.AddCard(card, GameManager.PLAYER, true);
    }

    private void CloneUnit()
    {
        ManagerHandler.P_MAN.AetherCells -= cardCost;
        ManagerHandler.CA_MAN.AddCard(card, GameManager.PLAYER, true);
    }

    public void CancelButton_OnClick() => ManagerHandler.U_MAN.DestroyInteractablePopup(gameObject);
}
