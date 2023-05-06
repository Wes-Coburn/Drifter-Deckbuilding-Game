using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Modifier Ability", menuName = "Card Abilities/Modifier Ability")]
public class ModifierAbility : CardAbility
{
    public List<EffectGroup> EffectGroupList;

    [Header("Trigger Limit"), Range(0, 1),
        Tooltip("Limit the number of times this ability triggers each turn, 0 for unlimited")]
    public int TriggerLimit;

    [Header("Remove After Trigger"), Tooltip("Remove this ability after it triggers")]
    public bool RemoveAfterTrigger;

    [Header("Ability Trigger Modifier")]
    public AbilityTrigger AbilityTrigger;
    public bool AllAbilityTriggers;

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
        EnemyDestroyed,
        MarkedEnemyDestroyed,
        PoisonedEnemyDestroyed,

        // Allies
        AllyDestroyed,
        AllyRefreshed,
        DamagedAllyHealed,
        AllyGainsAbility,

        // Traps
        AllyTrapDestroyed,

        // Play Card
        PlayCardWithCost,
    }
    public TriggerType SpecialTriggerType;
    public int SpecialTriggerValue;
    public List<Effect> SpecialTriggerEffects;
    public CardAbility AllyAbility;

    public int TriggerCount { get; set; }

    public override void LoadCardAbility(CardAbility cardAbility)
    {
        base.LoadCardAbility(cardAbility);
        var modifierAbility = cardAbility as ModifierAbility;

        EffectGroupList = modifierAbility.EffectGroupList.ToList();

        TriggerLimit = modifierAbility.TriggerLimit;
        RemoveAfterTrigger = modifierAbility.RemoveAfterTrigger;

        AbilityTrigger = modifierAbility.AbilityTrigger;
        AllAbilityTriggers = modifierAbility.AllAbilityTriggers;

        ModifyPlayUnit = modifierAbility.ModifyPlayUnit;
        PlayUnitEffects = modifierAbility.PlayUnitEffects.ToList();

        ModifyPlayAction = modifierAbility.ModifyPlayAction;
        PlayActionType = modifierAbility.PlayActionType;

        ModifySpecialTrigger = modifierAbility.ModifySpecialTrigger;
        SpecialTriggerType = modifierAbility.SpecialTriggerType;
        SpecialTriggerValue = modifierAbility.SpecialTriggerValue;
        SpecialTriggerEffects = modifierAbility.SpecialTriggerEffects.ToList();

        AllyAbility = modifierAbility.AllyAbility;

        TriggerCount = 0;
    }
}
