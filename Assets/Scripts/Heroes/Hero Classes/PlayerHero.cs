using UnityEngine;

[CreateAssetMenu(fileName = "New Player Hero", menuName = "Heroes/Player/Player Hero")]
public class PlayerHero : Hero
{
    [Header("ALT HERO POWERS")]
    [SerializeField] private HeroPower[] altHeroPowers;
    [Header("HERO ULTIMATE")]
    [SerializeField] private HeroPower heroUltimate;
    [Header("ALT HERO ULTIMATES")]
    [SerializeField] private HeroPower[] altHeroUltimates;
    [Header("HERO BACKSTORY")]
    [SerializeField, TextArea] private string heroBackstory;

    public HeroPower[] AltHeroPowers { get => altHeroPowers; }
    public HeroPower HeroUltimate { get => heroUltimate; }
    public HeroPower CurrentHeroUltimate { get; set; }
    public HeroPower[] AltHeroUltimates { get => altHeroUltimates; }
    public string HeroBackstory { get => heroBackstory; }

    public override void LoadHero(Hero hero)
    {
        base.LoadHero(hero);
        var ph = hero as PlayerHero;
        altHeroPowers = (HeroPower[])ph.AltHeroPowers?.Clone();
        heroUltimate = ph.HeroUltimate;
        altHeroUltimates = (HeroPower[])ph.AltHeroUltimates?.Clone();
        heroBackstory = ph.HeroBackstory;
        CurrentHeroUltimate = heroUltimate;
    }
}
