using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Clip", menuName = "Dialogue/Dialogue Clip")]
public class DialogueClip : ScriptableObject
{
    [Header("DEVELOPER NOTES")]
    [TextArea]
    [Tooltip("Notes For Development Only")]
    [SerializeField] protected string developerNotes;
    public string DeveloperNotes { get => developerNotes; }

    [Header("JOURNAL NOTES")]
    [Tooltip("The journal notes the clip triggers")]
    [SerializeField] protected List<JournalNote> journalNotes;
    public List<JournalNote> JournalNotes { get => journalNotes; }

    [Header("DIALOGUE PROMPT")]
    [TextArea]
    [Tooltip("The NPC dialogue that prompts a response")]
    [SerializeField] protected string dialoguePrompt;
    public string DialoguePrompt { get => dialoguePrompt; }

    [TextArea]
    [Tooltip("The initial response given in the dialogue pool that leads to this clip")]
    [SerializeField] private string dialoguePoolResponse;
    [SerializeField] [Range(-5, 5)] private int dialoguePoolResponse_Respect;
    [SerializeField] protected bool dialoguePoolResponse_isExit;
    [SerializeField] protected bool dialoguePoolResponse_isCombatStart;
    public string DialoguePoolResponse { get => dialoguePoolResponse; }
    public int DialoguePoolResponse_Respect { get => dialoguePoolResponse_Respect; }
    public bool DialoguePoolResponse_isExit { get => dialoguePoolResponse_isExit; }
    public bool DialoguePoolResponse_isCombatStart { get => dialoguePoolResponse_isCombatStart; }

    [Header("DIALOGUE RESPONSES")]
    [SerializeField] private DialogueResponse dialogueResponse1;
    [SerializeField] private DialogueResponse dialogueResponse2;
    [SerializeField] private DialogueResponse dialogueResponse3;
    public DialogueResponse DialogueResponse1 { get => dialogueResponse1; }
    public DialogueResponse DialogueResponse2 { get => dialogueResponse2; }
    public DialogueResponse DialogueResponse3 { get => dialogueResponse3; }

    public virtual void LoadDialogueClip(DialogueClip dc)
    {
        DialogueResponse ResponseInstance(DialogueResponse dr)
        {
            DialogueResponse newDR = ScriptableObject.CreateInstance<DialogueResponse>();
            newDR.LoadResponse(dr);
            return newDR;
        }
        developerNotes = dc.DeveloperNotes;
        journalNotes = new List<JournalNote>();
        foreach (JournalNote jn in dc.journalNotes)
            journalNotes.Add(jn);
        dialogueResponse1 = ResponseInstance(dc.dialogueResponse1);
        dialogueResponse2 = ResponseInstance(dc.dialogueResponse2);
        dialogueResponse3 = ResponseInstance(dc.dialogueResponse3);
        dialoguePrompt = dc.dialoguePrompt;
    }
}