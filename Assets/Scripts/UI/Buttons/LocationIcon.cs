using UnityEngine;
using TMPro;

public class LocationIcon : MonoBehaviour
{
    [SerializeField] private GameObject locationPopupPrefab;
    [SerializeField] private GameObject locationName;

    private GameObject locationPopup;
    private GameManager gMan;
    private UIManager uMan;
    private DialogueManager dMan;

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

    private void Start()
    {
        gMan = GameManager.Instance;
        uMan = UIManager.Instance;
        dMan = DialogueManager.Instance;
    }

    public void OnClick()
    {
        gMan.CurrentLocation = Location;
        if (Location.CurrentNPC == null)
        {
            Debug.LogError("CURRENT NPC IS NULL!");
            return;
        }
        dMan.EngagedHero = Location.CurrentNPC;
        gMan.ExitWorldMap(); // TESTING
        SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
    }

    public void OnPointerEnter()
    {
        locationPopup = Instantiate(locationPopupPrefab, uMan.CurrentCanvas.transform);
        LocationPopupDisplay lpd = locationPopup.GetComponent<LocationPopupDisplay>();
        lpd.LocationName = Location.LocationFullName;
        lpd.LocationDescription = Location.LocationDescription;
        lpd.ObjectivesDescription = Location.FirstObjective;
        lpd.WorldMapPosition = new Vector2(0, 0); // FOR TESTING ONLY
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
