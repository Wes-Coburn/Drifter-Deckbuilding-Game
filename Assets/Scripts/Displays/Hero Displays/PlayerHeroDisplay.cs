using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeroDisplay : HeroDisplay
{
    [SerializeField] private GameObject powerCost;
    [SerializeField] private GameObject powerReadyIcon;

    [SerializeField] private GameObject heroUltimate;
    [SerializeField] private GameObject ultimateImage;
    [SerializeField] private GameObject ultimateCost;
    [SerializeField] private GameObject ultimateReadyIcon;
    [SerializeField] private GameObject ultimateButton;
    [SerializeField] private GameObject ultimateProgressValue;
    [SerializeField] private GameObject[] ultimateProgressBars = new GameObject[GameManager.HERO_ULTMATE_GOAL];

    public GameObject PowerReadyIcon { get => powerReadyIcon; }
    public GameObject HeroUltimate { get => heroUltimate; }
    public GameObject UltimateReadyIcon { get => ultimateReadyIcon; }
    public GameObject UltimateButton { get => ultimateButton; }
    public int UltimateProgressValue
    {
        set
        {
            int goal = GameManager.HERO_ULTMATE_GOAL;
            if (value > goal) value = goal;

            //ultimateProgressValue.GetComponent<TextMeshProUGUI>().SetText
            //(value + "/" + GameManager.HERO_ULTMATE_GOAL);

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
        powerCost.GetComponent<TextMeshProUGUI>().SetText(HeroScript.CurrentHeroPower.PowerCost.ToString());
        ultimateImage.GetComponent<Image>().sprite = (HeroScript as PlayerHero).CurrentHeroUltimate.PowerSprite;

        int cost = Managers.P_MAN.GetUltimateCost(out Color ultimateColor);
        var ultimateGui = ultimateCost.GetComponent<TextMeshProUGUI>();
        ultimateGui.SetText(cost.ToString());
        ultimateGui.color = ultimateColor;
    }
}
