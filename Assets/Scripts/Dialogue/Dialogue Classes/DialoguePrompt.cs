using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Prompt", menuName = "Dialogue/Dialogue Prompt")]
public class DialoguePrompt : DialogueClip
{
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
    [Header("HIDE NPC")]
    [SerializeField] private bool hideNPC;
    [Header("DIALOGUE REWARDS")]
    [SerializeField] private Card newCard;
    [SerializeField] private bool newAllyCard;
    [SerializeField] private bool newExecutionCard;
    [SerializeField] [Range(0, 5)] private int aetherCells;
    [SerializeField] private NewLocation[] newLocations;

    public string DialoguePromptText { get => dialoguePromptText; }
    public DialogueResponse DialogueResponse1 { get => dialogueResponse1; }
    public DialogueResponse DialogueResponse2 { get => dialogueResponse2; }
    public DialogueResponse DialogueResponse3 { get => dialogueResponse3; }

    public NPCHero NewEngagedHero { get => newEngagedHero; }
    public bool HideNPC { get => hideNPC; }
    public Card NewCard { get => newCard; }
    public bool NewAllyCard { get => newAllyCard; }
    public bool NewExecutionCard { get => newExecutionCard; }
    public int AetherCells { get => aetherCells; }
    public NewLocation[] NewLocations { get => newLocations; }

    public override void LoadDialogueClip(DialogueClip dc)
    {
        base.LoadDialogueClip(dc);
        DialoguePrompt dp = dc as DialoguePrompt;
        dialogueResponse1 = new DialogueResponse();
        dialogueResponse2 = new DialogueResponse();
        dialogueResponse3 = new DialogueResponse();
        dialogueResponse1.LoadResponse(dp.DialogueResponse1);
        dialogueResponse2.LoadResponse(dp.DialogueResponse2);
        dialogueResponse3.LoadResponse(dp.DialogueResponse3);
        dialoguePromptText = dp.DialoguePromptText;
        newEngagedHero = dp.NewEngagedHero;
        hideNPC = dp.HideNPC;
        newCard = dp.NewCard;
        newAllyCard = dp.NewAllyCard;
        newExecutionCard = dp.NewExecutionCard;
        aetherCells = dp.AetherCells;
        newLocations = (NewLocation[])dp.NewLocations.Clone();
    }
}
