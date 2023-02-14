using UnityEngine;

public class ChooseRewardPopupDisplay : MonoBehaviour
{
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
