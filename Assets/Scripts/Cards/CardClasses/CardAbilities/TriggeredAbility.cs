using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Keyword Ability", menuName = "Card Abilities/Keyword Ability")]

public class TriggeredAbility : CardAbility
{
    public AbilityTrigger KeywordTrigger;
    public List<Effect> EffectGroup;
}
