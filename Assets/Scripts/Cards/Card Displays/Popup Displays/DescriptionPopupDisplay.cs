using UnityEngine;
using TMPro;

public class DescriptionPopupDisplay : MonoBehaviour
{
    public void DisplayDescriptionPopup(string heroLore) => 
        gameObject.GetComponent<TextMeshPro>().SetText(heroLore);
}
