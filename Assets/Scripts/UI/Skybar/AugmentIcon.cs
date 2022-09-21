using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AugmentIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject iconImage;

    private UIManager uMan;
    private HeroAugment loadedAugment;
    //private const string AUGMENT_POPUP_TIMER = "AugmentPopupTimer";
    public HeroAugment LoadedAugment
    {
        get => loadedAugment;
        set
        {
            loadedAugment = value;
            iconImage.GetComponent<Image>().sprite = 
                loadedAugment.AugmentImage;
        }
    }

    private void Start() => uMan = UIManager.Instance;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //FunctionTimer.Create(() =>
        //uMan.CreateAugmentIconPopup(LoadedAugment, gameObject), 0.5f, AUGMENT_POPUP_TIMER);

        uMan.CreateAugmentIconPopup(LoadedAugment, gameObject);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //FunctionTimer.StopTimer(AUGMENT_POPUP_TIMER);
        uMan.DestroyAugmentIconPopup();
    }
}
