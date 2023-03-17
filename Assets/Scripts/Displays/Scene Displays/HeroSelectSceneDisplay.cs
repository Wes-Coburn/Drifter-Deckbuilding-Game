using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectSceneDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject heroPortrait, heroName, heroDescription, heroBackstory,
        heroPower, heroPowerImage, heroPowerDescription, heroPowerCost,
        heroUltimate, heroUltimateImage, heroUltimateDescription, heroUltimateCost,
        lockedHeroIcon, lockedPowerIcon, lockedUltimateIcon, confirmHeroButton;

    private List<PlayerHero> playerHeroes;
    private int currentHero, currentPower, currentUltimate;

    private const string LOCK_TEXT = "<b>???</b>";
    private const string LOCK_TEXT_SHORT = "<b>?</b>";

    private PlayerHero LoadedHero => playerHeroes[currentHero];
    private HeroPower LoadedPower => currentPower == 0 ? LoadedHero.CurrentHeroPower : LoadedHero.AltHeroPowers[currentPower - 1];
    private HeroPower LoadedUltimate => currentUltimate == 0 ? LoadedHero.CurrentHeroUltimate : LoadedHero.AltHeroUltimates[currentUltimate - 1];
    private bool HeroIsUnlocked => Managers.G_MAN.UnlockedHeroes.Contains(LoadedHero.HeroName);
    private bool PowerIsUnlocked => currentPower == 0 || Managers.G_MAN.UnlockedPowers.Contains(LoadedPower.PowerName);
    private bool UltimateIsUnlocked => currentUltimate == 0 || Managers.G_MAN.UnlockedPowers.Contains(LoadedUltimate.PowerName);

    private void Start()
    {
        var allHeroes = Resources.LoadAll<PlayerHero>("Heroes");
        var startingHeroes = new List<PlayerHero>();
        var unlockedHeroes = new List<PlayerHero>();
        var lockedHeroes = new List<PlayerHero>();

        foreach (var hero in allHeroes)
        {
            var newHero = ScriptableObject.CreateInstance<PlayerHero>();
            newHero.LoadHero(hero);

            if (Managers.G_MAN.StartingHeroes.Contains(newHero.HeroName)) startingHeroes.Add(newHero);
            else if (Managers.G_MAN.UnlockedHeroes.Contains(newHero.HeroName)) unlockedHeroes.Add(newHero);
            else lockedHeroes.Add(newHero);
        }

        playerHeroes = startingHeroes.Concat(unlockedHeroes).Concat(lockedHeroes).ToList();
        heroBackstory.SetActive(false);
        currentPower = 0;
        currentUltimate = 0;
    }

    private void LoadSelection()
    {
        var newPH = ScriptableObject.CreateInstance<PlayerHero>();
        newPH.LoadHero(LoadedHero);
        newPH.CurrentHeroPower = LoadedPower;
        newPH.CurrentHeroUltimate = LoadedUltimate;
        Managers.P_MAN.HeroScript = newPH;

        foreach (var uc in Managers.CA_MAN.PlayerStartUnits)
            for (int i = 0; i < GameManager.PLAYER_START_UNITS; i++)
                Managers.CA_MAN.AddCard(uc, Managers.P_MAN);
    }

    public void ConfirmButton_OnClick()
    {
        LoadSelection();
        SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
    }

    public void BackButton_OnClick() => Managers.G_MAN.EndGame();

    public void SelectHero_RightArrow_OnClick()
    {
        if (++currentHero > playerHeroes.Count - 1) currentHero = 0;
        DisplaySelectedHero();
    }

    public void SelectHero_LeftArrow_OnClick()
    {
        if (--currentHero < 0) currentHero = playerHeroes.Count - 1;
        DisplaySelectedHero();
    }

    public void SelectedHero_OnClick()
    {
        if (lockedHeroIcon.activeInHierarchy) return;
        heroBackstory.SetActive(!heroBackstory.activeSelf);
    }

    public void SelectPower_RightArrow_OnClick()
    {
        if (++currentPower > LoadedHero.AltHeroPowers.Length) currentPower = 0;
        DisplaySelectedPower();
    }

    public void SelectPower_LeftArrow_OnClick()
    {
        if (--currentPower < 0) currentPower = LoadedHero.AltHeroPowers.Length;
        DisplaySelectedPower();
    }

    public void SelectUltimate_RightArrow_OnClick()
    {
        if (++currentUltimate > LoadedHero.AltHeroUltimates.Length) currentUltimate = 0;
        DisplaySelectedUltimate();
    }

    public void SelectUltimate_LeftArrow_OnClick()
    {
        if (--currentUltimate < 0) currentUltimate = LoadedHero.AltHeroUltimates.Length;
        DisplaySelectedUltimate();
    }

    public void DisplaySelectedHero()
    {
        bool unlocked = HeroIsUnlocked;
        lockedHeroIcon.SetActive(!unlocked);

        foreach (var s in LoadedHero.CurrentHeroPower.PowerSounds)
            Managers.AU_MAN.StartStopSound(null, s);

        heroBackstory.GetComponentInChildren<TextMeshProUGUI>().SetText(unlocked ? LoadedHero.HeroBackstory : LOCK_TEXT);
        heroName.GetComponent<TextMeshProUGUI>().SetText(unlocked ? LoadedHero.HeroName : LOCK_TEXT);
        heroPortrait.GetComponent<Image>().sprite = LoadedHero.HeroPortrait; // Keep image revealed
        Managers.U_MAN.GetPortraitPosition(LoadedHero.HeroName,
            out Vector2 position, out Vector2 scale, SceneLoader.Scene.HeroSelectScene);
        heroPortrait.transform.localPosition = position;
        heroPortrait.transform.localScale = scale;
        heroDescription.GetComponent<TextMeshProUGUI>().SetText(unlocked ? LoadedHero.HeroDescription : LOCK_TEXT);

        currentPower = 0;
        currentUltimate = 0;
        DisplaySelectedPower();
        DisplaySelectedUltimate();
    }
    
    public void DisplaySelectedPower()
    {
        bool unlocked = HeroIsUnlocked && PowerIsUnlocked;
        lockedPowerIcon.SetActive(!unlocked);

        bool canContinue = unlocked && UltimateIsUnlocked;
        confirmHeroButton.SetActive(canContinue);

        heroPower.GetComponent<PowerZoom>().LoadedPower = unlocked ? LoadedPower : null;
        heroPowerCost.GetComponent<TextMeshProUGUI>().SetText(unlocked ? LoadedPower.PowerCost.ToString() : LOCK_TEXT_SHORT);
        heroPowerImage.GetComponent<Image>().sprite = LoadedPower.PowerSprite; // Keep image revealed

        string description;
        if (unlocked) description = $"<b><u>{LoadedPower.PowerName}:</b></u> {Managers.CA_MAN.FilterKeywords(LoadedPower.PowerDescription)}";
        else description = LOCK_TEXT;
        heroPowerDescription.GetComponent<TextMeshProUGUI>().SetText(description);
    }

    public void DisplaySelectedUltimate()
    {
        bool unlocked = HeroIsUnlocked && UltimateIsUnlocked;
        lockedUltimateIcon.SetActive(!unlocked);

        bool canContinue = unlocked && PowerIsUnlocked;
        confirmHeroButton.SetActive(canContinue);

        heroUltimate.GetComponent<PowerZoom>().LoadedPower = unlocked ? LoadedUltimate : null;
        heroUltimateCost.GetComponent<TextMeshProUGUI>().SetText(unlocked ? LoadedUltimate.PowerCost.ToString() : LOCK_TEXT_SHORT);
        heroUltimateImage.GetComponent<Image>().sprite = LoadedUltimate.PowerSprite; // Keep image revealed

        string description;
        if (unlocked) description = $"<b><u>{LoadedUltimate.PowerName} (Ultimate):</b></u> {Managers.CA_MAN.FilterKeywords(LoadedUltimate.PowerDescription)}";
        else description = LOCK_TEXT;
        heroUltimateDescription.GetComponent<TextMeshProUGUI>().SetText(description);
    }
}
