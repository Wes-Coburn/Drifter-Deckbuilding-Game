using TMPro;
using UnityEngine;

public class ChooseRewardPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupTitle;

    public static int BonusRewards = 0;

    private void Awake()
    {
        var titleTxt = popupTitle.GetComponent<TextMeshProUGUI>();
        if (BonusRewards > 0) titleTxt.SetText($"<u>Choose Bonus Cards!</u>\n--- {BonusRewards} left ---");
        else titleTxt.SetText("<u>Choose Your Reward!</u>");
    }

    public void ActionRewardButton_OnClick()
    {
        Managers.U_MAN.CreateNewCardPopup(null, "New Action!",
            Managers.CA_MAN.ChooseCards(CardManager.ChooseCard.Action));
        Managers.U_MAN.DestroyInteractablePopup(gameObject);
    }

    public void UnitRewardButton_OnClick()
    {
        Managers.U_MAN.CreateNewCardPopup(null, "New Unit!",
            Managers.CA_MAN.ChooseCards(CardManager.ChooseCard.Unit));
        Managers.U_MAN.DestroyInteractablePopup(gameObject);
    }
}
