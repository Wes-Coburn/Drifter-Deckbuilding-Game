using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Hero", menuName = "Heroes/Player Hero")]
public class PlayerHero : Hero
{
    [Header("HERO POWER")]
    public HeroPower HeroPower;
    [Header("HERO SKILLS")]
    public List<SkillCard> HeroSkills;
}
