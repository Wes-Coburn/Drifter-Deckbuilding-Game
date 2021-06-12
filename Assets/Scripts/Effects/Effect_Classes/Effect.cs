using UnityEngine;

public abstract class Effect : ScriptableObject
{
    [Tooltip("The effect is required by the effect group")]
    public bool IsRequired;

    [Tooltip("The value of the effect (1-10)")]
    [Range(1, 10)]
    public int Value;

    [Tooltip("Targets: Self, Ally, Opponent, Enemy")]
    public string Targets;

    [Tooltip("The number of targets (1-5)")]
    [Range(1, 5)]
    public int TargetNumber;
    
    [Tooltip("The effect targets all possible targets")]
    public bool TargetsAll;
}
