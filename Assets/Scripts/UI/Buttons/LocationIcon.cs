using UnityEngine;
using TMPro;

public class LocationIcon : MonoBehaviour
{
    [SerializeField] private GameObject locationName;
    [SerializeField] private GameObject locationPopupPrefab;
    private GameObject locationPopup;
    private UIManager uMan;
    
    public string LocationName // TESTING
    {
        set
        {
            locationName.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    public Location Location { get; set; }

    private void Start()
    {
        uMan = UIManager.Instance;
    }

    public void OnClick()
    {
        DialogueManager.Instance.EngagedHero = Location.CurrentNPC; // TESTING
        GameManager.Instance.ExitWorldMap(); // TESTING
        SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
    }

    public void OnPointerEnter()
    {
        locationPopup = Instantiate(locationPopupPrefab, uMan.CurrentCanvas.transform);
        LocationPopupDisplay lpd = locationPopup.GetComponent<LocationPopupDisplay>();
        lpd.LocationName = Location.LocationFullName;
        lpd.LocationDescription = Location.LocationDescription;
        lpd.WorldMapPosition = Location.WorldMapPosition;
        lpd.ObjectivesDescription = Location.FirstObjective;
    }

    public void OnPointerExit()
    {
        if (locationPopup != null)
        {
            Destroy(locationPopup);
            locationPopup = null;
        }
    }
}
