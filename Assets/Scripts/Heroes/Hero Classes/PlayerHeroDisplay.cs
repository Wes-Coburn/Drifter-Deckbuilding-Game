using UnityEngine;

public class PlayerHeroDisplay : HeroDisplay
{
    public Sprite PowerImage
    {
        set => powerImage.GetComponent<SpriteRenderer>().sprite = value;
    }
    [SerializeField] private GameObject powerImage;

    public override void DisplayHero()
    {
        base.DisplayHero();
        PowerImage = HeroScript.HeroPower.PowerSprite;
    }
}
