using UnityEngine;
using TMPro;

public class PlayerHeroDisplay : HeroDisplay
{
    public PlayerHero PlayerHero { get => HeroScript as PlayerHero; }

    public Sprite PowerImage
    {
        set => powerImage.GetComponent<SpriteRenderer>().sprite = value;
    }
    [SerializeField] private GameObject powerImage;

    public string PlayerActions
    {
        set => playerActions.GetComponent<TextMeshPro>().SetText(value);
    }
    [SerializeField] private GameObject playerActions;

    public override void DisplayHero()
    {
        base.DisplayHero();
        PowerImage = PlayerHero.HeroPower.PowerSprite;
    }
}
