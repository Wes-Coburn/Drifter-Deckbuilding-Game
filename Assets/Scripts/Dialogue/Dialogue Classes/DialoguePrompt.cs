using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Prompt", menuName = "Dialogue/Dialogue Prompt")]
public class DialoguePrompt : DialogueClip
{
    [Header("JOURNAL NOTES")]
    [Tooltip("The journal notes the clip triggers")]
    [SerializeField] protected List<JournalNote> journalNotes;
    [Space]
    [Header("DIALOGUE PROMPT")]
    [TextArea]
    [Tooltip("The NPC dialogue that prompts a response")]
    [SerializeField] protected string dialoguePromptText;
    [Space]
    [Header("DIALOGUE RESPONSES")]
    [SerializeField] private DialogueResponse dialogueResponse1;
    [SerializeField] private DialogueResponse dialogueResponse2;
    [SerializeField] private DialogueResponse dialogueResponse3;
    [SerializeField] private NPCHero newEngagedHero;
    [SerializeField] private Card newCard;
    [SerializeField] [Range(0, 5)] private int aetherCells;

    public List<JournalNote> JournalNotes { get => journalNotes; }
    public string DialoguePromptText { get => dialoguePromptText; }
    public DialogueResponse DialogueResponse1 { get => dialogueResponse1; }
    public DialogueResponse DialogueResponse2 { get => dialogueResponse2; }
    public DialogueResponse DialogueResponse3 { get => dialogueResponse3; }
    public NPCHero NewEngagedHero { get => newEngagedHero; }
    public Card NewCard { get => newCard; }
    public int AetherCells { get => aetherCells; }

    public override void LoadDialogueClip(DialogueClip dc)
    {
        base.LoadDialogueClip(dc);
        
        DialoguePrompt dp = dc as DialoguePrompt;
        journalNotes = new List<JournalNote>();
        foreach (JournalNote jn in dp.JournalNotes)
            journalNotes.Add(jn);

        dialogueResponse1.LoadResponse(dp.dialogueResponse1);
        dialogueResponse2.LoadResponse(dp.dialogueResponse2);
        dialogueResponse3.LoadResponse(dp.dialogueResponse3);

        dialoguePromptText = dp.DialoguePromptText;
        newEngagedHero = dp.NewEngagedHero;
        newCard = dp.NewCard;
    }
}
