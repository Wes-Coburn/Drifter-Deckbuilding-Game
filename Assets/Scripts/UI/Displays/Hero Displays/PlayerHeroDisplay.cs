using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHeroDisplay : HeroDisplay
{
    [SerializeField] private GameObject powerImage;
    [SerializeField] private GameObject playerActions;
    [SerializeField] private GameObject powerCost;
    [SerializeField] private GameObject powerUsedIcon;

    public PlayerHero PlayerHero { get => HeroScript as PlayerHero; }
    public GameObject PowerUsedIcon { get => powerUsedIcon; }
    public string PlayerActions
    {
        set => playerActions.GetComponent<TextMeshProUGUI>().SetText(value);
    }

    public override void DisplayHero()
    {
        base.DisplayHero();
        powerImage.GetComponent<Image>().sprite = PlayerHero.HeroPower.PowerSprite;
        powerCost.GetComponent<TextMeshProUGUI>().SetText(PlayerHero.HeroPower.PowerCost.ToString()); // TESTING
    }
}
