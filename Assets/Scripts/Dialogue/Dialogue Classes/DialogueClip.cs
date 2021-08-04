using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Clip", menuName = "Dialogue/Dialogue Clip")]
public class DialogueClip : ScriptableObject
{
    [Header("JOURNAL NOTES")]
    [Tooltip("The journal notes the clip triggers")]
    [SerializeField] protected List<JournalNote> journalNotes;
    public List<JournalNote> JournalNotes { get => journalNotes; }
    [Header("DIALOGUE PROMPT")]
    [TextArea]
    [Tooltip("The NPC dialogue that prompts a response")]
    [SerializeField] protected string dialoguePrompt;
    [Header("DIALOGUE RESPONSES")]
    [SerializeField] private DialogueResponse dialogueResponse1;
    [SerializeField] private DialogueResponse dialogueResponse2;
    [SerializeField] private DialogueResponse dialogueResponse3;
    [SerializeField] private NPCHero newEngagedHero;
    [SerializeField] private Card newCard;

    public string DialoguePrompt { get => dialoguePrompt; }
    public DialogueResponse DialogueResponse1 { get => dialogueResponse1; }
    public DialogueResponse DialogueResponse2 { get => dialogueResponse2; }
    public DialogueResponse DialogueResponse3 { get => dialogueResponse3; }
    public NPCHero NewEngagedHero { get => newEngagedHero; }
    public Card NewCard { get => newCard; }

    public virtual void LoadDialogueClip(DialogueClip dc)
    {
        static DialogueResponse ResponseInstance(DialogueResponse dr)
        {
            DialogueResponse newDR = ScriptableObject.CreateInstance<DialogueResponse>();
            newDR.LoadResponse(dr);
            return newDR;
        }
        journalNotes = new List<JournalNote>();
        foreach (JournalNote jn in dc.JournalNotes)
            journalNotes.Add(jn);
        dialogueResponse1 = ResponseInstance(dc.DialogueResponse1);
        dialogueResponse2 = ResponseInstance(dc.DialogueResponse2);
        dialogueResponse3 = ResponseInstance(dc.DialogueResponse3);
        dialoguePrompt = dc.DialoguePrompt;
        newEngagedHero = dc.NewEngagedHero;
        newCard = dc.NewCard;
    }
}