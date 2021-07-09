using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Card", menuName = "Cards/Skill")]
public class SkillCard : ActionCard
{
    public Hero Hero; // The hero whose skill this is
}
