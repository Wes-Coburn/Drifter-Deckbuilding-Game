using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocationIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject locationName, locationImage, backgroundSpotlight;

    [Header("BADGES"), SerializeField] private GameObject badges;
    [SerializeField] private GameObject unvisitedBadge, priorityBadge,
        nonPriorityBadge, closedBadge;

    [Header("ICONS")]
    [SerializeField] private Sprite priorityIcon;
    [SerializeField] private Sprite nonPriorityIcon, homeBaseSprite,
        itemShopSprite, recruitmentSprite, actionShopSprite,
        cloningSprite, augmenterSprite, healerSprite;

    private Location location;

    public Location Location
    {
        get => location;
        set
        {
            location = value;
            locationName.GetComponent<TextMeshProUGUI>().SetText(location.LocationName);
            transform.position = location.WorldMapPosition;

            bool visited = Managers.G_MAN.VisitedLocations
                .FindIndex(x => x == location.LocationName) != -1;
            bool isOpen = Managers.G_MAN.LocationOpen(location);
            bool isPriority = location.IsPriorityLocation;

            closedBadge.SetActive(!isOpen);

            // Don't use the background spotlight for non-priority locations
            // Switch the target graphic to the location image
            if (!isPriority && !location.IsHomeBase)
            {
                backgroundSpotlight.SetActive(false);
                GetComponent<Button>().targetGraphic =
                    locationImage.GetComponent<Graphic>();
            }

            if (isOpen)
            {
                unvisitedBadge.SetActive(!visited);
                priorityBadge.SetActive(isPriority);
                nonPriorityBadge.SetActive(!isPriority);
            }
            else
            {
                unvisitedBadge.SetActive(false);

                // Change selected color to red for closed locations
                var btn = GetComponent<Button>();
                var clrs = btn.colors;
                clrs.selectedColor = Color.red;
                btn.colors = clrs;
            }

            if ((!isOpen || !isPriority) && !location.IsHomeBase)
            {
                float scaleDown = 0.8f;
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
                    var img = go.GetComponentInChildren<Image>();
                    var clr = img.color;
                    clr.a = 0.8f;
                    img.color = clr;
                }

                locationName.GetComponent<RectTransform>().localPosition = new Vector2(0, -60);
            }

            Sprite image = null;

            // Recurring Locations
            if (location.IsHomeBase) image = homeBaseSprite;
            else if (location.IsItemShop) image = itemShopSprite;
            else if (location.IsRecruitment) image = recruitmentSprite;
            else if (location.IsActionShop) image = actionShopSprite;
            else if (location.IsCloner) image = cloningSprite;
            else if (location.IsAugmenter) image = augmenterSprite;
            else if (location.IsHealer) image = healerSprite;

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
        if (!Managers.U_MAN.PlayerCanTravel) return;

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

    public void OnPointerEnter(PointerEventData data)
    {
        if (!Managers.U_MAN.PlayerCanTravel) return;
        Managers.U_MAN.CreateLocationPopup(Location);
    }
    public void OnPointerExit(PointerEventData data)
    {
        if (!Managers.U_MAN.PlayerCanTravel) return;
        Managers.U_MAN.DestroyLocationPopup();
    }
}
