using TMPro;
using UnityEngine;

public class RemoveItemPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;

    private HeroItem heroItem;

    private string PopupText
    {
        set
        {
            popupText.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    public HeroItem HeroItem
    {
        set
        {
            heroItem = value;
            int cost;
            if (heroItem.IsRareItem) cost = GameManager.SELL_RARE_ITEM_VALUE;
            else cost = GameManager.SELL_ITEM_VALUE;

            string text = "Sell <b><u>" + heroItem.ItemName + "</u></b>" +
                " for <color=\"red\"><b>" + cost + "</b></color> aether?";
            PopupText = text;
        }
    }

    public void ConfirmButton_OnClick()
    {
        ManagerHandler.P_MAN.HeroItems.Remove(heroItem);
        int cost;
        if (heroItem.IsRareItem) cost = GameManager.SELL_RARE_ITEM_VALUE;
        else cost = GameManager.SELL_ITEM_VALUE;
        ManagerHandler.P_MAN.AetherCells += cost;

        ManagerHandler.U_MAN.CreateItemPagePopup(true, false);
    }

    public void CancelButton_OnClick() =>
        ManagerHandler.U_MAN.DestroyInteractablePopup(gameObject);
}