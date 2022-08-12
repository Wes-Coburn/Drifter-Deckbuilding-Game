using UnityEngine;

public class ChooseRewardPopupDisplay : MonoBehaviour
{
    private UIManager uMan;
    private CardManager caMan;

    private void Awake()
    {
        uMan = UIManager.Instance;
        caMan = CardManager.Instance;
    }
    public void ActionRewardButton_OnClick()
    {
        uMan.CreateNewCardPopup(null, "New Action!",
            caMan.ChooseCards(CardManager.ChooseCard.Action));

        uMan.DestroyInteractablePopup(gameObject);
    }

    public void UnitRewardButton_OnClick()
    {
        uMan.CreateNewCardPopup(null, "New Unit!",
            caMan.ChooseCards(CardManager.ChooseCard.Unit));

        uMan.DestroyInteractablePopup(gameObject);
    }
}
