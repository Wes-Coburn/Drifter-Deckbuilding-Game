using UnityEngine;
using TMPro;

public class InfoPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;
    public bool IsSecondary { get; set; }
    public void DisplayInfoPopup(string displayText) =>
        popupText.GetComponent<TextMeshProUGUI>().SetText(CardManager.Instance.FilterKeywords(displayText));
}
