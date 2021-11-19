using UnityEngine;
using TMPro;

public class RecruitUnitPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;

    private UIManager uMan;
    private PlayerManager pMan;
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
    }

    public UnitCard UnitCard
    {
        set
        {
            int aether = pMan.AetherCells;
            unitCard = value;
            string text = "Recruit " + unitCard.CardName +
                " for " + GameManager.LEARN_SKILL_COST +
                " aether? (You have " + aether + " aether)";
            PopupText = text;
        }
    }

    public void ConfirmButton_OnClick()
    {
        CardManager.Instance.AddCard(unitCard, GameManager.PLAYER);
        pMan.AetherCells -= GameManager.RECRUIT_UNIT_COST;
        CancelButton_OnClick();
        uMan.CreateCardPagePopup(CardPageDisplay.CardPageType.RecruitUnit);
    }

    public void CancelButton_OnClick() =>
        uMan.DestroyRecruitUnitPopup();
}
