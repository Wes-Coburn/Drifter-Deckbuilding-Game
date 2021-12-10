using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemDescriptionDisplay : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject itemImage;
    [SerializeField] private GameObject itemDescription;

    private const string ITEM_ABILITY_POPUP_TIMER = "ItemAbilityPopupTimer";

    private PlayerManager pMan;
    private UIManager uMan;
    private HeroItem loadedItem;

    public HeroItem LoadedItem
    {
        set
        {
            loadedItem = value;
            itemImage.GetComponent<Image>().sprite = loadedItem.ItemImage;
            string description = "<b>" + loadedItem.ItemName + ":</b> " + loadedItem.ItemDescription;
            itemDescription.GetComponent<TextMeshProUGUI>().SetText(description);
        }
    }

    private void Awake()
    {
        pMan = PlayerManager.Instance;
        uMan = UIManager.Instance;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pMan.HeroItems.Count >= 5)
            uMan.CreateFleetingInfoPopup("You can't have more than 5 items!", true);
        else if (pMan.AetherCells < GameManager.BUY_ITEM_COST)
            uMan.InsufficientAetherPopup();
        else uMan.CreateBuyItemPopup(loadedItem);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        FunctionTimer.Create(() =>
        uMan.CreateItemAbilityPopup(loadedItem), 0.5f, ITEM_ABILITY_POPUP_TIMER);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        FunctionTimer.StopTimer(ITEM_ABILITY_POPUP_TIMER);
        uMan.DestroyItemAbilityPopup();
    }
}
