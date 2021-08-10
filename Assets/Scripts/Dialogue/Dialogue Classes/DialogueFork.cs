using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Fork", menuName = "Dialogue/Dialogue Fork")]
public class DialogueFork : DialogueClip
{
    [SerializeField] private bool isRespectCondition;
    [Space]
    [SerializeField] private DialogueClip clip_1;
    [SerializeField] [Range(-10, 10)] private int clip_1_Condition_Value;
    [Space]
    [SerializeField] private DialogueClip clip_2;
    [SerializeField] [Range(-10, 10)] private int clip_2_Condition_Value;
    [Space]
    [SerializeField] private DialogueClip clip_3;
    [SerializeField] [Range(-10, 10)] private int clip_3_Condition_Value;

    public bool IsRespectCondition { get => isRespectCondition; }
    public DialogueClip Clip_1 { get => clip_1; }
    public DialogueClip Clip_2 { get => clip_2; }
    public DialogueClip Clip_3 { get => clip_3; }
    public int Clip_1_Condition_Value { get => clip_1_Condition_Value; }
    public int Clip_2_Condition_Value { get => clip_2_Condition_Value; }
    public int Clip_3_Condition_Value { get => clip_3_Condition_Value; }

    public override void LoadDialogueClip(DialogueClip dc)
    {
        base.LoadDialogueClip(dc);

        DialogueFork df = dc as DialogueFork;
        base.LoadDialogueClip(dc);
        isRespectCondition = df.IsRespectCondition;
        clip_1 = df.Clip_1;
        clip_2 = df.Clip_2;
        clip_3 = df.Clip_3;
        clip_1_Condition_Value = df.Clip_1_Condition_Value;
        clip_2_Condition_Value = df.Clip_2_Condition_Value;
        clip_3_Condition_Value = df.Clip_3_Condition_Value;
    }
}
