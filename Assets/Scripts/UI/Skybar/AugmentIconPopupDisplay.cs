using UnityEngine;
using TMPro;

public class AugmentIconPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject augmentText;

    public HeroAugment HeroAugment
    {
        set => augmentText.GetComponent<TextMeshProUGUI>().SetText
            (value.AugmentName + ": " + value.AugmentDescription);
    }
}
