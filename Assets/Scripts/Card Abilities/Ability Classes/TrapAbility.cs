using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trap Ability", menuName = "Card Abilities/Trap Ability")]
public class TrapAbility : TriggeredAbility
{
    [Header("TRAP ABILITY")]

    public bool ResolveLast;
    public List<Effect> TrapEffects;
    public List<Effect> SelfEffects;
}
