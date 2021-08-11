using UnityEngine;

public abstract class DialogueClip : ScriptableObject
{
    [Tooltip("For development only, no effect on gameplay")]
    [TextArea]
    [SerializeField] private string developerNotes;

    public string DeveloperNotes { get => developerNotes; }

    public virtual void LoadDialogueClip(DialogueClip dc)
    {
        developerNotes = dc.DeveloperNotes;
    }
}