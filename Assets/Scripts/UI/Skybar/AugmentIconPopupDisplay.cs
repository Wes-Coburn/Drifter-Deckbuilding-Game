using UnityEngine;
using TMPro;

public class AugmentIconPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject augmentText;

    public HeroAugment HeroAugment
    {
        set
        {
            string description = "<b>" + value.AugmentName + ":</b> " + value.AugmentDescription;
            augmentText.GetComponent<TextMeshProUGUI>().SetText(description);
        }
    }
}
