using UnityEngine;
using TMPro;

public class DescriptionPopupDisplay : MonoBehaviour
{
    public void DisplayDescriptionPopup(string description) => 
        GetComponentInChildren<TextMeshProUGUI>().SetText(description);
}
