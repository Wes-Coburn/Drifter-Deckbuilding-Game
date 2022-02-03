using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocationIcon : MonoBehaviour
{
    [SerializeField] private GameObject locationName;
    [SerializeField] private GameObject locationImage;
    [SerializeField] private Sprite homeBaseSprite;
    [SerializeField] private Sprite augmenterSprite;
    [SerializeField] private GameObject unvisitedIcon;
    [SerializeField] private GameObject priorityIcon;
    [SerializeField] private GameObject nonPriorityLocation;
    
    private UIManager uMan;
    private Location location;

    public Location Location
    {
        get => location;
        set
        {
            location = value;
            locationName.GetComponent<TextMeshProUGUI>().SetText(location.LocationName);
            transform.position = location.WorldMapPosition;
            if (location.IsHomeBase) SetHomeBaseImage();
            if (location.IsAugmenter) SetAugmenterImage();
            bool unvisited = true;
            if (GameManager.Instance.VisitedLocations.FindIndex(x => x == location.LocationName) != -1)
                unvisited = false;
            unvisitedIcon.SetActive(unvisited);
            priorityIcon.SetActive(location.IsPriorityLocation);
            nonPriorityLocation.SetActive(!location.IsPriorityLocation);
        }
    }

    private void Start() => 
        uMan = UIManager.Instance;

    public void SetHomeBaseImage() =>
        locationImage.GetComponent<Image>().sprite = homeBaseSprite;

    public void SetAugmenterImage() =>
        locationImage.GetComponent<Image>().sprite = augmenterSprite;

    public void OnClick() => 
        uMan.CreateTravelPopup(Location);

    public void OnPointerEnter() => 
        uMan.CreateLocationPopup(Location);

    public void OnPointerExit() => 
        uMan.DestroyLocationPopup();
}
