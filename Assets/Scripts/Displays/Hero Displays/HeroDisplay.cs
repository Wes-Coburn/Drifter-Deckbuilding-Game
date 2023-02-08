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

    [SerializeField] private GameObject heroBase;
    [SerializeField] private GameObject heroFrame;
    [SerializeField] private GameObject heroStats;
    [SerializeField] private GameObject heroPortrait;
    [SerializeField] private GameObject heroName;
    [SerializeField] private GameObject heroHealth;
    [SerializeField] private GameObject woundedIcon;
    [SerializeField] private GameObject heroHealthSlider;
    [SerializeField] private GameObject heroEnergy;
    [SerializeField] private GameObject[] energyBars = new GameObject[10];
    [SerializeField] private GameObject heroPower;
    [SerializeField] private GameObject powerImage;

    public GameObject HeroBase { get => heroBase; }
    public GameObject HeroFrame { get => heroFrame; }
    public GameObject HeroStats { get => heroStats; }
    public GameObject HeroNameObject { get => heroName; }
    public GameObject HeroHealthObject { get => heroHealth; }
    public GameObject HeroEnergyObject { get => heroEnergy; }
    public GameObject HeroPower { get => heroPower; }

    public Sprite HeroPortrait
    {
        set
        {
            heroPortrait.GetComponent<Image>().sprite = value;

            if (this is PlayerHeroDisplay)
            {
                UIManager.Instance.GetPortraitPosition
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
            int health = value;
            if (health < 0) health = 0;
            heroHealth.GetComponent<TextMeshProUGUI>().SetText(health.ToString());
            heroHealthSlider.GetComponent<Slider>().value = health;
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
                if (i < currentEnergy) color = Color.white;
                else color = Color.gray;
                energyBar.GetComponent<Image>().color = color;
            }
            else energyBar.SetActive(false);
        }
    }

    public virtual void DisplayHero()
    {
        HeroPortrait = heroScript.HeroPortrait;
        HeroName = heroScript.HeroName;

        if (HeroScript.HeroPower == null) heroPower.SetActive(false);
        else powerImage.GetComponent<Image>().sprite = HeroScript.HeroPower.PowerSprite;
    }
}
