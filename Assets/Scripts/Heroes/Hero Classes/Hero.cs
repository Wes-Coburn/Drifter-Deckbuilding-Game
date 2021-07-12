using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hero", menuName = "Heroes/Hero")]
public class Hero : ScriptableObject
{
    [Header("HERO NAME")]
    public string HeroName;
    [Header("HERO DESCRIPTION")]
    [TextArea]
    public string HeroDescription;
    [Header("HERO ABILITY")]
    public HeroPower HeroAbility;
    [Header("HERO SKILLS")]
    public List<SkillCard> HeroSkills;
    [Header("HERO FOLLOWERS")]
    [Tooltip("Only enemy heroes normally have followers")]
    public List<FollowerCard> HeroFollowers;
}
