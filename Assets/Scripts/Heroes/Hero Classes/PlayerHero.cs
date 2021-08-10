using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Hero", menuName = "Heroes/Player/Player Hero")]
public class PlayerHero : Hero
{
    [Header("HERO POWER")]
    public HeroPower HeroPower;
    [Header("HERO SKILLS")]
    public List<SkillCard> HeroSkills;

    public override void LoadHero(Hero hero)
    {
        base.LoadHero(hero);
        PlayerHero ph = hero as PlayerHero;
        HeroPower = ph.HeroPower;
        HeroSkills = ph.HeroSkills;
    }
}
