using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : ScriptableObject
{
    [Header("IS REQUIRED"), Tooltip("The effect is required by the effect group")]
    public bool IsRequired;

    [Header("IGNORE LEGALITY"), Tooltip("Skip GetLegalTargets for this effect")]
    public bool IgnoreLegality;

    [Header("VALUE"), Tooltip("The value of the effect (1-10)"), Range(1, 10)]
    public int Value;
    public bool IsNegative;

    [Tooltip("The value of the effect is derived")]
    public bool IsDerivedValue;
    [Tooltip("Specifies where the value is derived from")]
    public DerivedValueType DerivedValue;
    
    public enum DerivedValueType
    {
        Source_Power,
        Source_Health
    }

    [Header("COUNTDOWN"), Tooltip("The number of turns the effect lasts, 0 if permanent"), Range(0, 5)]
    public int Countdown;

    [Header("SHOOT RAY")]
    public bool ShootRay;
    public Color RayColor;

    [Header("PRECHECK CONDITIONS"), Tooltip("If enabled, the effect's conditions will also be checked during GetLegalTargets")]
    public bool PreCheckConditions;
    [Tooltip("If enabled, the PreCheck will be done independently")]
    public bool CheckConditionsIndependent;

    [Header("IF WOUNDED CONDITIONS"), Tooltip("If enabled, any IfWounded conditions selected will be reversed")]
    public bool IfNotWoundedCondition;
    [Space, Tooltip("If enabled, the effect will not resolve unless ANY hero is Wounded")]
    public bool IfAnyWoundedCondition;
    [Tooltip("If enabled, the effect will not resolve unless the PLAYER hero's health is Wounded")]
    public bool IfPlayerWoundedCondition;
    [Tooltip("If enabled, the effect will not resolve unless the ENEMY hero's health is Wounded")]
    public bool IfEnemyWoundedCondition;

    [Header("IF EXHAUSTED CONDITION"), Tooltip("If enabled, the effect will not resolve unless the target IS EXHAUSTED")]
    public bool IfExhaustedCondition;

    [Header("IF REFRESHED CONDITION"), Tooltip("If enabled, the effect will not resolve unless the target IS NOT EXHAUSTED")]
    public bool IfRefreshedCondition;

    [Header("IF DAMAGED CONDITION"), Tooltip("If enabled, the effect will not resolve unless the target IS DAMAGED")]
    public bool IfDamagedCondition;

    [Header("IF NOT DAMAGED CONDITION"), Tooltip("If enabled, the effect will not resolve unless the target IS NOT DAMAGED")]
    public bool IfNotDamagedCondition;

    [Header("IF HAS POWER CONDITION"), Tooltip("If enabled, the effect will not resolve unless the target has GREATER POWER (or LESSER if also enabled)")]
    public bool IfHasPowerCondition;
    public bool IsLessPowerCondition;
    [Range(0, 10)] public int IfHasPowerValue;

    [Header("IF HAS ABILITY CONDITION"), Tooltip("If not null, the effect will not resolve unless the target HAS this ABILITY")]
    public CardAbility IfHasAbilityCondition;
    [Tooltip("If enabled, the effect will not resolve unless the target DOES NOT have the ABILITY")]
    public bool IfNotHasAbilityCondition;

    [Header("IF HAS TRIGGER CONDITION"), Tooltip("If not null, the effect will not resolve unless the target HAS this TRIGGER")]
    public AbilityTrigger IfHasTriggerCondition;
    [Tooltip("If enabled, the effect will not resolve unless the target DOES NOT have the TRIGGER")]
    public bool IfNotHasTriggerCondition;

    [Header("IF HAS ABILITY EFFECTS"), Tooltip("If the target has this ability, resolve these effects")]
    public CardAbility IfHasAbility;
    public List<EffectGroup> IfHasAbilityEffects;

    [Header("IF HAS TRIGGER EFFECTS"), Tooltip("If the target has this trigger, resolve these effects")]
    public AbilityTrigger IfHasTrigger;
    public List<EffectGroup> IfHasTriggerEffects;

    [Header("IF HAS POWER EFFECTS"), Tooltip("If the target has greater (or lesser) power, resolve these effects)")]
    public int IfHasGreaterPowerValue;
    public List<EffectGroup> IfHasGreaterPowerEffects;
    public int IfHasLowerPowerValue;
    public List<EffectGroup> IfHasLowerPowerEffects;

    [Header("IF RESOLVES EFFECTS"), Tooltip("If the effect resolves, resolve additional effects on the target")]
    public List<Effect> IfResolvesEffects;
    public bool ResolveSimultaneous;

    [Header("IF RESOLVES GROUPS"), Tooltip("If the effect resolves, resolve additional effect groups")]
    public List<EffectGroup> IfResolvesGroups;

    [Header("FOR EACH EFFECTS"), Tooltip("Resolve these effects for each target")]
    public List<EffectGroup> ForEachEffects;

    public virtual void LoadEffect(Effect effect)
    {
        IsRequired = effect.IsRequired;
        IgnoreLegality = effect.IgnoreLegality;

        Value = effect.Value;
        IsNegative = effect.IsNegative;

        IsDerivedValue = effect.IsDerivedValue;
        DerivedValue = effect.DerivedValue;

        Countdown = effect.Countdown;

        ShootRay = effect.ShootRay;
        RayColor = effect.RayColor;

        PreCheckConditions = effect.PreCheckConditions;

        IfAnyWoundedCondition = effect.IfAnyWoundedCondition;
        IfPlayerWoundedCondition = effect.IfPlayerWoundedCondition;
        IfEnemyWoundedCondition = effect.IfEnemyWoundedCondition;

        IfExhaustedCondition = effect.IfExhaustedCondition;
        IfRefreshedCondition = effect.IfRefreshedCondition;

        IfDamagedCondition = effect.IfDamagedCondition;
        IfNotDamagedCondition = effect.IfNotDamagedCondition;

        IfHasPowerCondition = effect.IfHasPowerCondition;
        IsLessPowerCondition = effect.IsLessPowerCondition;
        IfHasPowerValue = effect.IfHasPowerValue;

        IfHasAbilityCondition = effect.IfHasAbilityCondition;
        IfNotHasAbilityCondition = effect.IfNotHasAbilityCondition;

        IfHasTriggerCondition = effect.IfHasTriggerCondition;
        IfNotHasTriggerCondition = effect.IfNotHasTriggerCondition;

        IfHasAbility = effect.IfHasAbility;
        IfHasAbilityEffects = new List<EffectGroup>();
        foreach (EffectGroup eg in effect.IfHasAbilityEffects)
            IfHasAbilityEffects.Add(eg);

        IfHasTrigger = effect.IfHasTrigger;
        IfHasTriggerEffects = new List<EffectGroup>();
        foreach (EffectGroup eg in effect.IfHasTriggerEffects)
            IfHasTriggerEffects.Add(eg);

        IfHasGreaterPowerValue = effect.IfHasGreaterPowerValue;
        IfHasGreaterPowerEffects = new List<EffectGroup>();
        foreach (EffectGroup eg in effect.IfHasGreaterPowerEffects)
            IfHasGreaterPowerEffects.Add(eg);

        IfHasLowerPowerValue = effect.IfHasLowerPowerValue;
        IfHasLowerPowerEffects = new List<EffectGroup>();
        foreach (EffectGroup eg in effect.IfHasLowerPowerEffects)
            IfHasLowerPowerEffects.Add(eg);

        IfResolvesEffects = new List<Effect>();
        foreach (Effect e in effect.IfResolvesEffects)
            IfResolvesEffects.Add(e);

        ResolveSimultaneous = effect.ResolveSimultaneous;

        IfResolvesGroups = new List<EffectGroup>();
        foreach (EffectGroup eg in effect.IfResolvesGroups)
            IfResolvesGroups.Add(eg);

        ForEachEffects = new List<EffectGroup>();
        foreach (EffectGroup eg in effect.ForEachEffects)
            ForEachEffects.Add(eg);
    }
}
