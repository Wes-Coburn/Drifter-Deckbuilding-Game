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
    [SerializeField] private GameObject undoButton;
    [SerializeField] private GameObject closePopupButton;

    private Location location;
    public Location Location
    {
        set
        {
            location = value;
            LocationName = location.LocationFullName;
            LocationDescription = location.LocationDescription;
            ObjectivesDescription = Managers.CA_MAN.FilterUnitTypes(location.CurrentObjective);
            WorldMapPosition = new Vector2(0, 0); // CHANGE?

            difficultyLevel.GetComponentInChildren<Slider>().SetValueWithoutNotify(Managers.CO_MAN.DifficultyLevel);
            SetDifficultyLevel(Managers.CO_MAN.DifficultyLevel);

            if (!location.IsRecurring)
            {
                LocationHours = "";
                return;
            }

            string hours = "Hours Open: ";
            List<string> openHours = new();
            if (!location.IsClosed_Hour1) openHours.Add("Morning");
            if (!location.IsClosed_Hour2) openHours.Add("Day");
            if (!location.IsClosed_Hour3) openHours.Add("Evening");

            if (openHours.Count == 3) hours += $" {TextFilter.Clrz_grn("ALL", false)}";
            else
            {
                for (int i = 0; i < openHours.Count; i++)
                {
                    if (i != 0) hours += ", ";
                    hours += TextFilter.Clrz_grn(openHours[i], false);
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

    private void Awake() => undoButton.SetActive(false);

    private void SetDifficultyLevel(int difficulty)
    {
        Color newColor;
        if (difficulty > 2) newColor = Color.red;
        else if (difficulty > 1) newColor = Color.yellow;
        else newColor = Color.green;

        int surgeValue = Managers.G_MAN.GetSurgeDelay(difficulty);
        int aetherValue = Managers.G_MAN.GetAdditionalRewardAether(difficulty);
        int energyValue = Managers.G_MAN.GetAdditionalEnergy(difficulty);
        int reputationValue = Managers.G_MAN.GetBonusReputation(difficulty);

        string text =
            $"-> Enemies surge every {TextFilter.Clrz_red(surgeValue + "")} turns." +
            $"\n-> Enemies start with +{TextFilter.Clrz_red(energyValue + "")} energy." +
            $"\n\n-> +{TextFilter.Clrz_grn(aetherValue + "")} aether.";

        if (location.IsRandomEncounter) text += $"\n-> +{TextFilter.Clrz_grn(reputationValue + "")} reputation."; // Only works if random encounters are the only locations with reputation rewards

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
        if (Managers.G_MAN.VisitedLocations.FindIndex(x => x == location.LocationName) == -1)
        {
            Managers.G_MAN.VisitedLocations.Add(location.LocationName);
            if (!location.IsRecurring) Managers.G_MAN.NextHour(!location.IsRandomEncounter);
        }
        if (location.IsHomeBase)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.HomeBaseScene);
            return;
        }

        Managers.G_MAN.CurrentLocation = Managers.G_MAN.GetActiveLocation(location);
        if (location.IsRecruitment || location.IsActionShop || location.IsShop || location.IsCloning) { }
        else Managers.G_MAN.ActiveLocations.Remove(Managers.G_MAN.CurrentLocation);
        Managers.D_MAN.EngagedHero = Managers.G_MAN.GetActiveNPC(Managers.G_MAN.CurrentLocation.CurrentNPC);

        if (location.IsCombatOnly) SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
        else SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene, true);
    }

    public void CancelButton_OnClick()
    {
        Managers.U_MAN.DestroyTravelPopup();
        Managers.U_MAN.DestroyLocationPopup();
        Destroy(gameObject);
    }
}
