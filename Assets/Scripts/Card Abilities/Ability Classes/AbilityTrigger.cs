using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Trigger", menuName = "Card Abilities/Ability Trigger")]
public class AbilityTrigger : CardAbility
{
    public Sound TriggerSound;

    public override void LoadCardAbility(CardAbility cardAbility)
    {
        base.LoadCardAbility(cardAbility);
        AbilityTrigger abilityTrigger = cardAbility as AbilityTrigger;
        TriggerSound = abilityTrigger.TriggerSound;
    }
}
