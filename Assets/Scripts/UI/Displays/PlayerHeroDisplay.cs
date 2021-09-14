using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHeroDisplay : HeroDisplay
{
    public PlayerHero PlayerHero { get => HeroScript as PlayerHero; }

    public Sprite PowerImage
    {
        set => powerImage.GetComponent<Image>().sprite = value;
    }
    [SerializeField] private GameObject powerImage;

    public string PlayerActions
    {
        set => playerActions.GetComponent<TextMeshProUGUI>().SetText(value);
    }
    [SerializeField] private GameObject playerActions;

    public override void DisplayHero()
    {
        base.DisplayHero();
        PowerImage = PlayerHero.HeroPower.PowerSprite;
    }
}
