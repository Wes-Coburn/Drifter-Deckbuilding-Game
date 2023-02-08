using TMPro;
using UnityEngine;

public class AugmentIconPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject augmentText;

    public HeroAugment HeroAugment
    {
        set
        {
            string description = "<b><u>" + value.AugmentName + "</u></b>\n" + value.AugmentDescription;
            augmentText.GetComponent<TextMeshProUGUI>().SetText(description);
        }
    }
}
