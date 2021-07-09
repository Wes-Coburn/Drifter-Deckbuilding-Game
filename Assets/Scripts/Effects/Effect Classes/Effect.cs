using UnityEngine;

public abstract class Effect : ScriptableObject
{
    [Tooltip("The effect is required by the effect group")]
    public bool IsRequired;

    [Tooltip("The value of the effect (1-10)")]
    [Range(1, 10)]
    public int Value;

    [Tooltip("Targets: Player, Ally, Opponent, Enemy")]
    public string Targets;

    [Tooltip("The number of targets (1-5)")]
    [Range(1, 5)]
    public int TargetNumber;
    
    [Tooltip("The effect targets all possible targets")]
    public bool TargetsAll;

    [Tooltip("The number of turns the effect lasts, 0 for permanent")]
    [Range(0, 5)]
    public int Countdown;

    public virtual void LoadEffect(Effect effect)
    {
        IsRequired = effect.IsRequired;
        Value = effect.Value;
        Targets = effect.Targets;
        TargetNumber = effect.TargetNumber;
        TargetsAll = effect.TargetsAll;
        Countdown = effect.Countdown;
    }
}
