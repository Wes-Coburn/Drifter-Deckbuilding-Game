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
    [SerializeField] private List<SkillCard> heroStartSkills;
    [SerializeField] private List<SkillCard> heroMoreSkills;
    [Header("HERO BACKSTORY")]
    [TextArea] [SerializeField] private string heroBackstory;

    public List<SkillCard> HeroStartSkills { get => heroStartSkills; }
    public List<SkillCard> HeroMoreSkills { get => heroMoreSkills; }
    public HeroPower HeroPower { get => heroPower; }
    public HeroPower HeroUltimate { get => heroUltimate; }
    public string HeroBackstory { get => heroBackstory; }


    public override void LoadHero(Hero hero)
    {
        base.LoadHero(hero);
        PlayerHero ph = hero as PlayerHero;
        heroPower = ph.HeroPower;
        heroUltimate = ph.HeroUltimate;
        heroStartSkills = ph.HeroStartSkills;
        heroMoreSkills = ph.HeroMoreSkills;
        heroBackstory = ph.HeroBackstory;
    }
}
