using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemDescriptionDisplay : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject itemImage;
    [SerializeField] private GameObject itemName;
    [SerializeField] private GameObject itemDescription;
    [SerializeField] private GameObject itemCost;
    [SerializeField] private GameObject rareIcon;

    private const string ITEM_ABILITY_POPUP_TIMER = "ItemAbilityPopupTimer";

    private PlayerManager pMan;
    private UIManager uMan;
    private AnimationManager anMan;
    private GameManager gMan;
    private HeroItem loadedItem;

    public HeroItem LoadedItem
    {
        set
        {
            loadedItem = value;
            itemImage.GetComponent<Image>().sprite = loadedItem.ItemImage;
            itemName.GetComponent<TextMeshProUGUI>().SetText(loadedItem.ItemName);
            itemDescription.GetComponent<TextMeshProUGUI>().SetText
                (CardManager.Instance.FilterKeywords(loadedItem.ItemDescription));
            itemCost.GetComponent<TextMeshProUGUI>().SetText(gMan.GetItemCost(loadedItem).ToString());
            rareIcon.SetActive(loadedItem.IsRareItem);
        }
    }
    
    private void Awake()
    {
        pMan = PlayerManager.Instance;
        uMan = UIManager.Instance;
        gMan = GameManager.Instance;
        anMan = AnimationManager.Instance;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (anMan.ProgressBarRoutine != null) return; // TESTING
        if (pMan.HeroItems.Count >= 5)
            uMan.CreateFleetingInfoPopup("You can't have more than 5 items!", true);
        else if (pMan.AetherCells < gMan.GetItemCost(loadedItem))
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
