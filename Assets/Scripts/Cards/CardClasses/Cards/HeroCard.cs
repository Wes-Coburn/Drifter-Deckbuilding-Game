using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hero Card", menuName = "Cards/Hero")]
public class HeroCard : Card
{
    public string HeroLore;
    public string XPCondition;
    public int AttackScore;
    public int DefenseScore;
    public List<CardAbility> Level1Abilities;
    public List<CardAbility> Level2Abiliites;
}
