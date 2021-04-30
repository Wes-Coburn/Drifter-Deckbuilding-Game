using UnityEngine;
using TMPro;

public class CenterPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;
    public void DisplayCenterPopup(string displayText) => popupText.GetComponent<TextMeshPro>().SetText(displayText);
}
