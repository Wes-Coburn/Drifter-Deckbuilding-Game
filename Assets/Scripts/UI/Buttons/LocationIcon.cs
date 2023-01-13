using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private UIManager uMan;
    private GameManager gMan;

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

            bool open = gMan.LocationOpen(location);
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

    private void Awake()
    {
        uMan = UIManager.Instance;
        gMan = GameManager.Instance;
    }

    public void OnClick()
    {
        if (Location.IsHomeBase) TravelPopup();
        else
        {
            if (gMan.CurrentHour == 4) TravelError("You must rest at your ship!");
            else if (!gMan.LocationOpen(Location)) TravelError("Location closed! Come back later.");
            else TravelPopup();
            
            void TravelError(string text)
            {
                uMan.CreateFleetingInfoPopup(text);
                uMan.DestroyTravelPopup();
            }
        }

        void TravelPopup() => uMan.CreateTravelPopup(Location);
    }

    public void OnPointerEnter() => uMan.CreateLocationPopup(Location);

    public void OnPointerExit() => uMan.DestroyLocationPopup();
}
