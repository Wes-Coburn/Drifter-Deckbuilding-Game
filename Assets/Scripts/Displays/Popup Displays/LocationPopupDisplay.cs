using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject locationName;
    [SerializeField] private GameObject locationDescription;
    [SerializeField] private GameObject locationHours;
    [SerializeField] private GameObject objectivesDescription;
    [SerializeField] private GameObject travelButtons;
    [SerializeField] private GameObject difficultyLevel;
    [SerializeField] private GameObject difficultyValue;
    [SerializeField] private GameObject difficultyText;
    [SerializeField] private GameObject closePopupButton;

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
            ObjectivesDescription = caMan.FilterUnitTypes(location.CurrentObjective);
            WorldMapPosition = new Vector2(0, 0); // CHANGE?

            if (!location.IsRecurring)
            {
                LocationHours = "";
                return;
            }

            string hours = "Hours Open: ";
            List<string> openHours = new List<string>();
            if (!location.IsClosed_Hour1) openHours.Add("Morning");
            if (!location.IsClosed_Hour2) openHours.Add("Day");
            if (!location.IsClosed_Hour3) openHours.Add("Evening");

            if (openHours.Count == 3) hours += " <color=\"green\">ALL</color>";
            else
            {
                for (int i = 0; i < openHours.Count; i++)
                {
                    if (i != 0) hours += ", ";
                    hours += $"<color=\"green\">{openHours[i]}</color>";
                }
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
    public GameObject DifficultyLevel { get => difficultyLevel; }
    public GameObject ClosePopupButton { get => closePopupButton; }
    
    private void Awake()
    {
        gMan = GameManager.Instance;
        uMan = UIManager.Instance;
        dMan = DialogueManager.Instance;
        caMan = CardManager.Instance;

        int level = CombatManager.Instance.DifficultyLevel;
        difficultyLevel.GetComponentInChildren<Slider>().SetValueWithoutNotify(level);
        SetDifficultyLevel(level);
    }

    private void SetDifficultyLevel(int difficulty)
    {
        Color newColor;
        if (difficulty > 2) newColor = Color.red;
        else if (difficulty > 1) newColor = Color.yellow;
        else newColor = Color.green;

        int surgeValue = gMan.GetSurgeDelay(difficulty);
        int energyValue = GameManager.BOSS_BONUS_ENERGY + difficulty - 1;
        int aetherValue = GameManager.ADDITIONAL_AETHER_REWARD * (difficulty - 1);

        string text =
            $"-> Enemies surge every <color=\"red\"><b>{surgeValue}</b></color> turns." +
            $"\n-> Enemy bosses start with <color=\"red\"><b>+{energyValue}</b></color> energy." +
            $"\n\n-> <color=\"green\"><b>+{aetherValue}</b></color> aether";

        difficultyValue.GetComponent<TextMeshProUGUI>().SetText(difficulty + "");
        difficultyValue.GetComponent<TextMeshProUGUI>().color = newColor;
        difficultyLevel.GetComponentInChildren<Slider>
            ().handleRect.GetComponent<Image>().color = newColor;
        difficultyText.GetComponent<TextMeshProUGUI>().SetText(text);
    }
    public void DifficultySlider_OnSlide(float level)
    {
        int intLevel = (int)level;
        CombatManager.Instance.DifficultyLevel = intLevel;
        SetDifficultyLevel(intLevel);
    }

    public void TravelButton_OnClick()
    {
        if (gMan.VisitedLocations.FindIndex(x => x == location.LocationName) == -1)
        {
            gMan.VisitedLocations.Add(location.LocationName);

            if (!location.IsRecurring) gMan.NextHour(!location.IsRandomEncounter);
        }

        if (location.IsHomeBase)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.HomeBaseScene);
            return;
        }

        gMan.CurrentLocation = gMan.GetActiveLocation(location);

        if (location.IsRecruitment || location.IsActionShop|| location.IsShop || location.IsCloning) { }
        else gMan.ActiveLocations.Remove(gMan.CurrentLocation);
        dMan.EngagedHero = gMan.GetActiveNPC(gMan.CurrentLocation.CurrentNPC);

        if (location.IsCombatOnly) SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
        else SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene, true);
    }

    public void CancelButton_OnClick()
    {
        uMan.DestroyTravelPopup();
        uMan.DestroyLocationPopup();
    }
}
