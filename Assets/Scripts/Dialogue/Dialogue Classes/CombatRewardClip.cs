using UnityEngine;

[CreateAssetMenu(fileName = "New Combat Reward Clip", menuName = "Dialogue/Combat Reward Clip")]
public class CombatRewardClip : DialogueClip
{
    [SerializeField] private DialogueClip nextDialogueClip;
    [SerializeField] [Range(1, 30)] private int aetherCells;
    [SerializeField] private NewLocation[] newLocations;
    [SerializeField] private Narrative newNarrative;
    
    public DialogueClip NextDialogueClip { get => nextDialogueClip; }
    public int AetherCells { get => aetherCells; }
    public NewLocation[] NewLocations { get => newLocations; }
    public Narrative NewNarrative { get => newNarrative; }

    public override void LoadDialogueClip(DialogueClip dc)
    {
        base.LoadDialogueClip(dc);
        CombatRewardClip crc = dc as CombatRewardClip;
        nextDialogueClip = crc.NextDialogueClip;
        aetherCells = crc.AetherCells;
        newLocations = (NewLocation[])crc.NewLocations.Clone();
        newNarrative = crc.NewNarrative;
    }
}
