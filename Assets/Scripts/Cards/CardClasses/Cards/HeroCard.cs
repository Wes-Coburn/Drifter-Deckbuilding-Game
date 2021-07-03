using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hero Card", menuName = "Cards/Hero")]
public class HeroCard : Card
{
    [Header("HERO CARD DETAILS")]
    public string XPCondition;
    public int AttackScore;
    public int DefenseScore;
    
    [Header("HERO CARD ABILITIES")]
    public List<CardAbility> Level1Abilities;
    public List<CardAbility> Level2Abiliites;
}
