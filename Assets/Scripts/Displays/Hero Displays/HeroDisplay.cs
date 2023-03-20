using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class HeroDisplay : MonoBehaviour
{
    /* HERO_SCRIPTABLE_OBJECT */
    private Hero heroScript;
    public Hero HeroScript
    {
        get => heroScript;
        set
        {
            heroScript = value;
            DisplayHero();
        }
    }

    [SerializeField] private GameObject heroBase, heroFrame, heroStats, heroPortrait,
        heroName, heroHealth, woundedIcon, heroHealthSlider, heroEnergy, heroPower, powerImage;
    [SerializeField] private GameObject[] energyBars = new GameObject[10];

    public GameObject HeroBase { get => heroBase; }
    public GameObject HeroFrame { get => heroFrame; }
    public GameObject HeroStats { get => heroStats; }
    public GameObject HeroNameObject { get => heroName; }
    public GameObject HeroHealthObject { get => heroHealth; }
    public GameObject HeroEnergyObject { get => heroEnergy; }
    public GameObject HeroPower { get => heroPower; }
    public GameObject HeroPowerImage { get => powerImage; }

    public Sprite HeroPortrait
    {
        set
        {
            heroPortrait.GetComponent<Image>().sprite = value;

            if (this is PlayerHeroDisplay)
            {
                Managers.U_MAN.GetPortraitPosition
                    (HeroScript.HeroName, out Vector2 position, out Vector2 scale, SceneLoader.Scene.CombatScene);
                heroPortrait.transform.localPosition = position;
                heroPortrait.transform.localScale = scale;
            }
        }
    }

    public string HeroName
    {
        set
        {
            heroName.GetComponentInChildren<TextMeshProUGUI>().SetText(value);
        }
    }

    public int HeroHealth
    {
        set
        {
            heroHealth.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
            heroHealthSlider.GetComponent<Slider>().value = value;
        }
    }

    public bool IsWounded
    {
        set
        {
            woundedIcon.SetActive(value);
        }
    }

    private int HeroEnergy
    {
        set
        {
            heroEnergy.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
        }
    }

    public void SetHeroEnergy(int currentEnergy, int maxEnergy)
    {
        HeroEnergy = currentEnergy;

        for (int i = 0; i < energyBars.Length; i++)
        {
            GameObject energyBar = energyBars[i];
            if (i < maxEnergy || i < currentEnergy)
            {
                Color color;
                energyBar.SetActive(true);
                color = i < currentEnergy ? Color.white : Color.gray;
                energyBar.GetComponent<Image>().color = color;
            }
            else energyBar.SetActive(false);
        }
    }

    public virtual void DisplayHero()
    {
        HeroPortrait = heroScript.HeroPortrait;
        HeroName = heroScript.HeroName;

        if (HeroScript.CurrentHeroPower == null) heroPower.SetActive(false);
        else powerImage.GetComponent<Image>().sprite = HeroScript.CurrentHeroPower.PowerSprite;

        heroHealthSlider.GetComponent<Slider>().maxValue = HeroScript is PlayerHero ?
            Managers.P_MAN.MaxHealth : Managers.EN_MAN.MaxHealth;
    }
}
