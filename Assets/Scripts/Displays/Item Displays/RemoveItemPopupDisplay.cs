using UnityEngine;
using TMPro;

public class RemoveItemPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;

    private UIManager uMan;
    private PlayerManager pMan;
    private HeroItem heroItem;

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
        pMan.HeroItems.Remove(heroItem);
        int cost;
        if (heroItem.IsRareItem) cost = GameManager.SELL_RARE_ITEM_VALUE;
        else cost = GameManager.SELL_ITEM_VALUE;
        pMan.AetherCells += cost;

        uMan.CreateItemPagePopup(true, false);
    }

    public void CancelButton_OnClick() =>
        uMan.DestroyInteractablePopup(gameObject);
}