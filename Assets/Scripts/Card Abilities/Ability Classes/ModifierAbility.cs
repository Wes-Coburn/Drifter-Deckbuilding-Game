using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Modifier Ability", menuName = "Card Abilities/Modifier Ability")]
public class ModifierAbility : CardAbility
{
    public List<EffectGroup> EffectGroupList;

    [Header("Trigger Limit"), Range(0, 1),
        Tooltip("Limit the number of times this ability triggers each turn, 0 for unlimited")]
    public int TriggerLimit;

    [Header("Ability Trigger Modifier")]
    public AbilityTrigger AbilityTrigger;

    [Header("Play Unit Modifiers")]
    public bool ModifyPlayUnit;
    public List<Effect> PlayUnitEffects;

    [Header("Play Action Modifiers")]
    public bool ModifyPlayAction;
    public string PlayActionType;

    [Header("Special Trigger Modifiers")]
    public bool ModifySpecialTrigger;
    public enum TriggerType
    {
        // Enemy Hero
        EnemyHeroWounded,

        // Enemies
        MarkedEnemyDestroyed,
        PoisonedEnemyDestroyed,

        // Allies
        AllyDestroyed,
        DamagedAllyHealed
    }
    public TriggerType SpecialTriggerType;

    public int TriggerCount { get; set; }

    public override void LoadCardAbility(CardAbility cardAbility)
    {
        base.LoadCardAbility(cardAbility);
        ModifierAbility modifierAbility = cardAbility as ModifierAbility;

        EffectGroupList = new List<EffectGroup>();
        foreach (EffectGroup eg in modifierAbility.EffectGroupList)
            EffectGroupList.Add(eg);

        TriggerLimit = modifierAbility.TriggerLimit;

        AbilityTrigger = modifierAbility.AbilityTrigger;

        ModifyPlayUnit = modifierAbility.ModifyPlayUnit;
        PlayUnitEffects = new List<Effect>();
        foreach (Effect e in modifierAbility.PlayUnitEffects)
            PlayUnitEffects.Add(e);

        ModifyPlayAction = modifierAbility.ModifyPlayAction;
        PlayActionType = modifierAbility.PlayActionType;

        ModifySpecialTrigger = modifierAbility.ModifySpecialTrigger;
        SpecialTriggerType = modifierAbility.SpecialTriggerType;

        TriggerCount = 0;
    }
}
