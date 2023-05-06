using TMPro;
using UnityEngine;

public class ItemIconPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject itemText, buttons;

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
                Managers.CA_MAN.FilterKeywords(value.ItemDescription);
            itemText.GetComponent<TextMeshProUGUI>().SetText(description);
        }
    }

    public void UseItem_OnClick()
    {
        if (!Managers.P_MAN.IsMyTurn || Managers.EF_MAN.EffectsResolving || Managers.EV_MAN.ActionsDelayed) return;

        if (SourceIcon == null)
        {
            Debug.LogError("SOURCE ICON IS NULL!");
            return;
        }

        if (!Managers.EF_MAN.CheckLegalTargets(loadedItem.EffectGroupList, SourceIcon, true))
        {
            Managers.U_MAN.CreateFleetingInfoPopup("You can't use that right now!");
            Managers.AU_MAN.StartStopSound("SFX_Error");
        }
        else Managers.EF_MAN.StartEffectGroupList(loadedItem.EffectGroupList, SourceIcon);
        ClosePopup_OnClick();
    }

    public void ClosePopup_OnClick() => Managers.U_MAN.DestroyItemIconPopup();
}
