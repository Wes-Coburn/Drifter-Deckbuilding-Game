using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Modifier Ability", menuName = "Card Abilities/Modifier Ability")]
public class ModifierAbility : CardAbility
{
    public AbilityTrigger AbilityTrigger;
    public List<Effect> PlayUnitEffects;

    public override void LoadCardAbility(CardAbility cardAbility)
    {
        base.LoadCardAbility(cardAbility);
        ModifierAbility modifierAbility = cardAbility as ModifierAbility;
        AbilityTrigger = modifierAbility.AbilityTrigger;
        PlayUnitEffects = new List<Effect>();
        foreach (Effect e in modifierAbility.PlayUnitEffects)
            PlayUnitEffects.Add(e);
    }
}
