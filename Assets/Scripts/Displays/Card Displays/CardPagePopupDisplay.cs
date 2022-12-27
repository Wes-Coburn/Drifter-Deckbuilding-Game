using UnityEngine;
using TMPro;

public class CardPagePopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;

    private UIManager uMan;
    private PlayerManager pMan;
    private GameManager gMan;
    private CardManager caMan;
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

    private void Awake()
    {
        uMan = UIManager.Instance;
        pMan = PlayerManager.Instance;
        gMan = GameManager.Instance;
        caMan = CardManager.Instance;
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

        uMan.CreateCardPage(cardPageType, false);
        AnimationManager.Instance.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress);

        if (setProgressBar) FindObjectOfType<CardPageDisplay>().SetProgressBar
                (previousProgress, currentProgress, rewardIsReady);
    }

    private void RemoveCard()
    {
        pMan.AetherCells += cardCost;
        caMan.RemovePlayerCard(card);
    }

    private void RecruitUnit()
    {
        int recruitIndex = caMan.PlayerRecruitUnits.FindIndex(x => x.CardName == card.CardName);
        if (recruitIndex != -1) caMan.PlayerRecruitUnits.RemoveAt(recruitIndex);
        else Debug.LogError("RECRUIT UNIT NOT FOUND!");

        pMan.AetherCells -= cardCost;
        previousProgress = gMan.RecruitLoyalty;
        if (++gMan.RecruitLoyalty == GameManager.RECRUIT_LOYALTY_GOAL) rewardIsReady = true;
        else if (gMan.RecruitLoyalty > GameManager.RECRUIT_LOYALTY_GOAL) gMan.RecruitLoyalty = 0;
        currentProgress = gMan.RecruitLoyalty;

        caMan.AddCard(card, GameManager.PLAYER, true);
    }

    private void AcquireAction()
    {
        int actionIndex = caMan.ActionShopCards.FindIndex(x => x.CardName == card.CardName);
        if (actionIndex != -1) caMan.ActionShopCards.RemoveAt(actionIndex);
        else Debug.LogError("ACQUIRED ACTION NOT FOUND!");

        pMan.AetherCells -= cardCost;
        previousProgress = gMan.ActionShopLoyalty;
        if (++gMan.ActionShopLoyalty == GameManager.ACTION_LOYALTY_GOAL) rewardIsReady = true;
        else if (gMan.ActionShopLoyalty > GameManager.ACTION_LOYALTY_GOAL) gMan.ActionShopLoyalty = 0;
        currentProgress = gMan.ActionShopLoyalty;

        caMan.AddCard(card, GameManager.PLAYER, true);
    }

    private void CloneUnit()
    {
        pMan.AetherCells -= cardCost;
        caMan.AddCard(card, GameManager.PLAYER, true);
    }

    public void CancelButton_OnClick() => uMan.DestroyInteractablePopup(gameObject);
}
