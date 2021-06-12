using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Triggered Ability", menuName = "Card Abilities/Triggered Ability")]

public class TriggeredAbility : CardAbility
{
    public AbilityTrigger AbilityTrigger;
    public List<Effect> EffectGroup;
}
