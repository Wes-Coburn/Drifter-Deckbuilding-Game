using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Modifier Ability", menuName = "Card Abilities/Modifier Ability")]
public class ModifierAbility : CardAbility
{
    [Header("MODIFIER ABILITY")]

    [Header("ABILITY TRIGGER")]
    public AbilityTrigger AbilityTrigger;

    [Header("PLAY UNIT")]
    public List<Effect> PlayUnitEffects;
}
