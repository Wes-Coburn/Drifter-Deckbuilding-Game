using UnityEngine;
using TMPro;

public class LocationIcon : MonoBehaviour
{
    [SerializeField] private GameObject locationName;
    
    private UIManager uMan;

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

    public void OnClick() => 
        uMan.CreateTravelPopup(Location);

    public void OnPointerEnter() => 
        uMan.CreateLocationPopup(Location);

    public void OnPointerExit() => 
        uMan.DestroyLocationPopup();
}
