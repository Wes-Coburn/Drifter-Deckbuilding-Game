using UnityEngine;
using TMPro;

public class BuyItemPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;
    private PlayerManager pMan;
    private UIManager uMan;
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
        pMan = PlayerManager.Instance;
        uMan = UIManager.Instance;
    }

    public HeroItem HeroItem
    {
        set
        {
            int aether = pMan.AetherCells;
            heroItem = value;
            string text = "Buy " + heroItem.ItemName +
                " for " + GameManager.BUY_ITEM_COST + " aether? (You have " + aether + " aether)";
            PopupText = text;
        }
    }
    
    public void ConfirmButton_OnClick()
    {
        pMan.HeroItems.Add(heroItem);
        pMan.AetherCells -= GameManager.BUY_ITEM_COST;
        GameManager.Instance.ShopItems.Remove(heroItem);
        CancelButton_OnClick();
        uMan.CreateItemPagePopup();
        uMan.SetSkybar(true);
    }

    public void CancelButton_OnClick() =>
        uMan.DestroyBuyItemPopup();
}
