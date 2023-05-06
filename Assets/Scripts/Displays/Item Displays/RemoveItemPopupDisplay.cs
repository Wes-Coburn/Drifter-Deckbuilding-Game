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
            int cost = heroItem.IsRareItem ?
                GameManager.SELL_RARE_ITEM_VALUE : GameManager.SELL_ITEM_VALUE;

            string text = $"Sell <b><u>{heroItem.ItemName}</u></b>" +
                $" for {TextFilter.Clrz_red(cost + "")} aether?";
            PopupText = text;
        }
    }

    public void ConfirmButton_OnClick()
    {
        Managers.P_MAN.HeroItems.Remove(heroItem);
        int cost;
        if (heroItem.IsRareItem) cost = GameManager.SELL_RARE_ITEM_VALUE;
        else cost = GameManager.SELL_ITEM_VALUE;
        Managers.P_MAN.CurrentAether += cost;
        Managers.U_MAN.CreateItemPagePopup(true, true);
    }

    public void CancelButton_OnClick() => Managers.U_MAN.DestroyInteractablePopup(gameObject);
}