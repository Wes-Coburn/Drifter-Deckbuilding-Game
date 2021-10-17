using UnityEngine;
using TMPro;

public class AcquireAugmentPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;

    private PlayerManager pMan;
    private HeroAugment heroAugment;

    private string PopupText
    {
        set
        {
            popupText.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    
    private void Awake()
    {
        pMan = PlayerManager.Instance;
    }

    public HeroAugment HeroAugment
    {
        set
        {
            int aether = pMan.AetherCells;
            heroAugment = value;
            string text = "Acquire " + heroAugment.AugmentName +
                " for " + GameManager.ACQUIRE_AUGMENT_COST + " aether? (You have " + aether + " aether)";
            PopupText = text;
        }
    }

    public void ConfirmButton_OnClick()
    {
        pMan.AddAugment(heroAugment);
        pMan.AetherCells -= GameManager.ACQUIRE_AUGMENT_COST;
        CancelButton_OnClick();
        FindObjectOfType<HomeBaseSceneDisplay>().CloseAugmentsButton_OnClick(); // Temporary fix, eventually reload augments?
        // Augment acquired popup
    }

    public void CancelButton_OnClick() =>
        UIManager.Instance.DestroyAcquireAugmentPopup();
}
