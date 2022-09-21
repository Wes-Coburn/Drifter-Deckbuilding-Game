using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHeroDisplay : HeroDisplay
{
    [SerializeField] private GameObject heroPower;
    [SerializeField] private GameObject powerImage;
    [SerializeField] private GameObject powerCost;
    [SerializeField] private GameObject powerReadyIcon;

    [SerializeField] private GameObject heroUltimate;
    [SerializeField] private GameObject ultimateImage;
    [SerializeField] private GameObject ultimateCost;
    [SerializeField] private GameObject ultimateReadyIcon;
    [SerializeField] private GameObject ultimateButton;
    [SerializeField] private GameObject ultimateProgressValue;
    [SerializeField] private GameObject[] ultimateProgressBars = new GameObject[GameManager.HERO_ULTMATE_GOAL];

    public PlayerHero PlayerHero { get => HeroScript as PlayerHero; }
    public GameObject HeroPower { get => heroPower; }
    public GameObject PowerReadyIcon { get => powerReadyIcon; }
    public GameObject HeroUltimate { get => heroUltimate; }
    public GameObject UltimateReadyIcon { get => ultimateReadyIcon; }
    public GameObject UltimateButton { get => ultimateButton; }
    public int UltimateProgressValue
    {
        set
        {
            ultimateProgressValue.GetComponent<TextMeshProUGUI>().SetText
                (value + "/" + GameManager.HERO_ULTMATE_GOAL);

            bool setBar = true;
            for (int i = 0; i < ultimateProgressBars.Length; i++)
            {
                if (setBar && i >= value) setBar = false;
                ultimateProgressBars[i].SetActive(setBar);
            }
        }
    }

    public override void DisplayHero()
    {
        base.DisplayHero();
        powerImage.GetComponent<Image>().sprite = PlayerHero.HeroPower.PowerSprite;
        powerCost.GetComponent<TextMeshProUGUI>().SetText(PlayerHero.HeroPower.PowerCost.ToString());
        ultimateImage.GetComponent<Image>().sprite = PlayerHero.HeroUltimate.PowerSprite;

        int cost = PlayerManager.Instance.GetUltimateCost(out Color ultimateColor);
        TextMeshProUGUI ultimateGui = ultimateCost.GetComponent<TextMeshProUGUI>();
        ultimateGui.SetText(cost.ToString());
        ultimateGui.color = ultimateColor;
    }
}
