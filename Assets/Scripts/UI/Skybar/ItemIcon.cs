using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject iconImage;
    [SerializeField] private GameObject itemUsedIcon;

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

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (EffectManager.Instance.EffectsResolving || EventManager.Instance.ActionsDelayed) return;
        if (!SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene)) return;

        if (loadedItem.IsUsed)
        {
            Managers.U_MAN.CreateFleetingInfoPopup("Item already used this combat!");
            AudioManager.Instance.StartStopSound("SFX_Error");
            return;
        }

        Managers.U_MAN.CreateItemIconPopup(loadedItem, gameObject, true);
        FunctionTimer.StopTimer(ITEM_ABILITY_POPUP_TIMER);
        Managers.U_MAN.DestroyItemAbilityPopup();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (Managers.U_MAN.ConfirmUseItemPopup != null) return;

        Managers.U_MAN.CreateItemIconPopup(loadedItem, gameObject);
        FunctionTimer.Create(() =>
        AbilityPopupTimer(), 0.5f, ITEM_ABILITY_POPUP_TIMER);

        void AbilityPopupTimer()
        {
            if (this == null) return;
            Managers.U_MAN.CreateItemAbilityPopup(loadedItem);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (Managers.U_MAN.ConfirmUseItemPopup != null) return;
        FunctionTimer.StopTimer(ITEM_ABILITY_POPUP_TIMER);
        Managers.U_MAN.DestroyItemIconPopup();
        Managers.U_MAN.DestroyItemAbilityPopup();
    }

    private void OnDisable() // TESTING
    {
        FunctionTimer.StopTimer(ITEM_ABILITY_POPUP_TIMER);
        Managers.U_MAN.DestroyItemIconPopup();
        Managers.U_MAN.DestroyItemAbilityPopup();
    }
}
