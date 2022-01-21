using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocationIcon : MonoBehaviour
{
    [SerializeField] private GameObject locationName;
    [SerializeField] private GameObject locationImage;
    [SerializeField] private Sprite homeBaseSprite;
    [SerializeField] private GameObject unvisitedIcon;
    
    private UIManager uMan;
    public GameObject UnvisitedIcon { get => unvisitedIcon; }
    public string LocationName
    {
        set
        {
            locationName.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    public Vector2 WorldMapPosition
    {
        set
        {
            transform.position = value;
        }
    }

    public Location Location { get; set; }

    private void Start() => 
        uMan = UIManager.Instance;

    public void SetHomeBaseImage() =>
        locationImage.GetComponent<Image>().sprite = homeBaseSprite;

    public void OnClick() => 
        uMan.CreateTravelPopup(Location);

    public void OnPointerEnter() => 
        uMan.CreateLocationPopup(Location);

    public void OnPointerExit() => 
        uMan.DestroyLocationPopup();
}
