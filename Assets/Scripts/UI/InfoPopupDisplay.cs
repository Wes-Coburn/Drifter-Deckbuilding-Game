using UnityEngine;
using TMPro;

public class InfoPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;
    public void DisplayInfoPopup(string displayText) => popupText.GetComponent<TextMeshPro>().SetText(displayText);
}
