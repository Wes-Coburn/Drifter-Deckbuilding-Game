using UnityEngine;
using TMPro;

public class InfoPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;
    [SerializeField] private GameObject closePopupButton;
    public GameObject ClosePopupButton => closePopupButton;
    public void DisplayInfoPopup(string displayText, bool showCloseButton = false)
    {
        popupText.GetComponent<TextMeshProUGUI>().SetText(CardManager.Instance.FilterKeywords(displayText));
        closePopupButton.SetActive(showCloseButton);
    }
    public void CloseTutorialPopup_OnClick() => UIManager.Instance.DestroyInfoPopup(UIManager.InfoPopupType.Tutorial);
}
