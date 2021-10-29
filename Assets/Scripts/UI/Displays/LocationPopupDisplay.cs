using UnityEngine;
using TMPro;

public class LocationPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject locationName;
    [SerializeField] private GameObject locationDescription;
    [SerializeField] private GameObject objectivesDescription;
    [SerializeField] private GameObject travelButtons;

    private GameManager gMan;
    private UIManager uMan;
    private DialogueManager dMan;

    private Location location;
    public Location Location
    {
        set
        {
            location = value;
            LocationName = location.LocationFullName;
            LocationDescription = location.LocationDescription;
            ObjectivesDescription = location.CurrentObjective; // TESTING
            WorldMapPosition = new Vector2(0, 0); // FOR TESTING ONLY?
        }
    }

    private string LocationName
    {
        set
        {
            locationName.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    private string LocationDescription
    {
        set
        {
            locationDescription.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    private string ObjectivesDescription
    {
        set
        {
            objectivesDescription.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    private Vector2 WorldMapPosition
    {
        set
        {
            transform.position = value;
        }
    }

    public GameObject TravelButtons { get => travelButtons; }

    private void Start()
    {
        gMan = GameManager.Instance;
        uMan = UIManager.Instance;
        dMan = DialogueManager.Instance;
    }

    public void TravelButton_OnClick()
    {
        if (location.IsHomeBase)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.HomeBaseScene);
            return;
        }
        gMan.CurrentLocation = gMan.GetActiveLocation(location);
        if (!location.IsRecruitment) // TESTING
            gMan.ActiveLocations.Remove(gMan.CurrentLocation);
        else gMan.CurrentLocation.CurrentObjective = "Recruit More Followers."; // TESTING
        gMan.ExitWorldMap();
        dMan.EngagedHero = gMan.GetActiveNPC(gMan.CurrentLocation.CurrentNPC);
        SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
    }

    public void CancelButton_OnClick() =>
        uMan.DestroyTravelPopup();
}
