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
    [SerializeField] [Range(0, 5)] private int aetherCells;
    [Header("NEW LOCATIONS")]
    [SerializeField] private NewLocation[] newLocations;
    [Header("REPUTATION")]
    [SerializeField] [Range(-3, 3)] private int reputation_Mages;
    [SerializeField] [Range(-3, 3)] private int reputation_Mutants;
    [SerializeField] [Range(-3, 3)] private int reputation_Rogues;
    [SerializeField] [Range(-3, 3)] private int reputation_Techs;
    [SerializeField] [Range(-3, 3)] private int reputation_Warriors;

    public string DialoguePromptText { get => dialoguePromptText; }
    public DialogueResponse DialogueResponse1 { get => dialogueResponse1; }
    public DialogueResponse DialogueResponse2 { get => dialogueResponse2; }
    public DialogueResponse DialogueResponse3 { get => dialogueResponse3; }

    public NPCHero NewEngagedHero { get => newEngagedHero; }
    public bool HideNPC { get => hideNPC; }
    public Card NewCard { get => newCard; }
    public int AetherCells { get => aetherCells; }
    public NewLocation[] NewLocations { get => newLocations; }

    public int Reputation_Mages { get => reputation_Mages; }
    public int Reputation_Mutants { get => reputation_Mutants; }
    public int Reputation_Rogues { get => reputation_Rogues; }
    public int Reputation_Techs { get => reputation_Techs; }
    public int Reputation_Warriors { get => reputation_Warriors; }

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
        aetherCells = dp.AetherCells;
        newLocations = (NewLocation[])dp.NewLocations.Clone();
        reputation_Mages = dp.Reputation_Mages;
        reputation_Mutants = dp.Reputation_Mutants;
        reputation_Rogues = dp.Reputation_Rogues;
        reputation_Techs = dp.Reputation_Techs;
        reputation_Warriors = dp.Reputation_Warriors;
    }
}
