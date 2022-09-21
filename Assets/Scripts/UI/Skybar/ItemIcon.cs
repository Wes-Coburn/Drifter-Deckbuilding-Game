using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject iconImage;

    private UIManager uMan;
    private HeroItem loadedItem;
    //private const string ITEM_POPUP_TIMER = "ItemPopupTimer";
    private const string ITEM_ABILITY_POPUP_TIMER = "ItemAbilityPopupTimer";
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

    private void Start() => uMan = UIManager.Instance;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (EffectManager.Instance.EffectsResolving || EventManager.Instance.ActionsDelayed) return; // TESTING
        if (!SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene)) return; // TESTING
        uMan.CreateItemIconPopup(loadedItem, gameObject, true);
        //FunctionTimer.StopTimer(ITEM_POPUP_TIMER);
        FunctionTimer.StopTimer(ITEM_ABILITY_POPUP_TIMER);
        uMan.DestroyItemAbilityPopup();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (uMan.ConfirmUseItemPopup != null) return;
        //FunctionTimer.Create(() =>
        //ItemPopupTimer(), 0.5f, ITEM_POPUP_TIMER);
        //FunctionTimer.Create(() =>
        //AbilityPopupTimer(), 1, ITEM_ABILITY_POPUP_TIMER);

        uMan.CreateItemIconPopup(loadedItem, gameObject);
        FunctionTimer.Create(() =>
        AbilityPopupTimer(), 0.5f, ITEM_ABILITY_POPUP_TIMER);

        /*
        void ItemPopupTimer()
        {
            if (this == null) return;
            uMan.CreateItemIconPopup(loadedItem, gameObject);
        }
        */
        void AbilityPopupTimer()
        {
            if (this == null) return;
            uMan.CreateItemAbilityPopup(loadedItem);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (uMan.ConfirmUseItemPopup != null) return;
        //FunctionTimer.StopTimer(ITEM_POPUP_TIMER);
        FunctionTimer.StopTimer(ITEM_ABILITY_POPUP_TIMER);
        uMan.DestroyItemIconPopup();
        uMan.DestroyItemAbilityPopup();
    }
}
