using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject iconImage;

    private UIManager uMan;
    private HeroItem loadedItem;
    private const string ITEM_POPUP_TIMER = "ItemPopupTimer";
    public HeroItem LoadedItem
    {
        get => loadedItem;
        set
        {
            loadedItem = value;
            iconImage.GetComponent<Image>().sprite = 
                loadedItem.ItemImage;
        }
    }

    private void Start() => 
        uMan = UIManager.Instance;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        uMan.CreateItemIconPopup(loadedItem, gameObject, true);
        FunctionTimer.StopTimer(ITEM_POPUP_TIMER);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (uMan.ConfirmUseItemPopup != null) return; // TESTING
        FunctionTimer.Create(() =>
        uMan.CreateItemIconPopup(loadedItem, gameObject), 0.5f, ITEM_POPUP_TIMER);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (uMan.ConfirmUseItemPopup != null) return; // TESTING
        FunctionTimer.StopTimer(ITEM_POPUP_TIMER);
        uMan.DestroyItemIconPopup();
    }
}
