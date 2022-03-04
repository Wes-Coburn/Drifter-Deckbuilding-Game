using UnityEngine;
using TMPro;

public class BuyItemPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;
    private PlayerManager pMan;
    private UIManager uMan;
    private GameManager gMan;
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
        gMan = GameManager.Instance;
    }

    public HeroItem HeroItem
    {
        set
        {
            int aether = pMan.AetherCells;
            heroItem = value;
            string text = "Buy " + heroItem.ItemName +
                " for " + gMan.GetItemCost(heroItem) +
                " aether? (You have " + aether + " aether)";
            PopupText = text;
        }
    }
    
    public void ConfirmButton_OnClick()
    {
        pMan.AddItem(heroItem, true);
        pMan.AetherCells -= gMan.GetItemCost(heroItem);
        gMan.ShopItems.Remove(heroItem);
        bool isReady = false;
        int previousProgress = gMan.ShopLoyalty;
        if (++gMan.ShopLoyalty == GameManager.SHOP_LOYALTY_GOAL) isReady = true;
        else if (gMan.ShopLoyalty > GameManager.SHOP_LOYALTY_GOAL) gMan.ShopLoyalty = 0; // TESTING
        uMan.CreateItemPagePopup();
        FindObjectOfType<ItemPageDisplay>().SetProgressBar(previousProgress, gMan.ShopLoyalty, isReady);
    }

    public void CancelButton_OnClick() =>
        uMan.DestroyBuyItemPopup();
}
