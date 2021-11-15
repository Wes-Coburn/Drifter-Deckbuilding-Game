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
    [SerializeField] private HeroAugment[] availableAugments;

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
        accessibleAugments = new List<HeroAugment>();
        PlayerHero ph = pMan.PlayerHero;
        List<HeroAugment> redundancies = new List<HeroAugment>();
        foreach (HeroAugment aug in availableAugments)
        {
            if (pMan.GetAugment(aug.AugmentName)) 
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
        if (selectedAugment.activeSelf == true)
        {
            CloseAugmentsButton_OnClick();
            return;
        }
        playerHero.SetActive(false);
        selectedAugment.SetActive(true);
        currentAugment = 0;

        if (accessibleAugments.Count < 1) 
            Debug.LogWarning("NO ACCESSIBLE AUGMENTS!");
        else
        {
            List<HeroAugment> redundancies = new List<HeroAugment>();
            foreach (HeroAugment aug1 in accessibleAugments)
            {
                if (pMan.GetAugment(aug1.AugmentName))
                    redundancies.Add(aug1);
            }
            foreach (HeroAugment aug2 in redundancies)
                accessibleAugments.Remove(aug2);

            DisplayCurrentAugment();
        }
    }

    public void SelectAugmentButton_OnClick()
    {
        if (pMan.AetherCells < GameManager.ACQUIRE_AUGMENT_COST)
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
