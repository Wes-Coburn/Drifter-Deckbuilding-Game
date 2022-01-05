using System.Collections.Generic;
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

    [Header("IF HAS ABILITY CONDITION")]
    [Tooltip("If not null, the effect will not resolve unless the target has this ABILITY")]
    public CardAbility IfHasAbilityCondition;

    [Header("IF HAS TRIGGER CONDITION")]
    [Tooltip("If not null, the effect will not resolve unless the target has this TRIGGER")]
    public AbilityTrigger IfHasTriggerCondition; // TESTING

    [Header("IF HAS ABILITY EFFECTS")]
    [Tooltip("If the target has this ability, resolve these effects")]
    public CardAbility IfHasAbility; // TESTING
    public List<EffectGroup> IfHasAbilityEffects; // TESTING

    [Header("IF HAS TRIGGER EFFECTS")]
    [Tooltip("If the target has this trigger, resolve these effects")]
    public AbilityTrigger IfHasTrigger; // TESTING
    public List<EffectGroup> IfHasTriggerEffects; // TESTING

    [Header("FOR EACH EFFECTS")]
    [Tooltip("Resolve these effects for each target")]
    public List<EffectGroup> ForEachEffects; // TESTING

    public virtual void LoadEffect(Effect effect)
    {
        IsRequired = effect.IsRequired;
        Value = effect.Value;
        IsNegative = effect.IsNegative;
        Countdown = effect.Countdown;
        IfHasAbilityCondition = effect.IfHasAbilityCondition;
        IfHasTriggerCondition = effect.IfHasTriggerCondition; // TESTING

        // TESTING
        IfHasAbility = effect.IfHasAbility;
        IfHasAbilityEffects = new List<EffectGroup>();
        foreach (EffectGroup eg in effect.IfHasAbilityEffects)
            IfHasAbilityEffects.Add(eg);

        // TESTING
        IfHasTrigger = effect.IfHasTrigger;
        IfHasTriggerEffects = new List<EffectGroup>();
        foreach (EffectGroup eg in effect.IfHasTriggerEffects)
            IfHasTriggerEffects.Add(eg);

        // TESTING
        ForEachEffects = new List<EffectGroup>();
        foreach (EffectGroup eg in effect.ForEachEffects)
            ForEachEffects.Add(eg);
    }
}
