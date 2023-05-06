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

        text += $" <b><u>{card.CardName}</u></b> for {TextFilter.Clrz_red(cardCost + "", false)} aether?";
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

        Managers.U_MAN.CreateCardPage(cardPageType, true);
        Managers.AN_MAN.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress);

        if (setProgressBar) FindObjectOfType<CardPageDisplay>().SetProgressBar
                (previousProgress, currentProgress, rewardIsReady);
    }

    private void RemoveCard()
    {
        Managers.P_MAN.CurrentAether += cardCost;
        Managers.P_MAN.DeckList.Remove(card);
    }

    private void RecruitUnit()
    {
        int recruitIndex = Managers.CA_MAN.PlayerRecruitUnits.FindIndex(x => x.CardName == card.CardName);
        if (recruitIndex != -1) Managers.CA_MAN.PlayerRecruitUnits.RemoveAt(recruitIndex);
        else Debug.LogError("RECRUIT UNIT NOT FOUND!");

        Managers.P_MAN.CurrentAether -= cardCost;
        previousProgress = Managers.G_MAN.RecruitLoyalty;
        if (++Managers.G_MAN.RecruitLoyalty == GameManager.RECRUIT_LOYALTY_GOAL) rewardIsReady = true;
        else if (Managers.G_MAN.RecruitLoyalty > GameManager.RECRUIT_LOYALTY_GOAL) Managers.G_MAN.RecruitLoyalty = 0;
        currentProgress = Managers.G_MAN.RecruitLoyalty;

        Managers.CA_MAN.AddCard(card, Managers.P_MAN, true);
    }

    private void AcquireAction()
    {
        int actionIndex = Managers.CA_MAN.ActionShopCards.FindIndex(x => x.CardName == card.CardName);
        if (actionIndex != -1) Managers.CA_MAN.ActionShopCards.RemoveAt(actionIndex);
        else Debug.LogError("ACQUIRED ACTION NOT FOUND!");

        Managers.P_MAN.CurrentAether -= cardCost;
        previousProgress = Managers.G_MAN.ActionShopLoyalty;
        if (++Managers.G_MAN.ActionShopLoyalty == GameManager.ACTION_LOYALTY_GOAL) rewardIsReady = true;
        else if (Managers.G_MAN.ActionShopLoyalty > GameManager.ACTION_LOYALTY_GOAL) Managers.G_MAN.ActionShopLoyalty = 0;
        currentProgress = Managers.G_MAN.ActionShopLoyalty;

        Managers.CA_MAN.AddCard(card, Managers.P_MAN, true);
    }

    private void CloneUnit()
    {
        Managers.P_MAN.CurrentAether -= cardCost;
        Managers.CA_MAN.AddCard(card, Managers.P_MAN, true);
    }

    public void CancelButton_OnClick() => Managers.U_MAN.DestroyInteractablePopup(gameObject);
}
