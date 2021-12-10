using UnityEngine;
using TMPro;

public class ItemIconPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject itemText;
    [SerializeField] private GameObject buttons;

    private EffectManager efMan;
    private PlayerManager pMan;
    private UIManager uMan;
    private HeroItem loadedItem;
    public GameObject Buttons { get => buttons; }
    public GameObject SourceIcon { get; set; }

    private void Awake() => buttons.SetActive(false);

    private void Start()
    {
        efMan = EffectManager.Instance;
        pMan = PlayerManager.Instance;
        uMan = UIManager.Instance;
    }

    public HeroItem LoadedItem
    {
        set
        {
            loadedItem = value;
            string description = "<b>" + value.ItemName + "</b>: " + value.ItemDescription;
            itemText.GetComponent<TextMeshProUGUI>().SetText(description);
        }
    }

    public void UseItem_OnClick()
    {
        if (!pMan.IsMyTurn) return;
        if (!efMan.CheckLegalTargets(loadedItem.EffectGroupList, SourceIcon, true))
        {
            uMan.CreateFleetingInfoPopup("You can't use that right now!");
            AudioManager.Instance.StartStopSound("SFX_Error");
        }
        else
        {
            efMan.StartEffectGroupList(loadedItem.EffectGroupList, SourceIcon);
        }
        ClosePopup_OnClick();
    }

    public void ClosePopup_OnClick() => uMan.DestroyItemIconPopup();
}
