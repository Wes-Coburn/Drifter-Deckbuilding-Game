using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDescriptionDisplay : MonoBehaviour
{
    [SerializeField] private GameObject itemImage;
    [SerializeField] private GameObject itemDescription;

    private PlayerManager pMan;
    private UIManager uMan;
    private HeroItem loadedItem;

    public HeroItem LoadedItem
    {
        set
        {
            loadedItem = value;
            itemImage.GetComponent<Image>().sprite = loadedItem.ItemImage;
            string description = loadedItem.ItemName + ": " + loadedItem.ItemDescription;
            itemDescription.GetComponent<TextMeshProUGUI>().SetText(description);
        }
    }

    private void Awake()
    {
        pMan = PlayerManager.Instance;
        uMan = UIManager.Instance;
    }

    public void BuyItemButton_OnClick()
    {
        if (pMan.AetherCells < GameManager.BUY_ITEM_COST)
            uMan.InsufficientAetherPopup();
        else uMan.CreateBuyItemPopup(loadedItem);
    }
}
