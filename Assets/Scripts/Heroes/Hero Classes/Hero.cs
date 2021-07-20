using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hero", menuName = "Heroes/Hero")]
public class Hero : ScriptableObject
{
    [Header("HERO NAME")]
    public string HeroName;
    [Header("HERO PORTRAIT")]
    public Sprite HeroPortrait;
    [Header("HERO DESCRIPTION")]
    [TextArea]
    public string HeroDescription;
    [Header("HERO POWER")]
    public HeroPower HeroPower;
    [Header("HERO SKILLS")]
    public List<SkillCard> HeroSkills;
    [Header("HERO UNITS")]
    [Tooltip("Only enemy heroes normally have units")]
    public List<UnitCard> HeroUnits;
    [Header("REINFORCEMENT SCHEDULE")]
    public List<int> ReinforcementSchedule;
}
