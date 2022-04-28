using UnityEngine;

[CreateAssetMenu(fileName = "New Combat Reward Clip", menuName = "Dialogue/Combat Reward Clip")]
public class CombatRewardClip : DialogueClip
{
    [SerializeField] private DialogueClip nextDialogueClip;
    [SerializeField] [Range(1, 30)] private int aetherCells;
    [SerializeField] private NewLocation[] newLocations;
    [SerializeField] private Narrative newNarrative;
    [Header("REPUTATION")]
    [SerializeField][Range(-5, 5)] private int reputation_Mages;
    [SerializeField][Range(-5, 5)] private int reputation_Mutants;
    [SerializeField][Range(-5, 5)] private int reputation_Rogues;
    [SerializeField][Range(-5, 5)] private int reputation_Techs;
    [SerializeField][Range(-5, 5)] private int reputation_Warriors;

    public DialogueClip NextDialogueClip { get => nextDialogueClip; }
    public int AetherCells { get => aetherCells; }
    public NewLocation[] NewLocations { get => newLocations; }
    public Narrative NewNarrative { get => newNarrative; }
    public int Reputation_Mages { get => reputation_Mages; }
    public int Reputation_Mutants { get => reputation_Mutants; }
    public int Reputation_Rogues { get => reputation_Rogues; }
    public int Reputation_Techs { get => reputation_Techs; }
    public int Reputation_Warriors { get => reputation_Warriors; }

    public override void LoadDialogueClip(DialogueClip dc)
    {
        base.LoadDialogueClip(dc);
        CombatRewardClip crc = dc as CombatRewardClip;
        nextDialogueClip = crc.NextDialogueClip;
        aetherCells = crc.AetherCells;
        newLocations = (NewLocation[])crc.NewLocations.Clone();
        newNarrative = crc.NewNarrative;
        reputation_Mages = crc.Reputation_Mages;
        reputation_Mutants = crc.Reputation_Mutants;
        reputation_Rogues = crc.Reputation_Rogues;
        reputation_Techs = crc.Reputation_Techs;
        reputation_Warriors = crc.Reputation_Warriors;
    }
}
