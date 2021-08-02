using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Fork", menuName = "Dialogue/Dialogue Fork")]
public class DialogueFork : DialogueClip
{
    [Header("CONDITION TYPE")]
    public bool IsRespectCondition;
        
    [Header("CONDITION VALUES")]
    [SerializeField] [Range(-10, 10)] private int response_1_Condition_Value;
    public int Response_1_Condition_Value
    {
        get => response_1_Condition_Value;
        private set => response_1_Condition_Value = value;
    }

    [SerializeField] [Range(-10, 10)] private int response_2_Condition_Value;
    public int Response_2_Condition_Value
    {
        get => response_2_Condition_Value;
        private set => response_2_Condition_Value = value;
    }
    
    [SerializeField] [Range(-10, 10)] private int response_3_Condition_Value;
    public int Response_3_Condition_Value
    {
        get => response_3_Condition_Value;
        private set => response_3_Condition_Value = value;
    }
}
