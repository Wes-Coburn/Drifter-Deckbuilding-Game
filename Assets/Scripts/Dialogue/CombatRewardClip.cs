using UnityEngine;

[CreateAssetMenu(fileName = "New Combat Reward Clip", menuName = "Dialogue/Combat Reward Clip")]
public class CombatRewardClip : DialogueClip
{
    [Header("NEXT CLIP")]
    [SerializeField] private DialogueClip nextDialogueClip;
    [Header("NEW LOCATIONS")]
    [SerializeField] private NewLocation[] newLocations;
    [Header("NEW NARRATIVE")]
    [SerializeField] private Narrative newNarrative;
    [Header("NEW HERO")]
    [SerializeField] private bool newHero;
    [Header("NEW POWERS")]
    [SerializeField] private bool newPowers;
    [Header("REPUTATION")]
    [SerializeField, Range(-5, 5)] private int reputation_Mages, reputation_Mutants,
        reputation_Rogues, reputation_Techs, reputation_Warriors;

    public DialogueClip NextDialogueClip { get => nextDialogueClip; }
    public NewLocation[] NewLocations { get => newLocations; }
    public Narrative NewNarrative { get => newNarrative; }
    public bool NewHero { get => newHero; }
    public bool NewPowers { get => newPowers; }
    public int Reputation_Mages { get => reputation_Mages; }
    public int Reputation_Mutants { get => reputation_Mutants; }
    public int Reputation_Rogues { get => reputation_Rogues; }
    public int Reputation_Techs { get => reputation_Techs; }
    public int Reputation_Warriors { get => reputation_Warriors; }

    public override void LoadDialogueClip(DialogueClip dc)
    {
        base.LoadDialogueClip(dc);
        var crc = dc as CombatRewardClip;
        nextDialogueClip = crc.NextDialogueClip;
        newLocations = (NewLocation[])crc.NewLocations.Clone();
        newNarrative = crc.NewNarrative;
        newHero = crc.NewHero;
        newPowers = crc.NewPowers;
        reputation_Mages = crc.Reputation_Mages;
        reputation_Mutants = crc.Reputation_Mutants;
        reputation_Rogues = crc.Reputation_Rogues;
        reputation_Techs = crc.Reputation_Techs;
        reputation_Warriors = crc.Reputation_Warriors;
    }
}
