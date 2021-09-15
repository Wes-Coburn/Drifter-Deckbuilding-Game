using UnityEngine;
using TMPro;

public class DescriptionPopupDisplay : MonoBehaviour
{
    public void DisplayDescriptionPopup(string heroLore) => 
        GetComponentInChildren<TextMeshProUGUI>().SetText(heroLore);
}
