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
    [SerializeField] private HeroAugment[] availableAugments;

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

    private void Start()
    {
        accessibleAugments = new List<HeroAugment>();
        PlayerHero ph = PlayerManager.Instance.PlayerHero;
        currentAugment = 0;
        List<HeroAugment> redundancies = new List<HeroAugment>();
        foreach (HeroAugment aug in availableAugments)
        {
            if (PlayerManager.Instance.GetAugment(aug.AugmentName)) 
                redundancies.Add(aug);
        }
        foreach (HeroAugment aug in availableAugments) 
            if (!redundancies.Contains(aug)) accessibleAugments.Add(aug);

        playerHero.SetActive(true);
        selectedAugment.SetActive(false);
        HeroPower = ph.HeroPower;
        HeroName = ph.HeroName;
        HeroSprite = ph.HeroPortrait;
        HeroDescription = ph.HeroDescription;
        HeroPowerDescription = ph.HeroPower.PowerDescription;
        HeroPowerSprite = ph.HeroPower.PowerSprite;
    }

    private void DisplayCurrentAugment()
    {
        HeroAugment aug = accessibleAugments[currentAugment];
        AugmentName = aug.AugmentName;
        AugmentDescription = aug.AugmentDescription;
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
        UIManager.Instance.CreateCardPagePopup();
    }

    public void AcquireAugmentButton_OnClick()
    {
        playerHero.SetActive(false);
        selectedAugment.SetActive(true);
        if (accessibleAugments.Count > 0)
            DisplayCurrentAugment();
        else Debug.LogWarning("NO ACCESSIBLE AUGMENTS!");
    }

    public void SelectAugmentButton_OnClick()
    {
        if (PlayerManager.Instance.AetherCells < 3)
            UIManager.Instance.CreateCenteredInfoPopup("Not enough aether!");
        else PlayerManager.Instance.HeroAugments.Add
                (accessibleAugments[currentAugment]); // TESTING
    }

    public void CloseAugmentsButton_OnClick()
    {
        selectedAugment.SetActive(false);
        playerHero.SetActive(true);
    }

    public void RemoveCardButton_OnClick()
    {
        Debug.LogWarning("BLANK!");
    }

    public void BackButton_OnClick()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
    }
}
