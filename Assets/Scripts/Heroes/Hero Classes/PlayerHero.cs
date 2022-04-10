using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Hero", menuName = "Heroes/Player/Player Hero")]
public class PlayerHero : Hero
{
    [Header("HERO POWER")]
    [SerializeField] private HeroPower heroPower;
    [Header("HERO ULTIMATE")]
    [SerializeField] private HeroPower heroUltimate;
    [Header("HERO SKILLS")]
    [SerializeField] private List<SkillCard> heroSkills;
    [Header("HERO BACKSTORY")]
    [SerializeField] [TextArea] private string heroBackstory;

    public List<SkillCard> HeroSkills { get => heroSkills; }
    public HeroPower HeroPower { get => heroPower; }
    public HeroPower HeroUltimate { get => heroUltimate; }
    public string HeroBackstory { get => heroBackstory; }

    public override void LoadHero(Hero hero)
    {
        base.LoadHero(hero);
        PlayerHero ph = hero as PlayerHero;
        heroPower = ph.HeroPower;
        heroUltimate = ph.HeroUltimate;
        heroSkills = ph.HeroSkills;
        heroBackstory = ph.HeroBackstory;
    }
}
