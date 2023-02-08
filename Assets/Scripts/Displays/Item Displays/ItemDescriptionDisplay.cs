using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDescriptionDisplay : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject itemImage;
    [SerializeField] private GameObject itemName;
    [SerializeField] private GameObject itemDescription;
    [SerializeField] private GameObject itemCost;
    [SerializeField] private GameObject rareIcon;

    private const string ITEM_ABILITY_POPUP_TIMER = "ItemAbilityPopupTimer";

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
                (Managers.CA_MAN.FilterKeywords(loadedItem.ItemDescription));

            string text = Managers.G_MAN.GetItemCost(loadedItem, out bool isDiscounted, IsItemRemoval).ToString();
            TextMeshProUGUI txtGui = itemCost.GetComponent<TextMeshProUGUI>();
            if (IsItemRemoval) text = "+" + text;
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

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (Managers.AN_MAN.ProgressBarRoutine != null) return;

        if (IsItemRemoval)
        {
            Managers.U_MAN.CreateRemoveItemPopup(loadedItem);
            return;
        }

        int maxItems = Managers.P_MAN.GetMaxItems(out bool hasBonus);
        if (Managers.P_MAN.HeroItems.Count >= maxItems)
        {
            string text = "You can't have more than " + maxItems + " items!";
            if (!hasBonus) text += "\n(Visit <b>The Augmenter</b>)";
            Managers.U_MAN.CreateFleetingInfoPopup(text);
        }
        else if (Managers.P_MAN.AetherCells < Managers.G_MAN.GetItemCost(loadedItem, out _, false))
            Managers.U_MAN.InsufficientAetherPopup();
        else Managers.U_MAN.CreateBuyItemPopup(loadedItem);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        FunctionTimer.Create(() =>
        Managers.U_MAN.CreateItemAbilityPopup(loadedItem), 0.5f, ITEM_ABILITY_POPUP_TIMER);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        FunctionTimer.StopTimer(ITEM_ABILITY_POPUP_TIMER);
        Managers.U_MAN.DestroyItemAbilityPopup();
    }
}
