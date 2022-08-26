using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Triggered Ability", menuName = "Card Abilities/Triggered Ability")]
public class TriggeredAbility : CardAbility
{
    public AbilityTrigger AbilityTrigger;
    public List<EffectGroup> EffectGroupList;

    public override void LoadCardAbility(CardAbility cardAbility)
    {
        base.LoadCardAbility(cardAbility);
        TriggeredAbility triggeredAbility = cardAbility as TriggeredAbility;
        AbilityTrigger = triggeredAbility.AbilityTrigger;
        EffectGroupList = new List<EffectGroup>();
        foreach (EffectGroup eg in triggeredAbility.EffectGroupList)
            EffectGroupList.Add(eg);
    }
}
