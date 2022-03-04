using UnityEngine;
using TMPro;

public class RecruitUnitPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;

    private UIManager uMan;
    private PlayerManager pMan;
    private GameManager gMan;

    private UnitCard unitCard;

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
    }

    public UnitCard UnitCard
    {
        set
        {
            int aether = pMan.AetherCells;
            unitCard = value;
            string text = "Recruit " + unitCard.CardName +
                " for " + gMan.GetRecruitCost(unitCard) +
                " aether? (You have " + aether + " aether)";
            PopupText = text;
        }
    }

    public void ConfirmButton_OnClick()
    {
        CardManager.Instance.AddCard(unitCard, GameManager.PLAYER);
        pMan.AetherCells -= gMan.GetRecruitCost(unitCard);
        bool isReady = false;
        int previousProgress = gMan.RecruitLoyalty;
        if (++gMan.RecruitLoyalty == GameManager.RECRUIT_LOYALTY_GOAL) isReady = true;
        else if (gMan.RecruitLoyalty > GameManager.RECRUIT_LOYALTY_GOAL) gMan.RecruitLoyalty = 0; // TESTING
        uMan.CreateCardPagePopup(CardPageDisplay.CardPageType.RecruitUnit);
        FindObjectOfType<CardPageDisplay>().SetProgressBar(previousProgress, gMan.RecruitLoyalty, isReady);
    }

    public void CancelButton_OnClick() =>
        uMan.DestroyRecruitUnitPopup();
}
