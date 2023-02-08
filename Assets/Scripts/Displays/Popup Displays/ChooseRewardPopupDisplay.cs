using UnityEngine;

public class ChooseRewardPopupDisplay : MonoBehaviour
{
    public void ActionRewardButton_OnClick()
    {
        ManagerHandler.U_MAN.CreateNewCardPopup(null, "New Action!",
            ManagerHandler.CA_MAN.ChooseCards(CardManager.ChooseCard.Action));

        ManagerHandler.U_MAN.DestroyInteractablePopup(gameObject);
    }

    public void UnitRewardButton_OnClick()
    {
        ManagerHandler.U_MAN.CreateNewCardPopup(null, "New Unit!",
            ManagerHandler.CA_MAN.ChooseCards(CardManager.ChooseCard.Unit));

        ManagerHandler.U_MAN.DestroyInteractablePopup(gameObject);
    }
}
