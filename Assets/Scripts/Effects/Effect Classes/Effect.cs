using UnityEngine;

public abstract class Effect : ScriptableObject
{
    [Tooltip("The effect is required by the effect group")]
    public bool IsRequired;

    [Tooltip("The value of the effect (1-10)")]
    [Range(1, 10)]
    public int Value;
    public bool IsNegative;

    [Tooltip("The number of turns the effect lasts, 0 for permanent")]
    [Range(0, 5)]
    public int Countdown;

    [Tooltip("If not null, the effect will only resolve if the target has this ability")]
    public CardAbility IfHasCondition;

    public virtual void LoadEffect(Effect effect)
    {
        IsRequired = effect.IsRequired;
        Value = effect.Value;
        IsNegative = effect.IsNegative;
        Countdown = effect.Countdown;
        IfHasCondition = effect.IfHasCondition;
    }
}
