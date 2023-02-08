using TMPro;
using UnityEngine;

public class ItemIconPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject itemText;
    [SerializeField] private GameObject buttons;

    private HeroItem loadedItem;
    public GameObject Buttons { get => buttons; }
    public GameObject SourceIcon { get; set; }

    private void Awake() => buttons.SetActive(false);

    public HeroItem LoadedItem
    {
        set
        {
            loadedItem = value;
            string description = "<b><u>" + value.ItemName + "</u></b>\n" +
                ManagerHandler.CA_MAN.FilterKeywords(value.ItemDescription);
            itemText.GetComponent<TextMeshProUGUI>().SetText(description);
        }
    }

    public void UseItem_OnClick()
    {
        if (!ManagerHandler.P_MAN.IsMyTurn || EffectManager.Instance.EffectsResolving ||
            EventManager.Instance.ActionsDelayed) return;

        if (SourceIcon == null)
        {
            Debug.LogError("SOURCE ICON IS NULL!");
            return;
        }

        if (!ManagerHandler.EF_MAN.CheckLegalTargets(loadedItem.EffectGroupList, SourceIcon, true))
        {
            ManagerHandler.U_MAN.CreateFleetingInfoPopup("You can't use that right now!");
            AudioManager.Instance.StartStopSound("SFX_Error");
        }
        else ManagerHandler.EF_MAN.StartEffectGroupList(loadedItem.EffectGroupList, SourceIcon);
        ClosePopup_OnClick();
    }

    public void ClosePopup_OnClick() => ManagerHandler.U_MAN.DestroyItemIconPopup();
}
