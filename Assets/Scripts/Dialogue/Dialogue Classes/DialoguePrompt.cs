using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Prompt", menuName = "Dialogue/Dialogue Prompt")]
public class DialoguePrompt : DialogueClip
{
    [Header("JOURNAL NOTES")]
    [SerializeField] protected JournalNote[] journalNotes;
    [Space]
    [Header("DIALOGUE PROMPT")]
    [TextArea]
    [SerializeField] protected string dialoguePromptText;
    [Space]
    [Header("DIALOGUE RESPONSES")]
    [SerializeField] private DialogueResponse dialogueResponse1;
    [SerializeField] private DialogueResponse dialogueResponse2;
    [SerializeField] private DialogueResponse dialogueResponse3;
    [Header("NEW ENGAGED HERO")]
    [SerializeField] private NPCHero newEngagedHero;
    [Header("DIALOGUE REWARDS")]
    [SerializeField] private Card newCard;
    [SerializeField] [Range(0, 5)] private int aetherCells;
    [SerializeField] private NewLocation[] newLocations;

    public JournalNote[] JournalNotes { get => journalNotes; }
    public string DialoguePromptText { get => dialoguePromptText; }
    public DialogueResponse DialogueResponse1 { get => dialogueResponse1; }
    public DialogueResponse DialogueResponse2 { get => dialogueResponse2; }
    public DialogueResponse DialogueResponse3 { get => dialogueResponse3; }
    public NPCHero NewEngagedHero { get => newEngagedHero; }
    public Card NewCard { get => newCard; }
    public int AetherCells { get => aetherCells; }
    public NewLocation[] NewLocations { get => newLocations; }

    public override void LoadDialogueClip(DialogueClip dc)
    {
        base.LoadDialogueClip(dc);
        
        DialoguePrompt dp = dc as DialoguePrompt;
        /*
        journalNotes = new List<JournalNote>();
        foreach (JournalNote jn in dp.JournalNotes)
            journalNotes.Add(jn);
        */
        dp.JournalNotes.CopyTo(journalNotes, 0);
        dialogueResponse1.LoadResponse(dp.DialogueResponse1);
        dialogueResponse2.LoadResponse(dp.DialogueResponse2);
        dialogueResponse3.LoadResponse(dp.DialogueResponse3);

        dialoguePromptText = dp.DialoguePromptText;
        newEngagedHero = dp.NewEngagedHero;
        newCard = dp.NewCard;
        aetherCells = dp.AetherCells;
        newLocations = dp.NewLocations;
    }
}
