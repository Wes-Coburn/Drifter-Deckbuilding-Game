using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HomeBaseSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject playerHero;
    [SerializeField] private GameObject heroPower;
    [SerializeField] private GameObject heroName;
    [SerializeField] private GameObject heroImage;
    [SerializeField] private GameObject heroDescription;
    [SerializeField] private GameObject heroPowerDescription;
    [SerializeField] private GameObject heroPowerImage;
    [SerializeField] private GameObject selectedAugment;
    [SerializeField] private GameObject augmentName;
    [SerializeField] private GameObject augmentImage;
    [SerializeField] private GameObject augmentDescription;
    [SerializeField] private GameObject augmentCost;
    [SerializeField] private GameObject confirmSpendPopup;

    private PlayerManager pMan;
    private UIManager uMan;
    private List<HeroAugment> accessibleAugments;
    private int currentAugment;

    public HeroPower HeroPower
    {
        set
        {
            heroPower.GetComponent<PowerZoom>().LoadedPower = value;
        }
    }
    private string HeroName
    {
        set
        {
            heroName.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    private Sprite HeroSprite
    {
        set
        {
            heroImage.GetComponent<Image>().sprite = value;
            uMan.GetPortraitPosition(pMan.PlayerHero.HeroName, out Vector2 position,
                out Vector2 scale, SceneLoader.Scene.HeroSelectScene);
            heroImage.transform.localPosition = position;
            heroImage.transform.localScale = scale;
        }
    }
    private string HeroDescription
    {
        set
        {
            heroDescription.GetComponent<TextMeshProUGUI>().SetText(value);
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
    private string AugmentName
    {
        set
        {
            augmentName.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    private Sprite AugmentSprite
    {
        set
        {
            augmentImage.GetComponent<Image>().sprite = value;
        }
    }
    private string AugmentDescription
    {
        set
        {
            augmentDescription.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    private string AugmentCost
    {
        set
        {
            augmentCost.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    private void Start()
    {
        pMan = PlayerManager.Instance;
        uMan = UIManager.Instance;
        PlayerHero ph = pMan.PlayerHero;
        accessibleAugments = new List<HeroAugment>();
        HeroAugment[] allAugments = Resources.LoadAll<HeroAugment>("Hero Augments");
        foreach (HeroAugment aug in allAugments)
            accessibleAugments.Add(aug);

        playerHero.SetActive(true);
        selectedAugment.SetActive(false);
        HeroPower = ph.HeroPower;
        HeroName = ph.HeroName;
        HeroSprite = ph.HeroPortrait;
        HeroDescription = ph.HeroDescription;
        HeroPowerDescription = "<b><u>" + ph.HeroPower.PowerName +
            ":</b></u> " + ph.HeroPower.PowerDescription;
        HeroPowerSprite = ph.HeroPower.PowerSprite;
    }

    private void DisplayCurrentAugment()
    {
        HeroAugment aug = accessibleAugments[currentAugment];
        AugmentName = aug.AugmentName;
        AugmentDescription = aug.AugmentDescription;
        AugmentCost = GameManager.ACQUIRE_AUGMENT_COST.ToString();
        AugmentSprite = aug.AugmentImage;
    }

    public void NextAugmentButton_OnClick()
    {
        if (currentAugment > accessibleAugments.Count - 2) 
            currentAugment = 0;
        else currentAugment++;
        DisplayCurrentAugment();
    }

    public void PreviousAugmentButton_OnClick()
    {
        if (currentAugment < 1) 
            currentAugment = accessibleAugments.Count - 1;
        else currentAugment--;
        DisplayCurrentAugment();
    }

    public void LearnSkillButton_OnClick()
    {
        CloseAugmentsButton_OnClick();
        uMan.CreateCardPagePopup(CardPageDisplay.CardPageType.LearnSkill);
    }

    public void AcquireAugmentButton_OnClick()
    {
        if (accessibleAugments.Count < 1)
        {
            NoAugmentsAvailable();
            return;
        }

        playerHero.SetActive(false);
        selectedAugment.SetActive(true);
        currentAugment = 0;

        foreach (HeroAugment aug in pMan.HeroAugments)
        {
            int index = accessibleAugments.FindIndex(x => x.AugmentName == aug.AugmentName);
            if (index != -1) accessibleAugments.RemoveAt(index);
        }

        if (accessibleAugments.Count < 1)
        {
            NoAugmentsAvailable();
            return;
        }

        DisplayCurrentAugment();

        void NoAugmentsAvailable()
        {
            CloseAugmentsButton_OnClick();
            uMan.CreateFleetingInfoPopup("No augments available!", true);
        }
    }

    public void SelectAugmentButton_OnClick()
    {
        if (pMan.HeroAugments.Count >= 5)
            uMan.CreateFleetingInfoPopup("You can't have more than 5 augments!", true);
        else if (pMan.AetherCells < GameManager.ACQUIRE_AUGMENT_COST)
            uMan.InsufficientAetherPopup();
        else uMan.CreateAcquireAugmentPopup(accessibleAugments[currentAugment]);
    }

    public void CloseAugmentsButton_OnClick()
    {
        selectedAugment.SetActive(false);
        playerHero.SetActive(true);
    }

    public void RemoveCardButton_OnClick()
    {
        CloseAugmentsButton_OnClick();
        uMan.CreateCardPagePopup(CardPageDisplay.CardPageType.RemoveCard);
    }

    public void BackButton_OnClick() => 
        SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
}
