using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationIcon : MonoBehaviour
{
    [SerializeField] private GameObject locationName;
    [SerializeField] private GameObject locationImage;

    [Header("BADGES"), SerializeField] private GameObject badges;
    [SerializeField] private GameObject unvisitedBadge, priorityBadge,
        nonPriorityBadge, closedBadge;

    [Header("ICONS")]
    [SerializeField] private Sprite priorityIcon, nonPriorityIcon, homeBaseSprite,
        augmenterSprite, shopSprite, recruitmentSprite, actionShopSprite, cloningSprite;

    private Location location;

    public Location Location
    {
        get => location;
        set
        {
            location = value;
            locationName.GetComponent<TextMeshProUGUI>().SetText(location.LocationName);
            transform.position = location.WorldMapPosition;

            bool visited = false;
            if (!location.IsAugmenter && GameManager.Instance.VisitedLocations.FindIndex
                (x => x == location.LocationName) != -1) visited = true;

            bool isOpen = Managers.G_MAN.LocationOpen(location);
            bool isPriority = location.IsPriorityLocation;
            closedBadge.SetActive(!isOpen);

            if (isOpen)
            {
                unvisitedBadge.SetActive(!visited);
                priorityBadge.SetActive(isPriority);
                nonPriorityBadge.SetActive(!isPriority);
            }
            else unvisitedBadge.SetActive(false);

            if ((!isOpen || !isPriority) && !location.IsHomeBase)
            {
                float scaleDown = 0.7f;
                foreach (var go in new GameObject[]
                {
                    locationImage,
                    priorityBadge,
                    nonPriorityBadge,
                    closedBadge,
                })
                {
                    var rect = go.GetComponent<RectTransform>();

                    // Scale
                    var scale = rect.localScale;
                    Vector2 newScale = new(scale.x * scaleDown, scale.y * scaleDown);
                    go.GetComponent<RectTransform>().localScale = newScale;

                    // Opacity
                    var img = go.GetComponent<Image>();
                    var clr = img.color;
                    clr.a = 0.6f;
                    img.color = clr;
                }

                locationName.GetComponent<RectTransform>().localPosition = new Vector2(0, -60);
            }

            Sprite image = null;

            // Recurring Locations
            if (location.IsHomeBase) image = homeBaseSprite;
            else if (location.IsAugmenter) image = augmenterSprite;
            else if (location.IsShop) image = shopSprite;
            else if (location.IsRecruitment) image = recruitmentSprite;
            else if (location.IsActionShop) image = actionShopSprite;
            else if (location.IsCloning) image = cloningSprite;

            // Default Locations
            else if (isPriority) image = priorityIcon;
            else
            {
                image = nonPriorityIcon;
                badges.transform.localPosition = new Vector2(10, 12);
            }

            if (image != null) locationImage.GetComponent<Image>().sprite = image;
        }
    }

    public void OnClick()
    {
        if (FindObjectOfType<ChooseRewardPopupDisplay>() != null ||
            FindObjectOfType<NarrativePopupDisplay>() != null) return;

        if (Location.IsHomeBase) TravelPopup();
        else
        {
            if (Managers.G_MAN.CurrentHour == 4) TravelError("You must rest at your ship!");
            else if (!Managers.G_MAN.LocationOpen(Location)) TravelError("Location closed! Come back later.");
            else TravelPopup();

            void TravelError(string text)
            {
                Managers.U_MAN.CreateFleetingInfoPopup(text);
                Managers.U_MAN.DestroyTravelPopup();
            }
        }

        void TravelPopup() => Managers.U_MAN.CreateTravelPopup(Location);
    }

    public void OnPointerEnter() => Managers.U_MAN.CreateLocationPopup(Location);
    public void OnPointerExit() => Managers.U_MAN.DestroyLocationPopup();
}
