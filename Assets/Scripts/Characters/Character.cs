using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class Character : ScriptableObject
{
    [Header("BASIC INFO")]
    [SerializeField] private string characterName;
    public string CharacterName
    {
        get => characterName;
        private set => characterName = value;
    }

    [SerializeField] [Range(1, 100)]
    private int age;
    public int Age
    {
        get => age;
        private set => age = value;
    }

    [SerializeField] private string gender;
    public string Gender
    {
        get => gender;
        private set => gender = value;
    }

    [Header("OCCUPATION")]
    [SerializeField] private string occupation;
    public string Occupation
    {
        get => occupation;
        private set => occupation = value;
    }

    [Header("BIOGRAPHY")]
    [TextArea]
    [SerializeField] private string biography;
    public string Biography
    {
        get => biography;
        private set => biography = value;
    }

    [Header("RESPECT SCORE")]
    [SerializeField] [Range(-10, 10)]
    private int respectScore;
    public int RespectScore
    {
        get => respectScore;
        set => respectScore = value;
    }

    /* CHARACTER_NOTES */
    private List<string> characterNotes;
    public List<string> CharacterNotes
    {
        get => characterNotes;
        private set => characterNotes = value;
    }
    

    [Header("FIRST DIALOGUE CLIP")]
    [SerializeField] private DialogueClip firstDialogueClip;
    public DialogueClip FirstDialogueClip
    {
        get => firstDialogueClip;
        private set => firstDialogueClip = value;
    }
    public DialogueClip NextDialogueClip { get; set; }

    private void Awake() => NextDialogueClip = FirstDialogueClip;
    public void LoadCharacter(Character character)
    {
        CharacterName = character.CharacterName;
        Age = character.Age;
        Gender = character.Gender;
        Occupation = character.Occupation;
        Biography = character.Biography;
        RespectScore = character.RespectScore;
        CharacterNotes = new List<string>();
        foreach (string note in character.CharacterNotes) CharacterNotes.Add(note);
        FirstDialogueClip = character.FirstDialogueClip;
        NextDialogueClip = FirstDialogueClip;
    }
}
