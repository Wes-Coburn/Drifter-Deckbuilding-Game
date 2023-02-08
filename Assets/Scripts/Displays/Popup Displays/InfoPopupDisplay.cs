using TMPro;
using UnityEngine;

public class InfoPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;
    [SerializeField] private GameObject closePopupButton;
    public void DisplayInfoPopup(string displayText, bool showCloseButton)
    {
        popupText.GetComponent<TextMeshProUGUI>().SetText(Managers.CA_MAN.FilterKeywords(displayText));
        if (closePopupButton != null) closePopupButton.SetActive(showCloseButton);
    }
    public void CloseTutorialPopup_OnClick() => UIManager.Instance.DestroyInfoPopup(UIManager.InfoPopupType.Tutorial);
}
