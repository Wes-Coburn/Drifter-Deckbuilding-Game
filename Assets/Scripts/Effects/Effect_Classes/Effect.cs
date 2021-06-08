using UnityEngine;

public abstract class Effect : ScriptableObject
{
    [Tooltip("The value of the effect (1-10)")]
    [Range(1, 10)]
    public int Value;

    [Tooltip("Targets: Self, Ally, Opponent, Enemy")]
    public string Targets;

    [Tooltip("The number of targets (1-3)")]
    [Range(1, 3)]
    public int TargetNumber;
}
