using UnityEngine;

[CreateAssetMenu(fileName = "New Combat Reward Clip", menuName = "Dialogue/Combat Reward Clip")]
public class CombatRewardClip : DialogueClip
{
    [SerializeField] private DialogueClip nextDialogueClip;
    [SerializeField] private Card newCard;
    [SerializeField] [Range(0, 5)] private int aetherCells;
    
    public DialogueClip NextDialogueClip { get => nextDialogueClip; }
    public Card NewCard { get => newCard; }
    public int AetherCells { get => aetherCells; }

    public override void LoadDialogueClip(DialogueClip dc)
    {
        base.LoadDialogueClip(dc);
        CombatRewardClip crc = dc as CombatRewardClip;
        nextDialogueClip = crc.NextDialogueClip;
        newCard = crc.NewCard;
        aetherCells = crc.AetherCells;
    }
}
