using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Trigger", menuName = "Card Abilities/Ability Trigger")]
public class AbilityTrigger : CardAbility
{
    [Range(0, 1), Tooltip("Limit the number of times this ability triggers each turn, 0 for none (overrides TriggerLimit of TriggeredAbility")]
    public int TriggerLimit;
    public Sound TriggerSound;

    public override void LoadCardAbility(CardAbility cardAbility)
    {
        base.LoadCardAbility(cardAbility);
        AbilityTrigger abilityTrigger = cardAbility as AbilityTrigger;
        TriggerLimit = abilityTrigger.TriggerLimit;
        TriggerSound = abilityTrigger.TriggerSound;
    }
}
