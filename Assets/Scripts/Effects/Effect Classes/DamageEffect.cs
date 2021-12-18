using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Effect", menuName = "Effects/Effect/Damage")]
public class DamageEffect : Effect
{
    [Tooltip("If the damage destroys a unit, this effect group resolves")]
    public List<EffectGroup> IfDestroyedEffects;
}
