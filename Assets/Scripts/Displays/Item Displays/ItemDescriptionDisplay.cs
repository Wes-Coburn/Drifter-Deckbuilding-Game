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

    public bool IsItemRemoval { get; set; }
    public HeroItem LoadedItem
    {
        set
        {
            loadedItem = value;
            itemImage.GetComponent<Image>().sprite = loadedItem.ItemImage;
            itemName.GetComponent<TextMeshProUGUI>().SetText(loadedItem.ItemName);
            itemDescription.GetComponent<TextMeshProUGUI>().SetText
                (CardManager.Instance.FilterKeywords(loadedItem.ItemDescription));

            string text = gMan.GetItemCost(loadedItem, out bool isDiscounted, IsItemRemoval).ToString();
            TextMeshProUGUI txtGui = itemCost.GetComponent<TextMeshProUGUI>();
            if (IsItemRemoval) text = "+" + text; // TESTING
            txtGui.SetText(text);

            if (!IsItemRemoval && isDiscounted)
            {
                Button button = GetComponent<Button>();
                var colors = button.colors;
                colors.normalColor = Color.green;
                button.colors = colors;
            }

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
        if (anMan.ProgressBarRoutine != null) return;

        if (IsItemRemoval) // TESTING
        {
            uMan.CreateRemoveItemPopup(loadedItem);
            return;
        }

        if (pMan.HeroItems.Count >= pMan.GetMaxItems())
            uMan.CreateFleetingInfoPopup("You can't have more than " + pMan.GetMaxItems() + " items!");
        else if (pMan.AetherCells < gMan.GetItemCost(loadedItem, out _, false))
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
