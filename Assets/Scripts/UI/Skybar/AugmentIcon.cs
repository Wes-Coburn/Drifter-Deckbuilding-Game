using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AugmentIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject iconImage;

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

    public void OnPointerEnter(PointerEventData pointerEventData) =>
        ManagerHandler.U_MAN.CreateAugmentIconPopup(LoadedAugment, gameObject);

    public void OnPointerExit(PointerEventData pointerEventData) =>
        ManagerHandler.U_MAN.DestroyAugmentIconPopup();

    private void OnDisable() => UIManager.Instance.DestroyAugmentIconPopup(); // TESTING
}
