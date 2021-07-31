using UnityEngine;

[CreateAssetMenu(fileName = "New Journal Note", menuName = "Journal/Note")]
public class JournalNote : ScriptableObject
{
    [Header("SOURCE CLIP")]
    [Tooltip("The dialogue clip that triggered this note")]
    [SerializeField]
    private DialogueClip sourceClip;
    public DialogueClip SourceClip
    {
        get => sourceClip;
        private set => sourceClip = value;
    }

    [Header("IS NEW LOCATION")]
    [Tooltip("The note is a location")]
    public bool IsNewLocation;

    [Header("IS NEW CHARACTER")]
    [Tooltip("The note is a character")]
    public bool IsNewCharacter;

    [Header("NOTE TEXT")]
    [Tooltip("The name of the location or character")]
    [TextArea]
    public string NoteText;

    [Header("NOTE IMAGE")]
    [Tooltip("The image for the note")]
    public Sprite NoteImage;
}
