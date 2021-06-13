using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Encounter", menuName = "Encounter")]
public class Encounter : ScriptableObject
{
    [SerializeField]
    [TextArea]
    private string developerNotes;

    //[Space] public DialogueClip SourceClip;

    [Space]
    public List<HeroCard> EnemyHeroes;

    public List<HeroCard> EnemyReinforcements;

    [Tooltip("The rate at which reinforcements appear")]
    [Range(1, 5)]
    public int ReinforcementInterval;

    [Space]
    public List<SkillCard> EnemySkills;
}
