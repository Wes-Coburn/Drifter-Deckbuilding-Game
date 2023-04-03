using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeBaseSceneDisplay : MonoBehaviour
{
    [Header("HERO INFO"), SerializeField] private GameObject heroName;
    [SerializeField] private GameObject heroDescription, heroBackstory, heroImage;

    [Header("HERO POWER"), SerializeField] private GameObject heroPower;
    [SerializeField] private GameObject heroPowerCost, heroPowerDescription, heroPowerImage;

    [Header("HERO ULTIMATE"), SerializeField] private GameObject heroUltimate;
    [SerializeField] private GameObject heroUltimateCost, heroUltimateDescription, heroUltimateImage;

    [Header("CLAIM REWARD BUTTON"), SerializeField] private GameObject claimRewardButton;

    private string HeroName
    {
        set
        {
            heroName.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    private string HeroDescription
    {
        set
        {
            heroDescription.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    private string HeroBackstory
    {
        set
        {
            heroBackstory.GetComponentInChildren<TextMeshProUGUI>().SetText(value);
        }
    }
    private Sprite HeroSprite
    {
        set
        {
            heroImage.GetComponent<Image>().sprite = value;
            Managers.U_MAN.GetPortraitPosition(Managers.P_MAN.HeroScript.HeroName, out Vector2 position, out Vector2 scale);
            heroImage.transform.localPosition = position;
            heroImage.transform.localScale = scale;
        }
    }
    private HeroPower HeroPower
    {
        set
        {
            heroPower.GetComponent<PowerZoom>().LoadedPower = value;
        }
    }
    private int HeroPowerCost
    {
        set
        {
            heroPowerCost.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
        }
    }
    private string HeroPowerDescription
    {
        set
        {
            heroPowerDescription.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    private Sprite HeroPowerSprite
    {
        set
        {
            heroPowerImage.GetComponent<Image>().sprite = value;
        }
    }
    private HeroPower HeroUltimate
    {
        set
        {
            heroUltimate.GetComponent<PowerZoom>().LoadedPower = value;
        }
    }
    private int HeroUltimateCost
    {
        set
        {
            heroUltimateCost.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
        }
    }
    private string HeroUltimateDescription
    {
        set
        {
            heroUltimateDescription.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    private Sprite HeroUltimateSprite
    {
        set
        {
            heroUltimateImage.GetComponent<Image>().sprite = value;
        }
    }

    private PlayerHero playerHero;

    private List<HeroPower> unlockedPowers;
    private List<HeroPower> unlockedUltimates;

    private int currentPower;
    private int currentUltimate;

    public GameObject ClaimRewardButton => claimRewardButton;

    private void Start()
    {
        playerHero = Managers.P_MAN.HeroScript as PlayerHero;
        HeroName = playerHero.HeroName;
        HeroDescription = playerHero.HeroDescription;
        HeroBackstory = playerHero.HeroBackstory;
        HeroSprite = playerHero.HeroPortrait;
        heroBackstory.SetActive(false);

        unlockedPowers = new() { playerHero.HeroPower };
        AddUnlockedPowers(playerHero.AltHeroPowers, unlockedPowers);

        unlockedUltimates = new() { playerHero.HeroUltimate };
        AddUnlockedPowers(playerHero.AltHeroUltimates, unlockedUltimates);

        currentPower = unlockedPowers.FindIndex(x => x.PowerName == playerHero.CurrentHeroPower.PowerName);
        currentUltimate = unlockedUltimates.FindIndex(x => x.PowerName == playerHero.CurrentHeroUltimate.PowerName);

        DisplaySelectedPower();
        DisplaySelectedUltimate();

        void AddUnlockedPowers(HeroPower[] powers, List<HeroPower> powerList)
        {
            if (powers == null)
            {
                Debug.Log("POWERS IS NULL!");
                return;
            }

            foreach (var power in powers)
                AddUnlockedPower(power, powerList);
        }
        void AddUnlockedPower(HeroPower power, List<HeroPower> powerList)
        {
            int powerIndex = Managers.G_MAN.UnlockedPowers.FindIndex(x => x == power.PowerName);
            if (powerIndex != -1) powerList.Add(power);
        }
    }

    private void DisplaySelectedPower()
    {
        playerHero.CurrentHeroPower = unlockedPowers[currentPower];
        HeroPower = playerHero.CurrentHeroPower;
        HeroPowerCost = playerHero.CurrentHeroPower.PowerCost;
        HeroPowerDescription = $"<b><u>{playerHero.CurrentHeroPower.PowerName}:</b></u> " +
            $"{Managers.CA_MAN.FilterKeywords(playerHero.CurrentHeroPower.PowerDescription)}";
        HeroPowerSprite = playerHero.CurrentHeroPower.PowerSprite;
    }

    private void DisplaySelectedUltimate()
    {
        playerHero.CurrentHeroUltimate = unlockedUltimates[currentUltimate];
        HeroUltimate = playerHero.CurrentHeroUltimate;
        HeroUltimateCost = playerHero.CurrentHeroUltimate.PowerCost;
        HeroUltimateDescription = $"<b><u>{playerHero.CurrentHeroUltimate.PowerName} (Ultimate):</b></u> " +
            $"{Managers.CA_MAN.FilterKeywords(playerHero.CurrentHeroUltimate.PowerDescription)}";
        HeroUltimateSprite = playerHero.CurrentHeroUltimate.PowerSprite;
    }

    public void ShowInfoButton_OnClick() =>
        heroBackstory.SetActive(!heroBackstory.activeSelf);

    public void RemoveCardButton_OnClick(bool playSound = true) =>
        Managers.U_MAN.CreateCardPage(CardPageDisplay.CardPageType.RemoveCard, playSound);

    public void RemoveItemButton_OnClick() => Managers.U_MAN.CreateItemPagePopup(true);

    public void ClaimRewardButton_OnClick()
    {
        Managers.U_MAN.DestroyTooltipPopup();
        claimRewardButton.SetActive(false);
        Managers.U_MAN.CreateChooseRewardPopup();
    }

    public void BackButton_OnClick()
    {
        if (claimRewardButton.activeSelf)
        {
            Managers.U_MAN.CreateFleetingInfoPopup("Claim Your Reward!");
            return;
        }
        SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
    }

    public void SelectPower_RightArrow_OnClick()
    {
        if (++currentPower > unlockedPowers.Count - 1) currentPower = 0;
        DisplaySelectedPower();
    }

    public void SelectPower_LeftArrow_OnClick()
    {
        if (--currentPower < 0) currentPower = unlockedPowers.Count - 1;
        DisplaySelectedPower();
    }

    public void SelectUltimate_RightArrow_OnClick()
    {
        if (++currentUltimate > unlockedUltimates.Count - 1) currentUltimate = 0;
        DisplaySelectedUltimate();
    }

    public void SelectUltimate_LeftArrow_OnClick()
    {
        if (--currentUltimate < 0) currentUltimate = unlockedUltimates.Count - 1;
        DisplaySelectedUltimate();
    }
}
