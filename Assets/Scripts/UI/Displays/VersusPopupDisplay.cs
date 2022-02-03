using UnityEngine;
using TMPro;

public class VersusPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;

    private string PopupText
    {
        set
        {
            popupText.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    public bool IsBossBattle
    {
        set
        {
            if (value)
            {
                PopupText = "Boss Battle";
            }
            else
            {
                PopupText = "VS";
            }
        }
    }
}
