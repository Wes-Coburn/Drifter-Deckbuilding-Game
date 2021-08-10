using UnityEngine;

public abstract class DialogueClip : ScriptableObject
{
    [SerializeField] private string developerNotes;
    public string DeveloperNotes { get => developerNotes; }

    public virtual void LoadDialogueClip(DialogueClip dc)
    {
        developerNotes = dc.DeveloperNotes;
    }
}