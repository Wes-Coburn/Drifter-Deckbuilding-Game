using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationIcon : MonoBehaviour
{
    [SerializeField] private GameObject locationName;
    [SerializeField] private GameObject locationImage;

    [Header("BADGES"), SerializeField] private GameObject badges;
    [SerializeField] private GameObject unvisitedBadge;
    [SerializeField] private GameObject priorityBadge;
    [SerializeField] private GameObject nonPriorityBadge;
    [SerializeField] private GameObject closedBadge;

    [Header("ICONS")]
    [SerializeField] private Sprite priorityIcon;
    [SerializeField] private Sprite nonPriorityIcon;
    [SerializeField] private Sprite homeBaseSprite;
    [SerializeField] private Sprite augmenterSprite;
    [SerializeField] private Sprite shopSprite;
    [SerializeField] private Sprite recruitmentSprite;
    [SerializeField] private Sprite actionShopSprite;
    [SerializeField] private Sprite cloningSprite;

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

            bool open = Managers.G_MAN.LocationOpen(location);
            closedBadge.SetActive(!open);

            if (open)
            {
                unvisitedBadge.SetActive(!visited);
                priorityBadge.SetActive(location.IsPriorityLocation);
                nonPriorityBadge.SetActive(!location.IsPriorityLocation);
            }
            else unvisitedBadge.SetActive(false);

            Sprite image = null;

            // Recurring Locations
            if (location.IsHomeBase) image = homeBaseSprite;
            else if (location.IsAugmenter) image = augmenterSprite;
            else if (location.IsShop) image = shopSprite;
            else if (location.IsRecruitment) image = recruitmentSprite;
            else if (location.IsActionShop) image = actionShopSprite;
            else if (location.IsCloning) image = cloningSprite;

            // Default Locations
            else if (location.IsPriorityLocation) image = priorityIcon;
            else
            {
                image = nonPriorityIcon;
                badges.transform.localPosition = new Vector2(17, 19);
            }

            if (image != null) locationImage.GetComponent<Image>().sprite = image;
        }
    }

    public void OnClick()
    {
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
