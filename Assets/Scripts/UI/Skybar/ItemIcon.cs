using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject iconImage;
    [SerializeField] private GameObject itemUsedIcon;

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
            IsUsed = loadedItem.IsUsed; // TESTING
        }
    }

    public bool IsUsed
    {
        set
        {
            loadedItem.IsUsed = value;
            itemUsedIcon.SetActive(loadedItem.IsUsed);
        }
    }

    private void Start() => uMan = UIManager.Instance;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (EffectManager.Instance.EffectsResolving || EventManager.Instance.ActionsDelayed) return;
        if (!SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene)) return;

        if (loadedItem.IsUsed)
        {
            uMan.CreateFleetingInfoPopup("Item already used this combat!");
            AudioManager.Instance.StartStopSound("SFX_Error");
            return;
        }

        uMan.CreateItemIconPopup(loadedItem, gameObject, true);
        FunctionTimer.StopTimer(ITEM_ABILITY_POPUP_TIMER);
        uMan.DestroyItemAbilityPopup();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (uMan.ConfirmUseItemPopup != null) return;

        uMan.CreateItemIconPopup(loadedItem, gameObject);
        FunctionTimer.Create(() =>
        AbilityPopupTimer(), 0.5f, ITEM_ABILITY_POPUP_TIMER);

        void AbilityPopupTimer()
        {
            if (this == null) return;
            uMan.CreateItemAbilityPopup(loadedItem);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (uMan.ConfirmUseItemPopup != null) return;
        FunctionTimer.StopTimer(ITEM_ABILITY_POPUP_TIMER);
        uMan.DestroyItemIconPopup();
        uMan.DestroyItemAbilityPopup();
    }

    private void OnDisable() // TESTING
    {
        UIManager uMan = UIManager.Instance;
        FunctionTimer.StopTimer(ITEM_ABILITY_POPUP_TIMER);
        uMan.DestroyItemIconPopup();
        uMan.DestroyItemAbilityPopup();
    }
}
