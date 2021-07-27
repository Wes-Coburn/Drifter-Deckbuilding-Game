using UnityEngine;

public class PlayerHeroDisplay : HeroDisplay
{
    public PlayerHero PlayerHero { get => HeroScript as PlayerHero; }

    public Sprite PowerImage
    {
        set => powerImage.GetComponent<SpriteRenderer>().sprite = value;
    }
    [SerializeField] private GameObject powerImage;

    public override void DisplayHero()
    {
        base.DisplayHero();
        PowerImage = PlayerHero.HeroPower.PowerSprite;
    }
}
