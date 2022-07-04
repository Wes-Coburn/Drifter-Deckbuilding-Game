using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocationPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject locationName;
    [SerializeField] private GameObject locationDescription;
    [SerializeField] private GameObject locationHours;
    [SerializeField] private GameObject objectivesDescription;
    [SerializeField] private GameObject travelButtons;

    private GameManager gMan;
    private UIManager uMan;
    private DialogueManager dMan;
    private CardManager caMan;

    private Location location;
    public Location Location
    {
        set
        {
            location = value;
            LocationName = location.LocationFullName;
            LocationDescription = location.LocationDescription;
            ObjectivesDescription = caMan.FilterCardTypes(location.CurrentObjective);
            WorldMapPosition = new Vector2(0, 0); // FOR TESTING ONLY?

            string hours = "Hours Closed: ";
            List<string> closedHours = new List<string>();
            if (location.IsClosed_Hour1) closedHours.Add("Morning");
            if (location.IsClosed_Hour2) closedHours.Add("Day");
            if (location.IsClosed_Hour3) closedHours.Add("Evening");
            for (int i = 0; i < closedHours.Count; i++)
            {
                if (i != 0) hours += ", ";
                hours += "<color=\"red\">" + closedHours[i] + "</color>";
            }
            if (closedHours.Count < 1) hours += " None.";
            else if (gMan.VisitedLocations.FindIndex(x => x == location.LocationName) == -1)
            {
                hours = "<s>" + hours + "</s>";
            }
            LocationHours = hours;
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

    private string LocationHours
    {
        set
        {
            locationHours.GetComponent<TextMeshProUGUI>().SetText(value);
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
    
    private void Awake()
    {
        gMan = GameManager.Instance;
        uMan = UIManager.Instance;
        dMan = DialogueManager.Instance;
        caMan = CardManager.Instance;
    }

    public void TravelButton_OnClick()
    {
        if (gMan.VisitedLocations.FindIndex(x => x == location.LocationName) == -1)
        {
            gMan.VisitedLocations.Add(location.LocationName);

            if (!location.IsHomeBase && !location.IsAugmenter)
                gMan.NextHour(!location.IsRandomEncounter);
        }
        if (location.IsHomeBase)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.HomeBaseScene);
            return;
        }

        gMan.CurrentLocation = gMan.GetActiveLocation(location);
        if (location.IsRecruitment) gMan.CurrentLocation.CurrentObjective = "Recruit a Unit.";
        else if (location.IsShop) gMan.CurrentLocation.CurrentObjective = "Buy an Item.";
        else if (location.IsCloning) gMan.CurrentLocation.CurrentObjective = "Clone a Unit.";
        else gMan.ActiveLocations.Remove(gMan.CurrentLocation);
        dMan.EngagedHero = gMan.GetActiveNPC(gMan.CurrentLocation.CurrentNPC);

        if (location.IsCombatOnly) SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
        else SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene, true);
    }

    public void CancelButton_OnClick() =>
        uMan.DestroyTravelPopup();
}
