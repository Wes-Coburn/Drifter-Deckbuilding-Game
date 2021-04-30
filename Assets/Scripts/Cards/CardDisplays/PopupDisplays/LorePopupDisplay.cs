using UnityEngine;
using TMPro;

public class LorePopupDisplay : MonoBehaviour
{
    public void DisplayLorePopup(string heroLore) => gameObject.GetComponent<TextMeshPro>().SetText(heroLore);
}
