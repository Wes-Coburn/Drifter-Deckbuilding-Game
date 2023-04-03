using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject locationName, locationDescription, locationHours,
        objectivesDescription, travelButtons, difficultyLevel, difficultyValue,
        difficultyText, undoButton, closePopupButton;

    private Location location;
    public Location Location
    {
        set
        {
            location = value;
            LocationName = location.LocationFullName;
            LocationDescription = location.LocationDescription;
            ObjectivesDescription = Managers.CA_MAN.FilterUnitTypes(location.CurrentObjective);

            Vector2 mapPos = location.WorldMapPosition;
            Vector2 popPos = new();

            float buffer = 100;
            if (mapPos.x > buffer) popPos.x = 350;
            else if (mapPos.x < -buffer) popPos.x = difficultyLevel.activeInHierarchy ? -100 : -350;

            if (mapPos.y > buffer) popPos.y = 100;
            else if (mapPos.y < -buffer) popPos.y = -200;

            WorldMapPosition = popPos;

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
            transform.localPosition = value;
        }
    }

    public GameObject TravelButtons { get => travelButtons; }
    public GameObject DifficultyLevel { get => difficultyLevel; }
    public GameObject ClosePopupButton { get => closePopupButton; }

    private void Awake() => undoButton.SetActive(false);

    private void SetDifficultyLevel(int difficulty)
    {
        Color newColor;
        switch (difficulty)
        {
            case 1:
                newColor = Color.green;
                break;
            case 2:
                newColor = Color.blue;
                break;
            case 3:
                newColor = Color.red;
                break;
            default:
                Debug.LogError("INVALID DIFFICULTY!");
                return;
        }

        int surgeValue = Managers.G_MAN.GetSurgeDelay(difficulty);
        int energyValue = Managers.G_MAN.GetEnemyStartingEnergy(difficulty);
        int aetherValue = Managers.G_MAN.GetAdditionalRewardAether(difficulty);
        int reputationValue = Managers.G_MAN.GetBonusReputation(difficulty);

        string text =
            $"-> Enemies surge every {TextFilter.Clrz_red(surgeValue + "")} turns." +
            $"\n-> Enemies start with +{TextFilter.Clrz_red(energyValue + "")} energy." +
            $"\n\n-> +{TextFilter.Clrz_grn(aetherValue + "")} aether.";

        // ::: WATCH :::
        // Only works if random encounters are the ONLY locations with reputation rewards
        if (location.IsRandomEncounter) text += $"\n-> +{TextFilter.Clrz_grn(reputationValue + "")} reputation.";
        var diffTmpro = difficultyValue.GetComponent<TextMeshProUGUI>();
        diffTmpro.SetText(difficulty + "");
        diffTmpro.color = newColor;
        difficultyLevel.GetComponentInChildren<Slider>().handleRect.GetComponent<Image>().color = newColor;
        difficultyText.GetComponent<TextMeshProUGUI>().SetText(text);
    }

    public void DifficultySlider_OnSlide(float level)
    {
        int intLevel = (int)level;
        Managers.CO_MAN.DifficultyLevel = intLevel;
        SetDifficultyLevel(intLevel);
    }

    public void TravelButton_OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;

        Managers.AN_MAN.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress);

        if (Managers.G_MAN.VisitedLocations.FindIndex(x => x == location.LocationName) == -1)
        {
            if (!location.IsRecurring) Managers.G_MAN.NextHour(!location.IsRandomEncounter);
            if ((location.IsRecurring && !location.IsAugmenter && !location.IsHealer) || location.IsRandomEncounter)
                Managers.G_MAN.VisitedLocations.Add(location.LocationName);
        }

        if (location.IsHomeBase)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.HomeBaseScene);
            return;
        }

        Managers.G_MAN.CurrentLocation = Managers.G_MAN.GetActiveLocation(location);
        if (!location.IsRecurring) Managers.G_MAN.ActiveLocations.Remove(Managers.G_MAN.CurrentLocation);
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
