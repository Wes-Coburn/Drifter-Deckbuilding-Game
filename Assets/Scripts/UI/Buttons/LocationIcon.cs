using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocationIcon : MonoBehaviour
{
    [SerializeField] private GameObject locationName;
    [SerializeField] private GameObject locationImage;
    
    [SerializeField] private GameObject unvisitedIcon;
    [SerializeField] private GameObject priorityIcon;
    [SerializeField] private GameObject nonPriorityIcon;
    [SerializeField] private GameObject closedIcon;

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
            closedIcon.SetActive(!open);

            if (open)
            {
                unvisitedIcon.SetActive(!visited);
                priorityIcon.SetActive(location.IsPriorityLocation);
                nonPriorityIcon.SetActive(!location.IsPriorityLocation);
            }
            else unvisitedIcon.SetActive(false);

            Sprite image = null;
            if (location.IsHomeBase) image = homeBaseSprite;
            else if (location.IsAugmenter) image = augmenterSprite;
            else if (location.IsShop) image = shopSprite;
            else if (location.IsRecruitment) image = recruitmentSprite;
            else if (location.IsActionShop) image = actionShopSprite;
            else if (location.IsCloning) image = cloningSprite;
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
