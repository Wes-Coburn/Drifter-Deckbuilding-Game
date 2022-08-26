using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trap Ability", menuName = "Card Abilities/Trap Ability")]
public class TrapAbility : TriggeredAbility
{
    public bool ResolveLast;
    public List<Effect> TrapEffects;
    public List<Effect> SelfEffects;

    public override void LoadCardAbility(CardAbility cardAbility)
    {
        base.LoadCardAbility(cardAbility);
        TrapAbility trapAbility = cardAbility as TrapAbility;
        ResolveLast = trapAbility.ResolveLast;
        TrapEffects = new List<Effect>();
        foreach (Effect effect in trapAbility.TrapEffects)
            TrapEffects.Add(effect);
        SelfEffects = new List<Effect>();
        foreach (Effect effect in trapAbility.SelfEffects)
            SelfEffects.Add(effect);
    }
}
