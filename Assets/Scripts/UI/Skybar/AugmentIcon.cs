using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AugmentIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject iconImage;

    private UIManager uMan;
    private HeroAugment loadedAugment;
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

    public void OnPointerEnter(PointerEventData pointerEventData) =>
        uMan.CreateAugmentIconPopup(LoadedAugment, gameObject);

    public void OnPointerExit(PointerEventData pointerEventData) =>
        uMan.DestroyAugmentIconPopup();

    private void OnDisable() => UIManager.Instance.DestroyAugmentIconPopup(); // TESTING
}
