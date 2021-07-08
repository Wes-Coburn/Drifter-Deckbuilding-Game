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
    public List<FollowerCard> EnemyHeroes;
    [Space]
    public List<SkillCard> EnemySkills;
}
